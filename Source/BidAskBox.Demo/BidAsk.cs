using BidAskBox.Library;
using System;

namespace BidAskBox.Demo
{
    internal class BidAsk
    {
        public Symbol Symbol { get; private set; }
        public DateTime TickOn { get; private set; }
        public double Bid { get; private set; }
        public double Ask { get; private set; }

        public static BidAsk Parse(string line)
        {
            var fields = line.Split(',');

            return new BidAsk()
            {
                Symbol = (Symbol)Enum.Parse(typeof(Symbol), fields[0]),
                TickOn = DateTime.Parse(fields[1]),
                Bid = double.Parse(fields[2]),
                Ask = double.Parse(fields[3])
            };
        }
    }
}
