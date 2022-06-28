using System;
using System.Windows;
using System.Windows.Controls;

using CashApp.DataBase;
using CashApp.UIElements;

namespace CashApp.Currencies
{
    public partial class CurrenciesInform : Window
	{
		readonly MainWindow Window;


		Grid CreateItem(CurrencyDB data)
		{
			Grid body = new Grid();

			body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
			body.ColumnDefinitions.Add(new ColumnDefinition());


			TextBlock currency = new TextBlock
			{
				Text = data.Currency,
				Foreground = Colors.MainText,
				HorizontalAlignment = HorizontalAlignment.Left
			};


			TextBlock name = new TextBlock
			{
				Text = data.Name,
				Foreground = Colors.MainText,
				HorizontalAlignment = HorizontalAlignment.Left
			};
			name.SetValue(Grid.ColumnProperty, 1);


			body.Children.Add(currency);
			body.Children.Add(name);

			return body;
		}

		void XAdd_Click(object sender, EventArgs e)
		{
			CurrencyForm window = new CurrencyForm();

			if (!window.ShowDialog_Create()) return;

			XCurrencies.Items.Add(CreateItem(window.CurrencyData));
		}

		void XDelete_Click(object sender, EventArgs e)
		{
			if (XCurrencies.SelectItem == null)
			{
				MsgBox.Show("Валюта не выбранная!", "Валюты", MsgIcon.Error, "Ок");
				return;
			}


			int index = XCurrencies.SelectIndex;

			string result = MsgBox.Show(
				$"После удаления валюты, будут удалены все звязаные кошельки.\nУдалить \"{DBData.Currencies[index].Currency}\"?",
				"Валюты", MsgIcon.Question, "Да", "Нет");

			if (result == "Нет") return;


			XCurrencies.Items.RemoveAt(index);
			DBData.Currencies.RemoveAt(index);

			Window.Wallets.DownloadWallets();
			Window.Transactions.DownloadTransactions();
			Window.StatisticProfits.DownloadStatistics();
			Window.StatisticLosses.DownloadStatistics();
		}

		void XChange_Click(object sender, EventArgs e)
		{
			if (XCurrencies.SelectItem == null)
            {
				MsgBox.Show("Валюта не выбранная!", "Валюты", MsgIcon.Error, "Ок");
				return;
            }


			int index = XCurrencies.SelectIndex;

			CurrencyForm window = new CurrencyForm();

			if (!window.ShowDialog_Change(DBData.Currencies[index])) return;


			Grid body = (Grid)XCurrencies.Items[index];
			((TextBlock)body.Children[0]).Text = window.CurrencyData.Currency;
			((TextBlock)body.Children[1]).Text = window.CurrencyData.Name;

			Window.Wallets.DownloadWallets();
			Window.Transactions.DownloadTransactions();
			Window.StatisticProfits.DownloadStatistics();
			Window.StatisticLosses.DownloadStatistics();
		}


		public CurrenciesInform(MainWindow window)
		{
			InitializeComponent();

			XTop.CloseButton.Click += (sender, e) => Close();
			XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
			XTop.Body.MouseDown += (sender, e) => DragMove();


			XAdd.Click += XAdd_Click;
			XDelete.Click += XDelete_Click;
			XChange.Click += XChange_Click;


			Window = window;

			foreach (CurrencyDB currency in DBData.Currencies)
				XCurrencies.Items.Add(CreateItem(currency));
		}
	}
}
