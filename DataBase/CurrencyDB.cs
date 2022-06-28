using System;
using System.Linq;

namespace CashApp.DataBase
{
	public class CurrencyDB : DBClient
	{
		public string Currency
		{
			get => (string)Columns["Currency"].Value;
			set
			{
				if (value.Length > 5)
					throw new FormatException($"\"{value}\" contains more than 5 characters");

				ChangeColumnValue("Currency", value);
			}
		}

		public string Name
		{
			get => (string)Columns["Name"].Value;
			set => ChangeColumnValue("Name", value);
		}


		static void CurrencyDB_PreviewRemoved(object sender, EventArgs e)
		{
			CurrencyDB currency = (CurrencyDB)sender;

			var wallets = DBData.Wallets.Where(w => w.CurrencyId == currency.Id).ToArray();

			foreach (WalletDB wallet in wallets)
					DBData.Wallets.Remove(wallet);
		}

		void Initialize()
		{
			// Currency

			string name = "Currency";
			Columns[name] = new Column(name, DBType.TEXT);


			// Name

			name = "Name";
			Columns[name] = new Column(name, DBType.TEXT);


			CheckTable();


			PreviewRemoved += CurrencyDB_PreviewRemoved;
		}


		public CurrencyDB(long id, params string[] columnsNames) : base("Currencies")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public CurrencyDB() : base("Currencies") => Initialize();
	}
}
