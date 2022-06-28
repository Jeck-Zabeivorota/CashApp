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
    public partial class SimpleWallet : UserControl, IWallet
    {
        // FIELDS

        readonly Balance AmountBalance = new Balance();

        public double Amount
        {
            get => AmountBalance.Amount;
            set
            {
                AmountBalance.Amount = value;
                XBalance.Text = AmountBalance.ToString();
            }
        }

        long _currencyId;
        public long CurrencyId
        {
            get => _currencyId;
            set
            {
                _currencyId = value;
                AmountBalance.Currency = value != 0 ? DBData.Currencies.Find(c => c.Id == value).Currency : null;
                XBalance.Text = AmountBalance.ToString();
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
            set => XTopPanel.Background = value;
        }


        // EVENTS

        public event EventHandler Click;


        // METHODS

        public void SetData(WalletDB data)
        {
            Amount = (double)data.Amount;
            CurrencyId = (long)data.CurrencyId;
            Capture = data.Capture;
            Icon = data.Icon;
            Backcolor = data.Backcolor;
        }

        public void GetData(WalletDB data)
        {
            data.Type = WalletType.Simple;
            data.Amount = Amount;
            data.CurrencyId = CurrencyId;
            data.Capture = Capture;
            data.Icon = Icon;
            data.Backcolor = (SolidColorBrush)Backcolor;
            data.Addition = " ";
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


        // CONSTRUCTORS

        public SimpleWallet()
        {
            InitializeComponent();
        }
    }
}
