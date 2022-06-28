using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Linq;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;
using CashApp.Transactions;
using CashApp.Planned;
using CashApp.Categories;
using CashApp.Templates;

namespace CashApp.Wallets
{
    public partial class WalletsBlock : UserControl
	{
		// CLASSES

		class GroupsLinks
		{
			// FIELDS

			readonly StackPanel Container;

			readonly List<Link> Links = new List<Link>();

			readonly List<long> GroupsIds = new List<long>();


			public Link SelectedLink => Count > 0 ? Links[Count - 1] : null;

			public long SelectedGroupId => Count > 0 ? GroupsIds[Count - 1] : -1;

			public int Count => Links.Count;


			// EVENTS

			public event EventHandler Transition;


			// METHODS

			void Link_Click(object sender, EventArgs e)
			{
				if (SelectedLink == sender) return;


				int index = Links.FindLastIndex(link => link == sender) + 1;

				Container.Children.RemoveRange(index, Container.Children.Count - index);

				Links.RemoveRange(index, Links.Count - index);
				GroupsIds.RemoveRange(index, GroupsIds.Count - index);


				Transition?.Invoke(SelectedLink, new EventArgs());
			}

			public void Add(string capture, long groupId)
			{
				Link link = new Link
				{
					Text = $"→ {capture}",
					FontSize = 14,
					Margin = new Thickness { Left = 5 },
					VerticalAlignment = VerticalAlignment.Center
				};

				link.Click += Link_Click;


				Links.Add(link);
				GroupsIds.Add(groupId);

				Container.Children.Add(link);

				Transition?.Invoke(SelectedLink, new EventArgs());
			}

			public void InitializeMainGroup()
			{
				Container.Children.Clear();
				Links.Clear();
				GroupsIds.Clear();

				Add("Кошельки", 0);

				SelectedLink.Text = "Кошельки";
				SelectedLink.FontWeight = FontWeights.Bold;
			}


			// CONSTRUCTORS

			public GroupsLinks(StackPanel container) => Container = container;
		}

		class WalletComplect
		{
			public StackPanel Body;

			public IWallet Wallet;

			public WalletDB Data;

			public DropMenu Menu;

            DropMenu Build_Menu(WalletType? type)
			{
				DropMenu menu = new DropMenu { Width = 190 };

				if (type != WalletType.Group)
                {
					menu.Items.Add(new TextBlock { Text = ADD_OPTION, Foreground = Colors.MainText });
					menu.Items.Add(new TextBlock { Text = PLANNED_OPTION, Foreground = Colors.MainText });
					menu.Items.Add(new TextBlock { Text = CATEGORY_OPTION, Foreground = Colors.MainText });
					menu.Items.Add(new TextBlock { Text = TEMPLATE_OPTION, Foreground = Colors.MainText });
				}
				menu.Items.Add(new TextBlock { Text = CHANGE_OPTION, Foreground = Colors.MainText });
				menu.Items.Add(new TextBlock { Text = REMOVE_OPTION, Foreground = Colors.MainText });

				return menu;
			}

			public WalletComplect(WalletDB data)
			{
				Data = data;

				Body = new StackPanel { Margin = new Thickness { Left = 20, Top = 20 } };
				Wallet = data.CreateWallet();
				Menu = Build_Menu(data.Type);

				(Wallet as UIElement).MouseRightButtonUp += (sender, e) => Menu.IsOpen = true;

				Body.Children.Add((UIElement)Wallet);
				Body.Children.Add(Menu);
			}
		}


		// FIELDS

		const string ADD_OPTION = "➕  Добавить транзакцию",
					 PLANNED_OPTION = "🕒  Сланировать транзакцию",
					 CATEGORY_OPTION = " ▢  Добавить категорию",
					 TEMPLATE_OPTION = " ⎘  Создать шаблон",
					 CHANGE_OPTION = "✎  Изменить",
					 REMOVE_OPTION = "🗑  Удалить";

		readonly WrapPanel XWallets;
		readonly TextBlock XTotal;

		readonly GroupsLinks Links;
		readonly List<WalletComplect> Complects = new List<WalletComplect>();

		public MainWindow Window;


		// METODS

		// Wallet

