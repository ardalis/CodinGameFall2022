partial class Player
{

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

        public override int MatterCost => 10;


        public override string ToString()
        {
            return $"SPAWN {Amount} {X} {Y}";
        }
    }
}