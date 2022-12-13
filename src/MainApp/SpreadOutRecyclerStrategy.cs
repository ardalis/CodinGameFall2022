partial class Player
{


    public class SpreadOutRecyclerStrategy : IRecyclerStrategy
    {
        public IEnumerable<Tile> GetBuildLocations(GameState gameState)
        {
            var location = gameState.BuildableTiles.ToList()
                    .Where(t => t.x > 0 && t.y > 0 && t.x < gameState.Width - 1 && t.y < gameState.Height - 1) // not on edges
                    .Where(t => !gameState.MyRecyclers.Any(r => gameState.DistanceBetween(t, r) <= 3)) // more than 3 away from existing recyclers
                    .OrderByDescending(t => gameState.DistanceFromNearestEnemyUnit(t))
                    .FirstOrDefault(t => !gameState.FriendlyMoveDestinations.Contains(t));

            yield return location;
        }
    }

}