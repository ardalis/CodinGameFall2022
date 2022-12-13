partial class Player
{

    public class GameState
    {
        public GameState(int width, int height, int myMatter, int oppMatter, int turn)
        {
            Width = width;
            Height = height;
            MyMatter = myMatter;
            OppMatter = oppMatter;
            Turn = turn;
            TopLeft = new Cell(0, 0);
            TopRight = new Cell(Width - 1, 0);
            BottomLeft = new Cell(0, Height - 1);
            BottomRight = new Cell(Width - 1, Height - 1);
        }

        public int Width { get; }
        public int Height { get; }
        public int MyMatter { get; }
        public int OppMatter { get; }
        public int Turn { get; }
        public Cell TopLeft { get; }
        public Cell TopRight { get; }
        public Cell BottomLeft { get; }
        public Cell BottomRight { get; }

        public List<GameAction> Actions { get; } = new();

        public int MyGameEdgeXValue { get; set; } = 0;

        public int DistanceBetween(Cell a, Cell b)
        {
            int horizontal = Math.Abs(a.x - b.x);
            int vertical = Math.Abs(a.y - b.y);

            return horizontal + vertical;
        }

        public int MyRemainingMatter => MyMatter - Actions.Sum(a => a.MatterCost);

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

        public Tile NearestEnemy(Tile friendlyUnit)
        {
            return EnemyTiles.OrderBy(t => DistanceBetween(t, friendlyUnit)).FirstOrDefault();
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
}