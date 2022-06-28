using System;
using System.Linq;
using System.Windows.Media;

namespace CashApp.DataBase
{
	public class CategoryDB : DBClient
	{
		public AmountType? Type
		{
			get => (AmountType?)Columns["Type"].Value;
			set => ChangeColumnValue("Type", value);
		}

		public string Name
        {
			get => (string)Columns["Name"].Value;
			set => ChangeColumnValue("Name", value);
		}

		public SolidColorBrush Backcolor
		{
			get => (SolidColorBrush)Columns["Backcolor"].Value;
			set => ChangeColumnValue("Backcolor", value);
		}

		public long? WalletId
		{
			get => (long?)Columns["WalletId"].Value;
			set => ChangeColumnValue("WalletId", value);
		}


		static void CategoryDB_PreviewRemoved(object sender, EventArgs e)
		{
			CategoryDB category = (CategoryDB)sender;

			var transactions = DBData.Transactions.Where(t => t.CategoryId == category.Id);

			foreach (TransactionDB transaction in transactions)
			{
				transaction.CategoryId = 0;
				transaction.SaveChanges();
			}
		}


		void Initialize()
		{
			// Type

			string name = "Type";

			Columns[name] = new Column(name, DBType.BOOLEAN)
			{
				ValueToDB = (value) => (AmountType)value == AmountType.Profit,
				DBToValue = (db_value) => (bool)db_value ? AmountType.Profit : AmountType.Loss
			};


			// Name

			name = "Name";
			Columns[name] = new Column(name, DBType.TEXT);


			// Backcolor

			name = "Backcolor";

			Columns[name] = new Column(name, DBType.TEXT)
			{

				ValueToDB = value =>
				{
					Color c = (value as SolidColorBrush).Color;
					return $"{c.R};{c.G};{c.B}";
				},

				DBToValue = db_value =>
				{
					byte[] RGB = ((string)db_value).Split(';').Select(value => Convert.ToByte(value)).ToArray();
					return new SolidColorBrush(Color.FromRgb(RGB[0], RGB[1], RGB[2]));
				}

			};


			// WalletId

			name = "WalletId";
			Columns[name] = new Column(name, DBType.INTEGER);


			CheckTable();

			PreviewRemoved += CategoryDB_PreviewRemoved;
		}


		public CategoryDB(long id, params string[] columnsNames) : base("Categories")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public CategoryDB() : base("Categories") => Initialize();
	}
}
