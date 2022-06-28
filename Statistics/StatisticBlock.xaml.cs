using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Statistics
{
    public partial class StatisticBlock : UserControl
	{
		// CLASSES

		static class TabBuilder
		{
			static Grid CreateContainer(StackPanel histogram, ScrollPanel categories, TextBlock amountsSum)
            {
				Grid container = new Grid();

				container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
				container.ColumnDefinitions.Add(new ColumnDefinition());

				container.RowDefinitions.Add(new RowDefinition());
				container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });


				histogram.SetValue(Grid.RowSpanProperty, 2);

				categories.SetValue(Grid.ColumnProperty, 1);

				amountsSum.SetValue(Grid.ColumnProperty, 1);
				amountsSum.SetValue(Grid.RowProperty, 1);


				container.Children.Add(histogram);
				container.Children.Add(categories);
				container.Children.Add(amountsSum);

				return container;
			}

			static StackPanel CreateHistogram(Dictionary<CategoryDB, double> amounts, double amountsSum)
			{
				StackPanel histogram = new StackPanel { Margin = new Thickness(15) };


				foreach (CategoryDB category in amounts.Keys)
				{
					double ratio = amounts[category] / amountsSum;

					histogram.Children.Add(new Rectangle
					{
						Height = 200 * ratio,
						Fill = category.Backcolor
					});
				}


				return histogram;
			}

			static ScrollPanel CreateCategories(Dictionary<CategoryDB, double> amounts, string currency, double amountsSum)
			{
				StackPanel list = new StackPanel();

				foreach (CategoryDB category in amounts.Keys)
					list.Children.Add(new CategoryItem(category, amounts[category], currency, amountsSum));

				return new ScrollPanel
				{
					Content = list,
					Margin = new Thickness { Top = 15, Bottom = 5 }
				};
			}


			public static Tab Build(string currency, Dictionary<CategoryDB, double> amounts)
            {
				double amountsSum = amounts.Values.Sum();

				StackPanel histogram = CreateHistogram(amounts, amountsSum);
				ScrollPanel categories = CreateCategories(amounts, currency, amountsSum);

				TextBlock amountsSumText = new TextBlock
				{
					Text = $"Всего: {new Balance(amountsSum, currency)}",
					Foreground = Colors.MainText,
					FontSize = 14,
					FontWeight = FontWeights.DemiBold,
					Margin = new Thickness { Right = 15 },
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Top
				};

				Grid container = CreateContainer(histogram, categories, amountsSumText);

				return new Tab(currency, container);
			}
        }


		// FIELDS

		AmountType _type;
        public AmountType Type
		{
			get => _type;
			set
            {
				_type = value;
				XCapture.Text = value == AmountType.Profit ? "Доходы" : "Расходы";
            }
		}


		// METODS

		CurrencyDB FindOrCreateCurrency(Dictionary<CurrencyDB, Dictionary<CategoryDB, double>> tabsData, long currencyId)
        {
			CurrencyDB currency = tabsData.Keys.FirstOrDefault(c => c.Id == currencyId);

			if (currency == null)
			{
				currency = DBData.Currencies.Find(c => c.Id == currencyId);
				tabsData[currency] = new Dictionary<CategoryDB, double>();
			}

			return currency;
		}

		CategoryDB FindOrCreateCategory(Dictionary<CategoryDB, double> amounts, long categoryId)
		{
			CategoryDB category = amounts.Keys.FirstOrDefault(c => c.Id == categoryId);

			if (category == null)
			{
				if (categoryId != 0)
					category = DBData.Categories.Find(c => c.Id == categoryId);
				else
					category = new CategoryDB { Id = 0, Name = "Другое", Backcolor = Brushes.Gray };

				amounts[category] = 0;
			}

			return category;
		}

		public void DownloadStatistics()
		{
			XTabs.Items.Clear();

			var tabsData = new Dictionary<CurrencyDB, Dictionary<CategoryDB, double>>();

			TransactionDB[] transactions = DBData.Transactions.Where(t => t.Type == Type).ToArray();


			foreach (TransactionDB transaction in transactions)
			{
				long currencyId = (long)DBData.Wallets.Find(w => w.Id == transaction.WalletId).CurrencyId;

				if (currencyId == 0) continue;


				CurrencyDB currency = FindOrCreateCurrency(tabsData, currencyId);

				CategoryDB category = FindOrCreateCategory(tabsData[currency], (long)transaction.CategoryId);

				tabsData[currency][category] += (double)transaction.Amount;
			}


			foreach (CurrencyDB currency in tabsData.Keys)
			{
				Dictionary<CategoryDB, double> amounts = tabsData[currency];

				amounts = Sorter.Sort(amounts, (key1, key2) => amounts[key1] > amounts[key2] ? key1 : key2);

				XTabs.Items.Add(TabBuilder.Build(currency.Currency, amounts));
			}
		}


		// CONSTRUCTOR

		public StatisticBlock()
		{
			InitializeComponent();
		}
	}
}
