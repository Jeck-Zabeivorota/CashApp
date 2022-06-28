using System;
using System.Windows.Media;

using CashApp.DataBase;

namespace CashApp.Wallets
{
    interface IWallet
    {
        double Amount { get; set; }
        long CurrencyId { get; set; }
        string Capture { get; set; }
        ImageSource Icon { get; set; }
        Brush Backcolor { get; set; }

        void SetData(WalletDB data);
        void GetData(WalletDB data);

        event EventHandler Click;
    }
}
