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

        public int MyGameEdgeXValue { get; set; } = 0;

        public int DistanceBetween(Cell a, Cell b)
        {
            int horizontal = Math.Abs(a.x - b.x);
            int vertical = Math.Abs(a.y - b.y);

            return horizontal + vertical;
        }

        public IEnumerable<Tile> NeighborsOf(Tile t)
        {
            var allTiles = Tiles.ToArray();

            var neighbors = new List<Tile>();
            int index = t.x + t.y * (Width - 1);

            if (t.y > 0) neighbors.Add(allTiles[index - Width]);
            if (t.x > 0) neighbors.Add(allTiles[index - 1]);
            if (t.x < Width - 1) neighbors.Add(allTiles[index + 1]);
            if (t.y < Height - 1) neighbors.Add(allTiles[index + Width]);

            return neighbors;
        }

        public List<Tile> FriendlyMoveDestinations { get; } = new();

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
        public int MyTotalUnits => MyUnits.Sum(x => x.units);
        public IEnumerable<Tile> EnemyUnits => EnemyTiles.Where(t => t.units > 0);
        public int EnemyTotalUnits => EnemyUnits.Sum(x => x.units);

        public IEnumerable<Tile> SpawnableTiles => Tiles.Where(t => t.canSpawn);
        public IEnumerable<Tile> BuildableTiles => Tiles.Where(t => t.canBuild);

        public int DistanceFromNearestEnemyUnit(Tile t)
        {
            var nearest = EnemyUnits.OrderBy(eu => DistanceBetween(t, eu)).FirstOrDefault();
            if (nearest == null) return 100;

            return DistanceBetween(t, nearest);
        }
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
        int turn = 0;
        int myGameEdgeXValue = 0;

        // game loop
        while (true)
        {
            turn++;
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

            // determine which edge is my side (left or right)
            if (turn == 1)
            {
                if (gameState.MyTiles.First().x < width / 2)
                {
                    myGameEdgeXValue = 0;
                }
                else
                {
                    myGameEdgeXValue = height - 1;
                }
            }
            gameState.MyGameEdgeXValue = myGameEdgeXValue;

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            // calculate important destinations
            // Tile topLeftNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.TopLeft, t)).FirstOrDefault();
            // Tile topRightNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.TopRight, t)).FirstOrDefault();
            // Tile bottomLeftNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.BottomLeft, t)).FirstOrDefault();
            // Tile bottomRightNotMine = gameState.NotMyTiles.OrderBy(t => gameState.DistanceBetween(gameState.BottomRight, t)).FirstOrDefault();

            //int unitNumber = 0;

            foreach (var unit in gameState.MyUnits)
            {
                // send unit closest to corner to that corner
                //if(unit == gameState.MyUnits.OrderByDescending(u => gameState.DistanceBetween(unit, gameState.TopLeft)))

                // try exploring or just fighting?
                int destinationCount = 2;
                var closestCells = gameState.EnemyTiles
                                    .Where(t => !(t.scrapAmount == 1 && t.inRangeOfRecycler)) // don't move to death zone
                                    .OrderBy(t => gameState.DistanceBetween(unit, t))
                                    .Take(destinationCount)
                                    .ToList();
                //                                    .FirstOrDefault(t => !destinationCells.Contains(t));
                if (closestCells != null)
                {
                    int unitsMoved = 0;
                    foreach (var cell in closestCells)
                    {
                        if (unitsMoved < unit.units)
                        {
                            int unitsToMove = Math.Max(1, unit.units / destinationCount);
                            actions.Add(new Move(unitsToMove, unit.x, unit.y, cell.x, cell.y));
                            unitsMoved += unitsToMove;
                            gameState.FriendlyMoveDestinations.Add(cell);
                        }
                    }
                }
            }



            // BUILD
            //if (gameState.MyRecyclers.Count() <= gameState.EnemyRecyclers.Count()) // don't need these

            int advantage = gameState.MyTotalUnits - gameState.EnemyTotalUnits;
            if (turn > 0 &&
                (turn < 12 || advantage < 1) &&
                myMatter >= 10)
            {
                var location = new SpreadOutRecyclerStrategy().GetBuildLocations(gameState).FirstOrDefault();

                if (location != null)
                {
                    var command = new Build(location.x, location.y);
                    //destinationCells.Add(new Cell(location.x, location.y));
                    myMatter -= 10;
                    actions.Add(command);
                }
            }

            // SPAWN
            if (myMatter > 10)
            {
                var location = gameState.SpawnableTiles.ToList()
                                .OrderBy(t => gameState.DistanceFromNearestEnemyUnit(t))
                                .FirstOrDefault();
                if (location != null)
                {
                    var command = new Spawn((myMatter) / 10, location.x, location.y);
                    actions.Add(command);
                }
            }




            var actionsString = string.Join(';', actions.Select(a => a.ToString()));
            if (actionsString.Any())
            {
                actionsString += $";MESSAGE Bots: {gameState.MyTotalUnits} - {gameState.EnemyTotalUnits}";
                Console.WriteLine(actionsString);
            }
            else
            {
                Console.WriteLine("WAIT");
            }

        }
    }

    public interface IRecyclerStrategy
    {
        IEnumerable<Player.Tile> GetBuildLocations(GameState gameState);
    }

    public class SpreadOutRecyclerStrategy : IRecyclerStrategy
    {
        public IEnumerable<Player.Tile> GetBuildLocations(GameState gameState)
        {
            var location = gameState.BuildableTiles.ToList()
                    .Where(t => t.x > 0 && t.y > 0 && t.x < gameState.Width - 1 && t.y < gameState.Height - 1) // not on edges
                    .Where(t => !gameState.MyRecyclers.Any(r => gameState.DistanceBetween(t, r) <= 3)) // more than 3 away from existing recyclers
                    .OrderByDescending(t => gameState.DistanceFromNearestEnemyUnit(t))
                    .FirstOrDefault(t => !gameState.FriendlyMoveDestinations.Contains(t));

            yield return location;
        }
    }

    // Encircle strategy
    // MOVE
    // IF enemy unit is reachable
    //   IF enemy is outnumbered OR more than 2 moves away: MOVE toward enemy
    //   ELSE (we are close) move AROUND enemy (if enemy is lower left, move left or south, etc.)
    //
    // BUILD
    // IF enemy units near my edge are adjacent to my tiles
    //   GET ALL MY EMPTY TILES WITH ENEMY ADJACENT ORDER BY distance to my edge
    // BUILD as many recyclers as I can (even trying to build more than I have)
}