		void XAddButton_Click(object sender, EventArgs e)
		{
			WalletForm window = new WalletForm();

			if (!window.ShowDialog_Create(Links.SelectedGroupId)) return;

			AddWallet(window.WalletData);

			SetTotalAmount(Links.SelectedGroupId);
		}

		void AddWallet(WalletDB data)
		{
			WalletComplect complect = new WalletComplect(data);

			if (complect.Data.Type != WalletType.Group)
				complect.Wallet.Click += (sender, e) => AddTransaction((long)complect.Data.Id);
			else
				complect.Wallet.Click += (sender, e) => Links.Add(complect.Data.Capture, (long)complect.Data.Id);

			complect.Menu.SelectionChanged += (sender, e) => Menu_SelectionChanged((sender as TextBlock).Text, complect);

			XWallets.Children.Add(complect.Body);
			Complects.Add(complect);
		}

		void ChangeWallet(WalletComplect complect)
		{
			WalletForm window = new WalletForm();

			if (!window.ShowDialog_Change(complect.Data)) return;

			complect.Wallet.SetData(window.WalletData);

			Window.Transactions.DownloadTransactions();
			Window.StatisticProfits.DownloadStatistics();
			Window.StatisticLosses.DownloadStatistics();
		}

		void RemoveWallet(WalletComplect complect)
		{
			if (MsgBox.Show($"Удалить \"{complect.Data.Capture}\"?", "Кошельки", MsgIcon.Question, "Да", "Нет") == "Нет") return;


			XWallets.Children.Remove(complect.Body);
			DBData.Wallets.Remove(complect.Data);
			Complects.Remove(complect);

			SetTotalAmount(Links.SelectedGroupId);


			Window.Transactions.DownloadTransactions();
			Window.StatisticProfits.DownloadStatistics();
			Window.StatisticLosses.DownloadStatistics();

		}

		// Menu

		void AddTransaction(long walletId)
        {
			TransactionForm window = new TransactionForm();

			if (!window.ShowDialog_Create(walletId)) return;


			Window.Transactions.DownloadTransactions();
			DownloadWallets();

			if (window.TransactionData.Type == AmountType.Profit)
				Window.StatisticProfits.DownloadStatistics();
			else
				Window.StatisticLosses.DownloadStatistics();
		}

		void Menu_SelectionChanged(string option, WalletComplect complect)
		{
			switch (option)
			{
				case ADD_OPTION:
					AddTransaction((long)complect.Data.Id);
					break;


				case PLANNED_OPTION:
					new PlannedForm().ShowDialog_Create((long)complect.Data.Id);
					break;

				case CATEGORY_OPTION:
					new CategoryForm().ShowDialog_Create((long)complect.Data.Id);
					break;

				case TEMPLATE_OPTION:
					new TemplateForm().ShowDialog_Create((long)complect.Data.Id);
					break;


				case CHANGE_OPTION:
					ChangeWallet(complect);
					break;


				case REMOVE_OPTION:
					RemoveWallet(complect);
					break;
			}
		}

		// Base

		public void DownloadWallets() => SetWallets(Links.SelectedGroupId);

		void SetWallets(long groupId)
        {
			XWallets.Children.Clear();
			Complects.Clear();

			WalletDB[] wallets = DBData.Wallets.Where(w => w.GroupId == groupId).ToArray();

			foreach (WalletDB wallet in wallets) AddWallet(wallet);

			SetTotalAmount(groupId);
		}

		void SetTotalAmount(long groupId)
		{
			StringBuilder totals = new StringBuilder();

			var amounts = WalletDB.GetTotalsAmounts(groupId);
			Balance balance = new Balance();

			foreach (string currency in amounts.Keys)
            {
				balance.Amount = amounts[currency];
				balance.Currency = currency;
				totals.Append($"{balance}  ");
            }

			XTotal.Text = totals.ToString();
		}

		public void InitializeMainGroup() => Links.InitializeMainGroup();


		// CONSTRUCTORS

		public WalletsBlock()
		{
			InitializeComponent();

			XAddWallet.Click += XAddButton_Click;

			XWallets = (WrapPanel)_XWallets_.Content;
			XTotal = (TextBlock)(_XTotal_.Content as StackPanel).Children[1];

			Links = new GroupsLinks((StackPanel)_XNavigate_.Content);
			Links.Transition += (sender, e) => SetWallets(Links.SelectedGroupId);
		}
	}
}
