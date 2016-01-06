using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BidAskBox.Library
{
    public partial class BidAskBoxControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(Symbol), typeof(BidAskBoxControl));

        public class ClickArgs : EventArgs
        {
            public ClickArgs(Side side, double price)
            {
                Side = side;
                Price = price;
            }

            public Side Side { get; }
            public double Price { get; }
        }

        private double? lastBid = null;
        private double? lastAsk = null;
        private double spreadInPips = 0;
        private string bidMajor = "0";
        private string bidPips = "0";
        private string bidTicks = "0";
        private string askMajor = "0";
        private string askPips = "0";
        private string askTicks = "0";

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ClickArgs> OnClick;

        public BidAskBoxControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public Symbol Symbol
        {
            get
            {
                return (Symbol)GetValue(SymbolProperty);
            }
            set
            {
                SetValue(SymbolProperty, value);
            }
        }

        public Delta Delta { get; private set; }

        public void SetBidAsk(double bid, double ask)
        {
            if (lastBid.HasValue && (bid != lastBid))
            {
                Delta = (bid > lastBid) ? Delta.Up : Delta.Down;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Delta)));
            }

            GetParts(bid, ref bidMajor, ref bidPips, ref bidTicks);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidMajor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidPips)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidTicks)));

            GetParts(ask, ref askMajor, ref askPips, ref askTicks);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskMajor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskPips)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskTicks)));

            spreadInPips = Math.Round((ask - bid) * (Symbol == Symbol.USDJPY ? 100 : 10000), 1);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpreadInPips)));

            lastBid = bid;
            lastAsk = ask;
        }

        public string BidMajor => bidMajor;
        public string BidPips => bidPips;
        public string BidTicks => bidTicks;
        public string AskMajor => askMajor;
        public string AskPips => askPips;
        public string AskTicks => askTicks;
        public double SpreadInPips => spreadInPips;

        private void GetParts(double value,
            ref string major, ref string pips, ref string ticks)
        {
            var price = value.ToString(Symbol == Symbol.USDJPY ? "000.000" : "0.00000");

            major = TrimLeadingZeros(price.Substring(0, 4));
            pips = price.Substring(4, 2);
            ticks = price.Substring(6, 1);
        }

        private string TrimLeadingZeros(string value)
        {
            if (Symbol != Symbol.USDJPY)
                return value;

            return value.TrimStart('0');
        }

        public DelegateCommand BidClickCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (lastBid.HasValue)
                        OnClick?.Invoke(this, new ClickArgs(Side.Buy, lastBid.Value));
                });
            }
        }

        public DelegateCommand AskClickCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (lastAsk.HasValue)
                        OnClick?.Invoke(this, new ClickArgs(Side.Sell, lastAsk.Value));
                });
            }
        }
    }
}
