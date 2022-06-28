using System;

namespace CashApp.DataBase
{
    enum DBType { TEXT, INTEGER, REAL, BOOLEAN, BLOB };

    class Column
    {
        // FIELDS

        public object Value = null;

        public object DBValue
        {
            get
            {
                if (ValueIsNull) throw new ArgumentNullException("\"Value\" is null");

                if (ValueToDB == null) return Value;
                return ValueToDB(Value);
            }
            set
            {
                if (DBToValue == null) Value = value;
                else Value = DBToValue(value);
            }
        }

        public Func<object, object> ValueToDB, DBToValue;

        public bool ValueIsNull => Value == null;


        public readonly string Name;

        public readonly DBType DBType;

        string Flags = " NOT NULL";

        public string ColumnFullName => $"{Name} {DBType} {Flags}";


        // METHODS

        public static Column CreatePrimaryKeyColumn()
        {
            return new Column("Id", DBType.INTEGER) { Flags = " PRIMARY KEY AUTOINCREMENT NOT NULL" };
        }


        // CONSTRUCTORS

        public Column(string name, DBType type)
        {
            Name = name;
            DBType = type;
        }
    }
}
