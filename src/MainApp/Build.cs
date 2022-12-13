partial class Player
{
    public class Build : GameAction
    {
        public Build(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override int MatterCost => 10;

        public override string ToString()
        {
            return $"BUILD {X} {Y}";
        }
    }
}