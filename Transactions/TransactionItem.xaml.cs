using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.Instruments;

namespace CashApp.Transactions
{
    public partial class TransactionItem : Grid
    {
        // FIELDS

        long WalletId, CategoryId;

        double Amount;


        // METHODS

        public void SetData(ITransactionItemData data)
        {
            WalletId = (long)data.WalletId;
            CategoryId = (long)data.CategoryId;
            Amount = (double)data.Amount;

            WalletDB wallet = DBData.Wallets.Find(w => w.Id == WalletId);
            CurrencyDB currency = DBData.Currencies.Find(c => c.Id == wallet.CurrencyId);


            XIcon.Source = wallet.Icon;

            XWallet.Text = wallet.Capture;

            Balance balance = new Balance(Amount, currency.Currency);

            if (data.Type == AmountType.Profit)
            {
                XAmount.Foreground = Brushes.LimeGreen;
                XAmount.Text = $"+{balance}";
                
            }
            else
            {
                XAmount.Foreground = Brushes.Tomato;
                XAmount.Text = $"-{balance}";
            }


            if (CategoryId != 0)
            {
                CategoryDB category = DBData.Categories.Find(c => c.Id == CategoryId);

                XCategory.Background = category.Backcolor;
                (XCategory.Child as TextBlock).Text = category.Name;

                XCategory.Visibility = Visibility.Visible;
            }
            else
                XCategory.Visibility = Visibility.Collapsed;


            if (data is TransactionDB transaction)
                XDate.Text = transaction.Date.Value.ToString("dd.MM.yyyy");

            else if (data is PlannedDB planned)
            {
                bool once = planned.Regularity.Value.IsNullSpan();
                XDate.Text = $"{(once ? "(Один раз) " : null)}Через {(planned.Date.Value - DateTime.Now).Days + 1} дней";
            }
        }

        public void GetData(ITransactionItemData data)
        {
            data.WalletId = WalletId;
            data.CategoryId = CategoryId;
            data.Type = XAmount.Text[0] == '+' ? AmountType.Profit : AmountType.Loss;
            data.Amount = Amount;
        }

        
        // CONSTRUCTORS

        public TransactionItem(ITransactionItemData data)
        {
            InitializeComponent();
            SetData(data);
        }
    }
}
