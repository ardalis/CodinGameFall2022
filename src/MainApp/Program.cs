using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    public record Cell(int x, int y);

    public record Tile(int x, int y, int scrapAmount, int owner, int units,
        bool recycler, bool canBuild, bool canSpawn, bool inRangeOfRecycler) : Cell(x, y)
    { }

    public class GameState
    {
        public GameState(int width, int height)
        {
            Width = width;
            Height = height;
            TopLeft = new Cell(0, 0);
            TopRight = new Cell(Width - 1, 0);
            BottomLeft = new Cell(0, Height - 1);
            BottomRight = new Cell(Width - 1, Height - 1);
        }

        public int Width { get; }
        public int Height { get; }

        public Cell TopLeft { get; }
        public Cell TopRight { get; }
        public Cell BottomLeft { get; }
        public Cell BottomRight { get; }

        public int DistanceFromCenter(Cell t)
        {
            int horizontal = Math.Abs(Width / 2 - t.x);
            int vertical = Math.Abs(Height / 2 - t.y);

            return horizontal + vertical;
        }

        public int DistanceBetween(Cell a, Cell b)
        {
            int horizontal = Math.Abs(a.x - b.x);
            int vertical = Math.Abs(a.y - b.y);

            return horizontal + vertical;
        }

        public List<Tile> Tiles { get; } = new();

        public void AddTile(Tile tile) => Tiles.Add(tile);

        public IEnumerable<Tile> MyTiles => Tiles.Where(t => t.owner == 1);
        public IEnumerable<Tile> NotMyTiles => Tiles.Where(t => t.owner != 1 && t.scrapAmount > 0);
        public IEnumerable<Tile> NeutralTiles => Tiles.Where(t => t.owner == -1);
        public IEnumerable<Tile> EnemyTiles => Tiles.Where(t => t.owner == 0);
        public IEnumerable<Tile> EmptyEnemyTiles => EnemyTiles.Where(t => !t.recycler);
        public IEnumerable<Tile> MyRecyclers => MyTiles.Where(t => t.recycler);
        public IEnumerable<Tile> EnemyRecyclers => EnemyTiles.Where(t => t.recycler);


        public IEnumerable<Tile> MyUnits => MyTiles.Where(t => t.units > 0);
        public IEnumerable<Tile> EnemyUnits => EnemyTiles.Where(t => t.units > 0);

        public IEnumerable<Tile> SpawnableTiles => Tiles.Where(t => t.canSpawn);
        public IEnumerable<Tile> BuildableTiles => Tiles.Where(t => t.canBuild);
    }

    public abstract class GameAction
    {
    }

    public class Move : GameAction
    {
        public Move(int amount, int fromX, int fromY, int toX, int toY)
        {
            Amount = amount;
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }

        public int Amount { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }

        public override string ToString()
        {
            return $"MOVE {Amount} {FromX} {FromY} {ToX} {ToY}";
        }
    }

    public class Build : GameAction
    {
        public Build(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"BUILD {X} {Y}";
        }
    }

    public class Spawn : GameAction
    {
        public Spawn(int amount, int x, int y)
        {
            Amount = amount;
            X = x;
            Y = y;
        }

        public int Amount { get; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"SPAWN {Amount} {X} {Y}";
        }
    }

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);

        // game loop
        while (true)
        {
            var gameState = new GameState(width, height);
            var actions = new List<GameAction>();
            inputs = Console.ReadLine().Split(' ');
            int myMatter = int.Parse(inputs[0]);
            int oppMatter = int.Parse(inputs[1]);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int scrapAmount = int.Parse(inputs[0]);
                    int owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
                    int units = int.Parse(inputs[2]);
                    int recycler = int.Parse(inputs[3]);
                    int canBuild = int.Parse(inputs[4]);
                    int canSpawn = int.Parse(inputs[5]);
                    int inRangeOfRecycler = int.Parse(inputs[6]);

                    var tile = new Tile(j, i, scrapAmount, owner, units, recycler > 0, canBuild > 0, canSpawn > 0, inRangeOfRecycler > 0);
                    gameState.AddTile(tile);
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            // calculate important destinations
            // Tile topLeftNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.TopLeft, t)).FirstOrDefault();
            // Tile topRightNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.TopRight, t)).FirstOrDefault();
            // Tile bottomLeftNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.BottomLeft, t)).FirstOrDefault();
            // Tile bottomRightNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.BottomRight, t)).FirstOrDefault();

            //int unitNumber = 0;
            var destinationCells = new List<Cell>();
            foreach (var unit in gameState.MyUnits)
            {
                // send unit closest to corner to that corner
                //if(unit == gameState.MyUnits.OrderByDescending(u => gameState.DistanceBetween(unit, gameState.TopLeft)))

                var closestCell = gameState.NotMyTiles
                                    .OrderBy(t => gameState.DistanceBetween(unit, t))
                                    .FirstOrDefault();
                //                                    .FirstOrDefault(t => !destinationCells.Contains(t));
                if (closestCell != null)
                {
                    destinationCells.Add(closestCell);
                    actions.Add(new Move(1, unit.x, unit.y, closestCell.x, closestCell.y));
                }


                // var destination = gameState.NotMyTiles.Skip(unitNumber).FirstOrDefault();
                // if(destination != null)
                // {
                //     var command = new Move(1, unit.x, unit.y, destination.x, destination.y);
                //     actions.Add(command);
                // } else {
                //     destination = gameState.NeutralTiles.Skip(unitNumber).FirstOrDefault();
                //     if(destination != null)
                //     {
                //         var command = new Move(1, unit.x, unit.y, destination.x, destination.y);
                //         actions.Add(command);
                //     }
                // }
                // unitNumber++;
            }

            // SPAWN
            if (myMatter > 10)
            {
                var location = gameState.SpawnableTiles.ToList()
                                .OrderBy(t => gameState.DistanceFromCenter(t))
                                .FirstOrDefault(t => !destinationCells.Contains(t));
                if (location != null)
                {
                    var command = new Spawn((myMatter - 10) / 10, location.x, location.y);
                    actions.Add(command);
                }
            }



            // BUILD
            if (gameState.MyRecyclers.Count() <= gameState.EnemyRecyclers.Count() && myMatter >= 10)
            {
                var location = gameState.BuildableTiles.ToList()
                    .OrderByDescending(t => t.scrapAmount)
                    .FirstOrDefault(t => !t.inRangeOfRecycler);

                if (location != null)
                {
                    var command = new Build(location.x, location.y);
                    destinationCells.Add(new Cell(location.x, location.y));
                    actions.Add(command);
                }
            }

            var actionsString = string.Join(';', actions.Select(a => a.ToString()));
            if (actionsString.Any())
            {
                actionsString += $";MESSAGE Bots: {gameState.MyUnits.Sum(x => x.units)} - {gameState.EnemyUnits.Sum(x => x.units)}";
                Console.WriteLine(actionsString);
            }
            else
            {
                Console.WriteLine("WAIT");
            }

        }
    }
}