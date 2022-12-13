partial class Player
{

    public record Tile(int x, int y, int scrapAmount, int owner, int units,
    bool recycler, bool canBuild, bool canSpawn, bool inRangeOfRecycler) : Cell(x, y)
    { }
}