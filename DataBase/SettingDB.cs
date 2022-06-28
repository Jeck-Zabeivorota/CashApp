using System;

namespace CashApp.DataBase
{
	public enum Setting { Theme }

    public class SettingDB : DBClient
	{
		public Setting? Name
		{
			get => (Setting?)Columns["Name"].Value;
			set => ChangeColumnValue("Name", value);
		}

		public string Value
		{
			get => (string)Columns["Value"].Value;
			set => ChangeColumnValue("Value", value);
		}


		void Initialize()
		{
			string name = "Name";
			Columns[name] = new Column(name, DBType.TEXT)
			{
				ValueToDB = value => value.ToString(),
				DBToValue = db_value => Enum.Parse(typeof(Setting), (string)db_value)
			};

			name = "Value";
			Columns[name] = new Column(name, DBType.TEXT);


			CheckTable();
		}


		public SettingDB(long id, params string[] columnsNames) : base("Settings")
		{
			Initialize();
			DownloadData(id, columnsNames);
		}

		public SettingDB() : base("Settings") => Initialize();
	}
}
