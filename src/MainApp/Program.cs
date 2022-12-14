using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

partial class Player
{
    public class DefaultStrategy
    {
        public string GetActions(GameState gameState)
        {
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
                            gameState.Actions.Add(new Move(unitsToMove, unit.x, unit.y, cell.x, cell.y));
                            unitsMoved += unitsToMove;
                            gameState.FriendlyMoveDestinations.Add(cell);
                        }
                    }
                }
            }



            // BUILD
            //if (gameState.MyRecyclers.Count() <= gameState.EnemyRecyclers.Count()) // don't need these

            int advantage = gameState.MyTotalUnits - gameState.EnemyTotalUnits;
            if (gameState.Turn > 0 &&
                (gameState.Turn < 12 || advantage < 1) &&
                gameState.MyMatter >= 10)
            {
                var location = new SpreadOutRecyclerStrategy().GetBuildLocations(gameState).FirstOrDefault();

                if (location != null)
                {
                    var command = new Build(location.x, location.y);
                    //destinationCells.Add(new Cell(location.x, location.y));
                    gameState.Actions.Add(command);
                }
            }

            // SPAWN
            if (gameState.MyRemainingMatter > 10)
            {
                var location = gameState.SpawnableTiles.ToList()
                                .OrderBy(t => gameState.DistanceFromNearestEnemyUnit(t))
                                .FirstOrDefault();
                if (location != null)
                {
                    var command = new Spawn(gameState.MyRemainingMatter / 10, location.x, location.y);
                    gameState.Actions.Add(command);
                }
            }




            var actionsString = string.Join(';', gameState.Actions.Select(a => a.ToString()));

            return actionsString;
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
            inputs = Console.ReadLine().Split(' ');
            int myMatter = int.Parse(inputs[0]);
            int oppMatter = int.Parse(inputs[1]);
            var gameState = new GameState(width, height, myMatter, oppMatter, turn);
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

            var strategy = new DefaultStrategy();

            var actionsString = strategy.GetActions(gameState);

            //foreach (var unit in gameState.MyUnits)
            //{
            //    // send unit closest to corner to that corner
            //    //if(unit == gameState.MyUnits.OrderByDescending(u => gameState.DistanceBetween(unit, gameState.TopLeft)))

            //    // try exploring or just fighting?
            //    int destinationCount = 2;
            //    var closestCells = gameState.EnemyTiles
            //                        .Where(t => !(t.scrapAmount == 1 && t.inRangeOfRecycler)) // don't move to death zone
            //                        .OrderBy(t => gameState.DistanceBetween(unit, t))
            //                        .Take(destinationCount)
            //                        .ToList();
            //    //                                    .FirstOrDefault(t => !destinationCells.Contains(t));
            //    if (closestCells != null)
            //    {
            //        int unitsMoved = 0;
            //        foreach (var cell in closestCells)
            //        {
            //            if (unitsMoved < unit.units)
            //            {
            //                int unitsToMove = Math.Max(1, unit.units / destinationCount);
            //                actions.Add(new Move(unitsToMove, unit.x, unit.y, cell.x, cell.y));
            //                unitsMoved += unitsToMove;
            //                gameState.FriendlyMoveDestinations.Add(cell);
            //            }
            //        }
            //    }
            //}



            //// BUILD
            ////if (gameState.MyRecyclers.Count() <= gameState.EnemyRecyclers.Count()) // don't need these

            //int advantage = gameState.MyTotalUnits - gameState.EnemyTotalUnits;
            //if (turn > 0 &&
            //    (turn < 12 || advantage < 1) &&
            //    myMatter >= 10)
            //{
            //    var location = new SpreadOutRecyclerStrategy().GetBuildLocations(gameState).FirstOrDefault();

            //    if (location != null)
            //    {
            //        var command = new Build(location.x, location.y);
            //        //destinationCells.Add(new Cell(location.x, location.y));
            //        myMatter -= 10;
            //        actions.Add(command);
            //    }
            //}

            //// SPAWN
            //if (myMatter > 10)
            //{
            //    var location = gameState.SpawnableTiles.ToList()
            //                    .OrderBy(t => gameState.DistanceFromNearestEnemyUnit(t))
            //                    .FirstOrDefault();
            //    if (location != null)
            //    {
            //        var command = new Spawn((myMatter) / 10, location.x, location.y);
            //        actions.Add(command);
            //    }
            //}




            //var actionsString = string.Join(';', actions.Select(a => a.ToString()));
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