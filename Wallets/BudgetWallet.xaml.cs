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
	public partial class BudgetWallet : UserControl, IWallet
	{
		readonly Balance AmountBalance = new Balance(),
						 BudgetBalance = new Balance(),
						 AvailableBalance = new Balance();


		void UpdateProgress()
		{
			if (StartDate == null || Period.IsNullSpan()) return;

			double ratio;

			if (Budget == 0) ratio = 0;      // 5 / 0 (loss = 0%)
			else if (Amount == 0) ratio = 1; // 0 / 5 (loss = 100%)
			else
            {
				ratio = Amount / Budget;

				if (ratio > 1) ratio = 1;
				else if (ratio < 0) ratio = 0;
			}

			ratio = 1 - ratio;

			XLossIndicate.Width = XProgressSpace.ActualWidth * ratio;


			TimeSpan allSpan = StartDate + Period - StartDate,
					 localSpan = DateTime.Now - StartDate;

			if (allSpan.Days == 0) ratio = 1;        // 5 / 0 (saved = 100%)
			else if (localSpan.Days == 0) ratio = 0; // 0 / 5 (saved = 0%)
			else
            {
				ratio = localSpan.Days / (double)allSpan.Days;

				if (ratio > 1) ratio = 1;
				else if (ratio < 0) ratio = 0;
			}

			XAvailableIndicate.Width = XProgressSpace.ActualWidth * ratio;
			
			AvailableBalance.Amount = Budget * ratio - (Budget - Amount);

			XAvailable.Text = $"({AvailableBalance})";
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
				AmountBalance.Currency = BudgetBalance.Currency = AvailableBalance.Currency = currency;

				XBalance.Text = AmountBalance.ToString();
				XBudget.Text = $"/ {BudgetBalance}";

				UpdateProgress();
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
				XLossIndicate.Fill = value;
				XAvailableIndicate.Fill = value;
			}
		}

		public double Budget
		{
			get => BudgetBalance.Amount;
			set
			{
				if (value < 0) throw new ArgumentException("\"Butget\" is less than zero");

				BudgetBalance.Amount = value;
				XBudget.Text = $"/ {BudgetBalance}";
				UpdateProgress();
			}
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get => _startDate;
			set
			{
				_startDate = value;
				UpdateProgress();
			}
		}

		DateSpan _period;
		public DateSpan Period
		{
			get => _period;
			set
			{
				_period = value;
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

			string[] addition = data.Addition.Split(';');
			Budget = double.Parse(addition[0]);
			StartDate = DateTime.Parse(addition[1]);
			Period = new DateSpan
			{
				Days = uint.Parse(addition[2]),
				Weeks = uint.Parse(addition[3]),
				Mounts = uint.Parse(addition[4]),
			};
		}

		public void GetData(WalletDB data)
		{
			data.Type = WalletType.Limit;
			data.Amount = Amount;
			data.CurrencyId = CurrencyId;
			data.Capture = Capture;
			data.Icon = Icon;
			data.Backcolor = (SolidColorBrush)Backcolor;
			data.Addition = $"{Budget};{StartDate:dd.MM.yyyy};{Period.Days};{Period.Weeks};{Period.Mounts}";
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


		public BudgetWallet()
		{
			InitializeComponent();
		}
	}
}
