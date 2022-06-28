using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CashApp.DataBase;

namespace CashApp.Wallets
{
	partial class GroupWallet : UserControl, IWallet
	{
		// FIELDS

		public double Amount
		{
			get => 0;
			set { }
		}

		public long CurrencyId
		{
			get => 0;
			set { }
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
			get => XBody.Background;
			set {
				XBody.Background = value;
				XSecondBody1.Background = value;
				XSecondBody2.Background = value;
			}
		}

		int _walletsCount;
		public int WalletsCount
		{
			get => _walletsCount;
			set
			{
				_walletsCount = value;
				XWallets.Text = $"{value} кошельков";
			}
		}


		// EVENTS

		public event EventHandler Click;


		// METHODS

		public void SetData(WalletDB data)
		{
			Capture = data.Capture;
			Icon = data.Icon;
			Backcolor = data.Backcolor;
			WalletsCount = DBData.Wallets.Count(w => w.GroupId == data.Id);
		}

		public void GetData(WalletDB data)
		{
			data.Type = WalletType.Group;
			data.Amount = Amount;
			data.CurrencyId = CurrencyId;
			data.Capture = Capture;
			data.Icon = Icon;
			data.Backcolor = (SolidColorBrush)Backcolor;
		}


		bool isPessed = false;
		void Wallet_MouseDown(object sender, RoutedEventArgs e)
		{
			XBody.Margin = new Thickness { Top = 2 };
			XSecondBody1.Margin = new Thickness(5, 6, 5, 0);

			isPessed = true;
		}

		void Wallet_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!isPessed) return;

			XBody.Margin = new Thickness(0);
			XSecondBody1.Margin = new Thickness(5, 5, 5, 0);

			isPessed = false;
		}

		void Wallet_MouseUp(object sender, RoutedEventArgs e)
		{
			if (!isPessed) return;
			
			Wallet_MouseLeave(this, null);
			Click?.Invoke(this, new EventArgs());
		}


		// CONSTRUCTORS

		public GroupWallet()
		{
			InitializeComponent();
		}
	}
}
