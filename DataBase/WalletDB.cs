using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CashApp.Wallets;
using CashApp.Instruments;

namespace CashApp.DataBase
{
	public enum WalletType { Simple, Limit, Budget, Group }

	public class WalletDB : DBClient
	{
		public WalletType? Type
		{
			get => (WalletType?)Columns["Type"].Value;
			set => ChangeColumnValue("Type", value);
		}

		public double? Amount
		{
			get => (double?)Columns["Amount"].Value;
			set => ChangeColumnValue("Amount", value);
		}

		public long? CurrencyId
		{
			get => (long?)Columns["CurrencyId"].Value;
			set => ChangeColumnValue("CurrencyId", value);
		}

		public string Capture
		{
			get => (string)Columns["Capture"].Value;
			set => ChangeColumnValue("Capture", value);
		}

		public SolidColorBrush Backcolor
		{
			get => (SolidColorBrush)Columns["Backcolor"].Value;
			set => ChangeColumnValue("Backcolor", value);
		}

		public ImageSource Icon
        {
			get => (ImageSource)Columns["Icon"].Value;
			set
			{
				ImageSource icon;

				if (value != null && (value.Width > 30 || value.Height > 30))
					icon = BitmapMaster.Resize((BitmapSource)value, 30, 30);

				else icon = value;

				ChangeColumnValue("Icon", icon);
			}
		}

		public string Addition
		{
			get => (string)Columns["Addition"].Value;
			set => ChangeColumnValue("Addition", value);
		}

		public long? GroupId
		{
			get => (long?)Columns["GroupId"].Value;
			set => ChangeColumnValue("GroupId", value);
		}


		public static Dictionary<string, double> GetTotalsAmounts(long groupId)
		{
			var totals = new Dictionary<string, double>();


			WalletDB[] walletsFromGroup = DBData.Wallets.Where(w => w.GroupId == groupId).ToArray();

			CurrencyDB[] currencies = ( from w in walletsFromGroup where w.Type != WalletType.Group
										select DBData.Currencies.Find(c => c.Id == w.CurrencyId) ).Distinct().ToArray();


			foreach (CurrencyDB currency in currencies)
			{
				WalletDB[] wallets = walletsFromGroup.Where(w => w.CurrencyId == currency.Id).ToArray();


				foreach (WalletDB wallet in wallets)
				
					if (wallet.Type != WalletType.Group)
					{
						if (!totals.ContainsKey(currency.Currency))
							totals[currency.Currency] = 0;

						totals[currency.Currency] += (double)wallet.Amount;
					}
					else
					{
						var localTotals = GetTotalsAmounts((long)wallet.Id);

						foreach (string _currency in localTotals.Keys)
						{
							if (!totals.ContainsKey(_currency))
								totals[_currency] = 0;

							totals[_currency] += localTotals[_currency];
						}
					}
			}

			return totals;
		}

		internal IWallet CreateWallet()
        {
			IWallet wallet;

			switch (Type)
			{
				case WalletType.Simple:
					wallet = new SimpleWallet();
					break;

				case WalletType.Limit:
					wallet = new LimitWallet();
					break;

				case WalletType.Budget:
					wallet = new BudgetWallet();
					break;

				default:
					wallet = new GroupWallet();
					break;
			}
			
			wallet.SetData(this);
			
			return wallet;
		}

		static void WalletDB_PreviewRemoved(object sender, EventArgs e)
		{
			WalletDB wallet = (WalletDB)sender;

			if (wallet.Type != WalletType.Group)
            {
				var transactionsFromWallet = DBData.Transactions.Where(t => t.WalletId == wallet.Id).ToArray();

				foreach (TransactionDB transaction in transactionsFromWallet)
						DBData.Transactions.Remove(transaction);


				var plannedFromWallet = DBData.Planned.Where(p => p.WalletId == wallet.Id).ToArray();

				foreach (PlannedDB plan in plannedFromWallet)
					DBData.Planned.Remove(plan);
			}
			else
			{
				var walletsFromGroup = DBData.Wallets.Where(w => w.GroupId == wallet.Id).ToArray();

				foreach (WalletDB _wallet in walletsFromGroup)
						DBData.Wallets.Remove(_wallet);
			}
		}


        void Initialize()
		{
			// Type

			string name = "Type";
			Columns[name] = new Column(name, DBType.TEXT)
			{
				ValueToDB = value => value.ToString(),
				DBToValue = db_value => Enum.Parse(typeof(WalletType), (string)db_value)
			};


			// Amount

			name = "Amount";
			Columns[name] = new Column(name, DBType.REAL);


			// CurrencyId

			name = "CurrencyId";
			Columns[name] = new Column(name, DBType.INTEGER);


			// Capture

			name = "Capture";
			Columns[name] = new Column(name, DBType.TEXT);


			// Backcolor

			name = "Backcolor";
			Columns[name] = new Column(name, DBType.TEXT)
			{

				ValueToDB = value =>
				{
					Color c = ((SolidColorBrush)value).Color;
					return $"{c.R};{c.G};{c.B}";
				},

				DBToValue = db_value =>
				{
					byte[] RGB = ((string)db_value).Split(';').Select((value) => Convert.ToByte(value)).ToArray();
					return new SolidColorBrush(Color.FromRgb(RGB[0], RGB[1], RGB[2]));
				}

			};


			// Icon

			name = "Icon";
			Columns[name] = new Column(name, DBType.BLOB)
			{
				ValueToDB = value => BitmapMaster.SourceToBytes((BitmapSource)Icon),
				DBToValue = db_value => BitmapMaster.BytesToSource((byte[])db_value)
			};


			// Addition

			name = "Addition";
			Columns[name] = new Column(name, DBType.TEXT);


			// GroupId

			name = "GroupId";
			Columns[name] = new Column(name, DBType.INTEGER);


			CheckTable();


			PreviewRemoved += WalletDB_PreviewRemoved;
		}


		public WalletDB(long id, params string[] columnsNames) : base("Wallets")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public WalletDB() : base("Wallets") => Initialize();
	}
}
