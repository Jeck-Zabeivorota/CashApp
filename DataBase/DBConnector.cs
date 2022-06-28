using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace CashApp.DataBase
{
	enum RequestResult { None, Scalar, Reader }

	class DBConnector : IDisposable
    {
        readonly SQLiteConnection Connection;
        readonly SQLiteCommand Command;


		public object ExecuteRequest(string request, RequestResult resultType = RequestResult.None, Dictionary<string, Column> dataSource = null)
		{
			Command.CommandText = request;


			if (dataSource != null)
			{
				foreach (string columnName in dataSource.Keys)
					if (!dataSource[columnName].ValueIsNull && request.IndexOf($"@{columnName}") != -1)
						Command.Parameters.AddWithValue($"@{columnName}", dataSource[columnName].DBValue);
			}


			object result = null;

            switch (resultType)
            {
				case RequestResult.Scalar:
					result = Command.ExecuteScalar();
					break;

				case RequestResult.Reader:
					result = Command.ExecuteReader();
					break;

				default:
					Command.ExecuteNonQuery();
					break;
			}

			return result;
		}

		public void Dispose() => Connection.Clone();


        public DBConnector()
        {
            Connection = new SQLiteConnection("Data Source = Data.db; Version=3; New=False; Compress=True;");
            Connection.Open();

            Command = Connection.CreateCommand();
        }
    }
}
