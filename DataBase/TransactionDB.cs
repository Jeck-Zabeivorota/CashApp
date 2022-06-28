using System;

namespace CashApp.DataBase
{
	public class TransactionDB : DBClient, Transactions.ITransactionItemData
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

		public string Description
		{
			get => (string)Columns["Description"].Value;
			set => ChangeColumnValue("Description", value);
		}


		static void TransactionDB_PreviewRemoved(object sender, EventArgs e)
        {
			TransactionDB transaction = (TransactionDB)sender;

			WalletDB wallet = DBData.Wallets.Find(w => w.Id == transaction.WalletId);

			int coef = transaction.Type == AmountType.Profit ? 1 : -1;
			wallet.Amount -= transaction.Amount * coef;

			wallet.SaveChanges();
        }

		static void TransactionDB_PreviewChanged(object sender, EventArgs e)
		{
			TransactionDB transaction = (TransactionDB)sender;

			if (!transaction.ChangedColumns.Contains("Amount") && !transaction.ChangedColumns.Contains("Type")) return;


			WalletDB wallet = DBData.Wallets.Find(w => w.Id == transaction.WalletId);

			TransactionDB oldTransaction = new TransactionDB((long)transaction.Id, "Type", "Amount");


			int coef = oldTransaction.Type == AmountType.Profit ? 1 : -1;
			wallet.Amount -= oldTransaction.Amount * coef;

			coef = transaction.Type == AmountType.Profit ? 1 : -1;
			wallet.Amount += transaction.Amount * coef;


			wallet.SaveChanges();
		}

		static void TransactionDB_Additing(object sender, EventArgs e)
		{
			TransactionDB transaction = (TransactionDB)sender;

			WalletDB wallet = DBData.Wallets.Find(w => w.Id == transaction.WalletId);
			
			int coef = transaction.Type == AmountType.Profit ? 1 : -1;
			wallet.Amount += transaction.Amount * coef;

			wallet.SaveChanges();
		}


		void Initialize()
		{
			// Type

			string name = "Type";

			Columns[name] = new Column(name, DBType.BOOLEAN)
			{
				ValueToDB = value => (AmountType)value == AmountType.Profit,
				DBToValue = db_value => (bool)db_value ? AmountType.Profit : AmountType.Loss
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


			// Description

			name = "Description";
			Columns[name] = new Column(name, DBType.TEXT);


			CheckTable();

			PreviewRemoved += TransactionDB_PreviewRemoved;
			PreviewChanged += TransactionDB_PreviewChanged;
			Additing += TransactionDB_Additing;
		}


		public TransactionDB(long id, params string[] columnsNames) : base("Transactions")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public TransactionDB() : base("Transactions") => Initialize();
	}
}
