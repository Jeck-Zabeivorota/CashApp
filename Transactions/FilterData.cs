using System;

using CashApp.DataBase;

namespace CashApp.Transactions
{
    public class FilterData
	{
		public DateTime? StartDate, EndDate;
		public AmountType? Type;
		public long? CategoryId;

		public Func<TransactionDB, bool> GetPredicate()
		{
			if ((StartDate == null || EndDate == null) && Type == null && CategoryId == null) return null;

			return t =>
			{
				bool isSuits = true;

				if (StartDate != null && EndDate != null)
					if (t.Date > StartDate.Value.Date || t.Date < EndDate.Value.Date)
						isSuits = false;

				if (Type != null && t.Type != Type)
					isSuits = false;

				if (CategoryId != null && t.CategoryId != CategoryId)
					isSuits = false;

				return isSuits;
			};
		}
	}
}
