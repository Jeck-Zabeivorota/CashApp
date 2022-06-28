using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Transactions
{
    public partial class TransactionsBlock : UserControl
	{
		// CLASSES

		class TransactionComplect
		{
			public StackPanel Body;

			public FlatButton Transaction;

			public TransactionDB Data;

			public DropMenu Menu;


			DropMenu Build_Menu()
			{
				DropMenu menu = new DropMenu { Width = 120 };

				menu.Items.Add(new TextBlock { Text = DUBLICATE_OPTION, Foreground = Colors.MainText });
				menu.Items.Add(new TextBlock { Text = CHANGE_OPTION, Foreground = Colors.MainText });
				menu.Items.Add(new TextBlock { Text = REMOVE_OPTION, Foreground = Colors.MainText });

				return menu;
			}


			public TransactionComplect(TransactionDB data)
			{
				Data = data;

				Body = new StackPanel();

				Transaction = new FlatButton
				{
					Child = new TransactionItem(data),
					Background = Brushes.Transparent,
					MouseOver_Color = Colors.Accent1,
					MouseDown_Color = Colors.Accent2,
					CornerRadius = new CornerRadius()
				};

				Menu = Build_Menu();

				Transaction.MouseRightButtonUp += (sender, e) => Menu.IsOpen = true;

				Body.Children.Add(Transaction);
				Body.Children.Add(Menu);
			}
		}


		// FIELDS

		const string DUBLICATE_OPTION = "⧉  Дублировать",
					 CHANGE_OPTION = "✎  Изменить",
					 REMOVE_OPTION = "🗑  Удалить";

		readonly StackPanel XTransactions;
		readonly List<TransactionComplect> Complects = new List<TransactionComplect>();

        readonly FilterData Filter = new FilterData();

		public MainWindow Window;


		// METODS

		void AddTransaction(TransactionDB data, int index = -1)
		{
			TransactionComplect complect = new TransactionComplect(data);

			complect.Transaction.Click += (sender, e) => ChangeTransaction(complect);

			complect.Menu.SelectionChanged += (sender, e) => Menu_SelectionChanged((sender as TextBlock).Text, complect);


			if (index < 0)
            {
				XTransactions.Children.Add(complect.Body);
				Complects.Add(complect);
			}
			else
            {
				XTransactions.Children.Insert(index, complect.Body);
				Complects.Insert(index, complect);
			}
		}

		void DuplicateTransaction(long transactionId)
        {
            TransactionDB data = new TransactionDB(transactionId) { Date = System.DateTime.Now };

            DBData.Transactions.Add(data);

			AddTransaction(data, 0);


			Window.Wallets.DownloadWallets();

			if (data.Type == AmountType.Profit)
				Window.StatisticProfits.DownloadStatistics();
			else
				Window.StatisticLosses.DownloadStatistics();
		}

		void ChangeTransaction(TransactionComplect complect)
        {
			TransactionForm window = new TransactionForm();

			if (!window.ShowDialog_Change(complect.Data)) return;
			
			DownloadTransactions();
			Window.Wallets.DownloadWallets();
			Window.StatisticProfits.DownloadStatistics();
			Window.StatisticLosses.DownloadStatistics();
		}

		void RemoveTransaction(TransactionComplect complect)
		{
			if (MsgBox.Show("Удалить транзакцию?", "Транзакции", MsgIcon.Question, "Да", "Нет") == "Нет") return;

            
			XTransactions.Children.Remove(complect.Body);
			DBData.Transactions.Remove(complect.Data);
			Complects.Remove(complect);


			Window.Wallets.DownloadWallets();

			if (complect.Data.Type == AmountType.Profit)
				Window.StatisticProfits.DownloadStatistics();
			else
				Window.StatisticLosses.DownloadStatistics();
		}


		void XFilter_Click(object sender, EventArgs e)
		{
			FilterForm window = new FilterForm();

			if (window.ShowDialog_Change(Filter))
				DownloadTransactions();
		}

		void Menu_SelectionChanged(string option, TransactionComplect complect)
		{
			switch (option)
			{
				case DUBLICATE_OPTION:
					DuplicateTransaction((long)complect.Data.Id);
					break;

				case CHANGE_OPTION:
					ChangeTransaction(complect);
					break;

				case REMOVE_OPTION:
					RemoveTransaction(complect);
					break;
			}
		}

		public void DownloadTransactions()
		{
			XTransactions.Children.Clear();
			Complects.Clear();


			TransactionDB[] transactions;
			Func<TransactionDB, bool> predicate = Filter?.GetPredicate();

			if (predicate == null)
				transactions = DBData.Transactions.ToArray();
			else
				transactions = DBData.Transactions.Where(predicate).ToArray();


			transactions = Sorter.Sort(transactions, (item1, item2) => item1.Date > item2.Date ? item1 : item2).ToArray();

			foreach (TransactionDB transaction in transactions) AddTransaction(transaction);
		}

		
		// CONSTRUCTORS

		public TransactionsBlock()
		{
			InitializeComponent();
			XTransactions = (StackPanel)_XTransactions_.Content;

            XFilter.Click += XFilter_Click;
		}
    }
}
