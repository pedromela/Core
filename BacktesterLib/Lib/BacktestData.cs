namespace BacktesterLib.Lib
{
    public class BacktestData
    {
        public int Positions { get; set; }
        public int Successes { get; set; }


        public BacktestData(int Positions, int Successes)
        {
            this.Positions = Positions;
            this.Successes = Successes;
        }
    }
}
