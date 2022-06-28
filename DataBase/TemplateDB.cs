namespace CashApp.DataBase
{
    public class TemplateDB : DBClient
	{
		public string Name
		{
			get => (string)Columns["Name"].Value;
			set => ChangeColumnValue("Name", value);
		}

		public AmountType? Type
		{
			get => (AmountType?)Columns["Type"].Value;
			set => ChangeColumnValue("Type", value);
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

		public long? WalletId
		{
			get => (long?)Columns["WalletId"].Value;
			set => ChangeColumnValue("WalletId", value);
		}


		void Initialize()
		{
			// Name

			string name = "Name";
			Columns[name] = new Column(name, DBType.TEXT);


			// Type

			name = "Type";

			Columns[name] = new Column(name, DBType.BOOLEAN)
			{
				ValueToDB = (value) => (AmountType)value == AmountType.Profit ? true : false,
				DBToValue = (db_value) => (bool)db_value ? AmountType.Profit : AmountType.Loss
			};


			// Amount

			name = "Amount";
			Columns[name] = new Column(name, DBType.REAL);


			// CategoryId

			name = "CategoryId";
			Columns[name] = new Column(name, DBType.INTEGER);


			// WalletId

			name = "WalletId";
			Columns[name] = new Column(name, DBType.INTEGER);


			CheckTable();
		}


		public TemplateDB(long id, params string[] columnsNames) : base("Templates")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public TemplateDB() : base("Templates") => Initialize();
	}
}
