using CashApp.DataBase;

namespace CashApp.Transactions
{
    public interface ITransactionItemData
    {
        AmountType? Type { get; set; }
        long? WalletId { get; set; }
        double? Amount { get; set; }
        long? CategoryId { get; set; }
    }
}
