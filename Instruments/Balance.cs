using System;
using System.Text;

namespace CashApp.Instruments
{
	public class Balance
	{
		bool isUpdate = false;


		string _currency;
		public string Currency
		{
			get => _currency;
			set
			{
				if (value != null)
				{
					_currency = value.Replace(" ", null).Trim();

					if (_currency.Length > 5) throw new FormatException("\"Currency\" contains more than 5 characters");
				}

				if (isUpdate)
				{
					int separatorIndex = _string.LastIndexOf(' ');
					_string = _string.Substring(0, separatorIndex + 1) + _currency;
				}
			}
		}


		double _amount = 0;
		public double Amount
		{
			get => _amount;
			set
			{
				_amount = Math.Round(value, 2);
				isUpdate = false;
			}
		}


		string _string;
		public override string ToString()
		{
			if (isUpdate) return _string;


			StringBuilder strAmount = new StringBuilder(_amount.ToString());

			int dotIdx = strAmount.ToString().LastIndexOf(',');

			if (dotIdx == -1)
			{
				dotIdx = strAmount.Length;
				strAmount.Append(",00");
			}


			// Добавление разделителей: 1000000,00 -> 1 000 000,00

			int firstIdx = strAmount[0] == '-' ? 1 : 0, lastIdx = dotIdx - 3;

			for (int i = lastIdx; i > firstIdx; i -= 3)
				strAmount.Insert(i, ' ');


			_string = $"{strAmount} {Currency}";

			isUpdate = true;
			return _string;
		}


		public Balance(double amount, string currency)
		{
			Amount = amount;
			Currency = currency;
		}

		public Balance() { }
	}
}
