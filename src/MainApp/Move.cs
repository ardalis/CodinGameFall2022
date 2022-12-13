partial class Player
{

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

        public override int MatterCost => 0;

        public override string ToString()
        {
            return $"MOVE {Amount} {FromX} {FromY} {ToX} {ToY}";
        }
    }
}