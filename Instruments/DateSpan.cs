using System;
using System.Text;

namespace CashApp.Instruments
{
	public struct DateSpan
	{
		// FIELDS

		public uint Days, Weeks, Mounts, Years;


		// METHODS

		public static DateTime operator +(DateTime date, DateSpan span)
        {
			int year = (int)(date.Year + span.Years + (date.Month + span.Mounts) / 12);
			int mount = (int)((date.Month + span.Mounts) % 12);
			int day;

			int daysInMount = DateTime.DaysInMonth(year, mount);

			if (date.Day <= daysInMount) day = date.Day;
			else day = daysInMount;

			return new DateTime(year, mount, day) + new TimeSpan((int)(span.Days + span.Weeks * 7), 0, 0, 0);
		}

		public static DateTime operator +(DateSpan span, DateTime date) => date + span;

		public static DateSpan operator +(DateSpan span1, DateSpan span2)
		{
			return new DateSpan(span1.Days + span2.Days, span1.Weeks + span2.Weeks, span1.Mounts + span2.Mounts, span1.Years + span2.Years);
		}


		public static DateTime operator -(DateTime date, DateSpan span)
		{
			int years = (int)(date.Year - (span.Years + span.Mounts / 12));
			int mount = (int)(date.Month - span.Mounts % 12);

			if (mount < 0)
            {
				years--;
				mount += 12;
            }

			return new DateTime(years, mount, date.Day) + new TimeSpan((int)(span.Days + span.Weeks * 7), 0, 0, 0);
		}

		public static DateSpan operator -(DateSpan span1, DateSpan span2)
		{
			return new DateSpan(span1.Days - span2.Days, span1.Weeks - span2.Weeks, span1.Mounts - span2.Mounts, span1.Years - span2.Years);
		}


		public static DateSpan GetSpan(DateTime date1, DateTime date2)
        {
			int years = date1.Year - date2.Year;
			int mounts = date1.Month - date2.Month;

			if (mounts < 0)
			{
				years--;
				mounts += 12;
			}

			
			int days = date1.Day - date2.Day;

			if (days < 0)
			{
				mounts--;

				if (mounts < 0) { years--; mounts += 12; }

				days += DateTime.DaysInMonth(years, mounts);
			}


			return new DateSpan((uint)(days % 7), (uint)(days / 7), (uint)mounts, (uint)years);
        }

		public bool IsNullSpan()
        {
			return Days == 0 && Weeks == 0 && Mounts == 0 && Years == 0;
		}

        public override string ToString()
        {
			if (IsNullSpan()) return null;

			StringBuilder str = new StringBuilder();

			if (Days != 0) str.Append($"{Days} дней");
			if (Weeks != 0) str.Append($" {Weeks} недель");
			if (Mounts != 0) str.Append($" {Mounts} месяцев");
			if (Years != 0) str.Append($" {Years} год");

			return str.ToString();
        }


        // CONSTRUCTORS

        public DateSpan(uint days, uint weeks, uint mounts, uint years)
		{
			Days = days;
			Weeks = weeks;
			Mounts = mounts;
			Years = years;
		}
	}
}
