using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace CashApp.DataBase
{
	public enum AmountType { Loss, Profit }

	public abstract class DBClient
	{
		// FIELDS

		public readonly string Table;

		public readonly List<string> ChangedColumns = new List<string>();

		private protected readonly Dictionary<string, Column> Columns = new Dictionary<string, Column>();


		public long? Id
		{
			get => (long?)Columns["Id"].Value;
			set => Columns["Id"].Value = value;
		}


		// EVENTS

		public event EventHandler PreviewRemoved;
		public event EventHandler PreviewChanged;
		public event EventHandler PreviewAdditing;

		public event EventHandler Removed;
		public event EventHandler Changed;
		public event EventHandler Additing;


		// METODS

		static string CreateParams(List<string> columnsNames, string separator = ", ")
		{
			return string.Join(separator, columnsNames.Select(name => $"{name} = @{name}"));
		}

		static List<string> SelectNotNullColumnsNames(DBClient client)
        {
			return (from kv in client.Columns where !kv.Value.ValueIsNull select kv.Key).ToList();
		}


		public static int Count(string table)
		{
			using (DBConnector db = new DBConnector())
            {
				string command = $"SELECT COUNT(*) FROM {table};";
				return (int)db.ExecuteRequest(command, RequestResult.Scalar);
            }
		}

		public static bool Exists(DBClient parametersSource)
		{
			List<string> names = SelectNotNullColumnsNames(parametersSource);

			if (names.Count == 0) throw new ArgumentNullException("All columns values is null");


			using (DBConnector db = new DBConnector())
            {
				string command = $"SELECT EXISTS(SELECT Id FROM {parametersSource.Table} WHERE {CreateParams(names, " AND ")});";

				return (long)db.ExecuteRequest(command, RequestResult.Scalar, parametersSource.Columns) > 0;
            }
		}

		public static void Remove<Tclient>(Tclient parametersSource) where Tclient : DBClient, new()
		{
			Tclient[] clients = new Tclient[0];

			if (parametersSource.PreviewRemoved != null)
            {
				clients = GetClients(parametersSource);

				foreach (DBClient client in clients)
					client.PreviewRemoved.Invoke(client, new EventArgs());
			}



			string command = $"DELETE FROM {parametersSource.Table}";

			List<string> columnsNames = SelectNotNullColumnsNames(parametersSource);

			if (columnsNames.Count > 0)
				command += $" WHERE {CreateParams(columnsNames, " AND ")}";

			command += ";";


			using (DBConnector db = new DBConnector())
				db.ExecuteRequest(command, RequestResult.None, parametersSource.Columns);



			if (parametersSource.Removed != null)
            {
				if (clients.Length == 0) clients = GetClients(parametersSource);

				foreach (DBClient client in clients)
					client.Removed.Invoke(client, new EventArgs());
			}
		}

		public static T[] Select<T>(DBClient parametersSource, string columnName, bool unique = false)
		{
			string command = $"SELECT {(!unique ? null : "DISTINCT ")} {columnName} FROM {parametersSource.Table}";

			List<string> columnsNames = SelectNotNullColumnsNames(parametersSource);

			if (columnsNames.Count > 0)
				command += $" WHERE {CreateParams(columnsNames, " AND ")}";

			command += ";";


			DBConnector db = new DBConnector();
			SQLiteDataReader reader = (SQLiteDataReader)db.ExecuteRequest(command, RequestResult.Reader, parametersSource.Columns);


			List<T> values = new List<T>();

			Func<object, object> convereter = parametersSource.Columns[columnName].DBToValue;

			if (convereter == null)
				while (reader.Read())
					values.Add((T)reader[columnName]);
			else
				while (reader.Read())
					values.Add((T)convereter(reader[columnName]));

			reader.Close();
			db.Dispose();


			return values.ToArray();
		}

		public static Tclient[] GetClients<Tclient>(Tclient parametersSource, params string[] columnsNames) where Tclient : DBClient, new()
		{
			long[] ids = Select<long>(parametersSource, "Id");

			Tclient[] clients = new Tclient[ids.Length];

			for (long i = 0; i < ids.Length; i++)
            {
				clients[i] = new Tclient();
				clients[i].DownloadData(ids[i], columnsNames);
            }

			return clients;
		}


		protected void ChangeColumnValue(string columnName, object value)
		{
			Columns[columnName].Value = value;

			if (!ChangedColumns.Contains(columnName)) ChangedColumns.Add(columnName);
		}

		public void CheckTable()
		{
			string[] columnsFullNames = Columns.Values.Select(column => column.ColumnFullName).ToArray();

			using (DBConnector db = new DBConnector())
				db.ExecuteRequest($"CREATE TABLE IF NOT EXISTS {Table} ({string.Join(", ", columnsFullNames)});");
		}

		public void Add()
		{
			PreviewAdditing?.Invoke(this, new EventArgs());

			ChangedColumns.Clear();


			List<string> columnsNames = SelectNotNullColumnsNames(this);
			columnsNames.Remove("Id");

			if (columnsNames.Count < Columns.Count - 1) throw new InvalidOperationException("All columns must have values");

			string command = $"INSERT INTO {Table}({string.Join(", ", columnsNames)}) VALUES(@{string.Join(", @", columnsNames)}); SELECT last_insert_rowid();";

			using (DBConnector db = new DBConnector())
				Id = (long)db.ExecuteRequest(command, RequestResult.Scalar, Columns);


			Additing?.Invoke(this, new EventArgs());
		}

		public void Remove()
        {
			PreviewRemoved?.Invoke(this, new EventArgs());

			if (Id == null) throw new NullReferenceException("\"Id\" is null");

			using (DBConnector db = new DBConnector())
				db.ExecuteRequest($"DELETE FROM {Table} WHERE Id = {Id};");


			Removed?.Invoke(this, new EventArgs());
		}

		public void SaveChanges()
		{
			if (ChangedColumns.Count == 0) return;

			PreviewChanged?.Invoke(this, new EventArgs());


			string command = $"UPDATE {Table} SET {CreateParams(ChangedColumns)} WHERE Id = {Id};";

			using (DBConnector db = new DBConnector())
				db.ExecuteRequest(command, RequestResult.None, Columns);


			Changed?.Invoke(this, new EventArgs());

			ChangedColumns.Clear();
		}

		public void DownloadData(long id, params string[] columnsNames)
		{
			Id = id;

			string command = "SELECT ";

			if (columnsNames.Length == 0)
			{
				command += "*";
				columnsNames = Columns.Keys.ToArray();
			}
			else
				command += string.Join(", ", columnsNames);

			command += $" FROM {Table} WHERE Id = {id};";


			using (DBConnector db = new DBConnector())
            {
				var reader = (SQLiteDataReader)db.ExecuteRequest(command, RequestResult.Reader);

				reader.Read();
				foreach (string name in columnsNames)
					Columns[name].DBValue = reader[name];

				reader.Close();
			}
		}

		public void DownloadData(params string[] columnsNames)
		{
			if (Id == null) throw new ArgumentNullException("\"Id\" is null");
			DownloadData((long)Id, columnsNames);
		}

		public void Clear()
		{
			ChangedColumns.Clear();

			foreach (string columnName in Columns.Keys)
				Columns[columnName].Value = null;
		}


        // CONSTRUCTORS

        public DBClient(string table)
		{
			Table = table;

			Columns["Id"] = Column.CreatePrimaryKeyColumn();

			if (!File.Exists("Data.db")) SQLiteConnection.CreateFile("Data.db");
		}
	}
}
