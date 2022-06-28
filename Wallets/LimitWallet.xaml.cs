using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

using CashApp.DataBase;
using CashApp.Instruments;

namespace CashApp.Wallets
{
    partial class LimitWallet : UserControl, IWallet
    {
        readonly Balance AmountBalance = new Balance(),
                         LimitBalance = new Balance();

        void UpdateProgress()
        {
            if (Amount == 0 | Limit == 0) return;

            double ratio;

            if (Limit == 0) ratio = 1;       // 5 / 0 (limit = 100%)
            else if (Amount == 0) ratio = 0; // 0 / 5 (limit = 0%)
            else
            {
                ratio = Amount / Limit;

                if (ratio > 1) ratio = 1;
                else if (ratio < 0) ratio = 0;
            }

            XProgressIndicate.Width = XProgressSpace.ActualWidth * ratio;
        }

        public double Amount
        {
            get => AmountBalance.Amount;
            set
            {
                AmountBalance.Amount = value;
                XBalance.Text = AmountBalance.ToString();
                UpdateProgress();
            }
        }

        long _currencyId;
        public long CurrencyId
        {
            get => _currencyId;
            set
            {
                _currencyId = value;

                string currency = value != 0 ? DBData.Currencies.Find(c => c.Id == value).Currency : null;
                AmountBalance.Currency = LimitBalance.Currency = currency;

                XBalance.Text = AmountBalance.ToString();
                XLimit.Text = $"/ {LimitBalance}";
            }
        }

        public string Capture
        {
            get => XCapture.Text;
            set => XCapture.Text = value;
        }

        public ImageSource Icon
        {
            get => XIcon.Source;
            set => XIcon.Source = value;
        }

        public Brush Backcolor
        {
            get => XTopPanel.Background;
            set
            {
                XTopPanel.Background = value;
                XProgressIndicate.Fill = value;
            }
        }

        public double Limit
        {
            get => LimitBalance.Amount;
            set
            {
                if (value < 0) throw new ArgumentException("\"Limit\" is less than zero");

                LimitBalance.Amount = value;
                XLimit.Text = $"/ {LimitBalance}";
                UpdateProgress();
            }
        }


        public event EventHandler Click;


        public void SetData(WalletDB data)
        {
            Amount = (double)data.Amount;
            CurrencyId = (long)data.CurrencyId;
            Capture = data.Capture;
            Icon = data.Icon;
            Backcolor = data.Backcolor;
            Limit = double.Parse(data.Addition);
        }

        public void GetData(WalletDB data)
        {
            data.Type = WalletType.Limit;
            data.Amount = Amount;
            data.CurrencyId = CurrencyId;
            data.Capture = Capture;
            data.Icon = Icon;
            data.Backcolor = (SolidColorBrush)Backcolor;
            data.Addition = Limit.ToString();
        }


        bool isPessed = false;
        void Wallet_MouseDown(object sender, RoutedEventArgs e)
        {
            XBody.Margin = new Thickness { Top = 3, Bottom = -3 };

            XShadow.Effect = new DropShadowEffect
            {
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.2,
                Direction = 270
            };

            isPessed = true;
        }

        void Wallet_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isPessed) return;

            XBody.Margin = new Thickness();
            XShadow.Effect = new DropShadowEffect
            {
                BlurRadius = 25,
                ShadowDepth = 10,
                Opacity = 0.2,
                Direction = 270
            };

            isPessed = false;
        }

        void Wallet_MouseUp(object sender, RoutedEventArgs e)
        {
            if (isPessed)
            {
                Wallet_MouseLeave(this, null);
                Click?.Invoke(this, new EventArgs());
            }
        }

        void Wallet_Loaded(object sender, RoutedEventArgs e) => UpdateProgress();


        public LimitWallet()
        {
            InitializeComponent();
        }
    }
}
