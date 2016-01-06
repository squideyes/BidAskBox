using BidAskBox.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BidAskBox.Demo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        private Dictionary<Symbol, BidAskBoxControl> controls =
            new Dictionary<Symbol, BidAskBoxControl>();

        private string status;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            AddControl(Symbol.AUDUSD, audUsdBox);
            AddControl(Symbol.EURUSD, eurUsdBox);
            AddControl(Symbol.GBPUSD, gbpUsdBox);
            AddControl(Symbol.USDCAD, usdCadBox);
            AddControl(Symbol.USDCHF, usdChfBox);
            AddControl(Symbol.USDJPY, usdJpyBox);

            var maxDelay = Properties.Settings.Default.MaxDelayInMS;

            Task.Factory.StartNew(async () =>
            {
                var random = new Random();

                foreach (var bidAsk in GetBidAsks())
                {
                    if (cts.IsCancellationRequested)
                        return;

                    await Task.Delay(random.Next(maxDelay), cts.Token);

                    if (!cts.IsCancellationRequested)
                        HandleBidAsk(bidAsk);
                }
            },
            cts.Token);

            Status = "Click a Bid/Ask box to show the clicked rate is selected";
        }

        private void AddControl(Symbol symbol, BidAskBoxControl control)
        {
            controls.Add(symbol, control);

            control.OnClick += (s, e) =>
            {
                Status = $"Clicked {symbol} {e.Side} {e.Price}";
            };
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;

                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        private void HandleBidAsk(BidAsk bidAsk)
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                controls[bidAsk.Symbol].SetBidAsk(bidAsk.Bid, bidAsk.Ask);
            }));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cts.Cancel();
        }

        private List<BidAsk> GetBidAsks()
        {
            var bidAsks = new List<BidAsk>();

            using (var reader = new StringReader(Properties.Resources.Ticks20120102))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                    bidAsks.Add(BidAsk.Parse(line));
            }

            return bidAsks;
        }
    }
}
