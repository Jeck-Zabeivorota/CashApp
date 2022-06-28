using System;
using System.Linq;
using CashApp.Instruments;

namespace CashApp.DataBase
{
	public class PlannedDB : DBClient, Transactions.ITransactionItemData
	{
		public AmountType? Type
		{
			get => (AmountType?)Columns["Type"].Value;
			set => ChangeColumnValue("Type", value);
		}

		public long? WalletId
		{
			get => (long?)Columns["WalletId"].Value;
			set => ChangeColumnValue("WalletId", value);
		}

		public double? Amount
		{
			get => (double?)Columns["Amount"].Value;
			set => ChangeColumnValue("Amount", value);
		}

		public long? CategoryId
		{
			get => (long?)Columns["CategoryId"].Value;
			set => ChangeColumnValue("CategoryId", value);
		}

		public DateTime? Date
		{
			get => (DateTime?)Columns["Date"].Value;
			set => ChangeColumnValue("Date", value);
		}

		public DateSpan? Regularity
		{
			get => (DateSpan?)Columns["Regularity"].Value;
			set => ChangeColumnValue("Regularity", value);
		}


		public TransactionDB CreateTransaction()
        {
			return new TransactionDB
			{
				Type = Type,
				WalletId = WalletId,
				Amount = Amount,
				CategoryId = CategoryId,
				Date = Date
			};
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


			// WalletId

			name = "WalletId";
			Columns[name] = new Column(name, DBType.INTEGER);


			// Amount

			name = "Amount";
			Columns[name] = new Column(name, DBType.REAL);


			// CategoryId

			name = "CategoryId";
			Columns[name] = new Column(name, DBType.INTEGER);


			// Date

			name = "Date";
			Columns[name] = new Column(name, DBType.TEXT)
			{
				ValueToDB = value => ((DateTime)value).ToString("dd.MM.yyyy"),
				DBToValue = db_value => Convert.ToDateTime((string)db_value)
			};


			// Regularity

			name = "Regularity";
			Columns[name] = new Column(name, DBType.TEXT)
			{
				ValueToDB = value =>
				{
					DateSpan span = (DateSpan)value;
					return $"{span.Days};{span.Weeks};{span.Mounts}";
				},
				DBToValue = db_value =>
				{
					uint[] spans = ((string)db_value).Split(';').Select(s => Convert.ToUInt32(s)).ToArray();
					return new DateSpan(spans[0], spans[1], spans[2], 0);
				}
			};

			CheckTable();
		}


		public PlannedDB(long id, params string[] columnsNames) : base("Planned")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public PlannedDB() : base("Planned") => Initialize();
	}
}
