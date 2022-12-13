partial class Player
{

    public interface IRecyclerStrategy
    {
        IEnumerable<Tile> GetBuildLocations(GameState gameState);
    }
}