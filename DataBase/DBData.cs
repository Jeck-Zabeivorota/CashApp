using System;
using System.Linq;

using CashApp.Instruments;

namespace CashApp.DataBase
{
    public static class DBData
    {
        public static TrackList<CurrencyDB> Currencies = new TrackList<CurrencyDB>();

        public static TrackList<CategoryDB> Categories = new TrackList<CategoryDB>();

        public static TrackList<WalletDB> Wallets = new TrackList<WalletDB>();

        public static TrackList<TransactionDB> Transactions = new TrackList<TransactionDB>();

        public static TrackList<PlannedDB> Planned = new TrackList<PlannedDB>();

        public static TrackList<TemplateDB> Templates = new TrackList<TemplateDB>();

        public static TrackList<SettingDB> Settings = new TrackList<SettingDB>();


        static void Clients_PreviewRemovedItems(object sender, DBClient[] items, int startIndex)
        {
            foreach (DBClient item in items) item.Remove();
        }

        static void Clients_AddingItems(object sender, DBClient[] items, int startIndex)
        {
            foreach (DBClient item in items) item.Add();
        }

        static void CheckPlanned()
        {
            PlannedDB[] planned = Sorter.Sort(Planned, (item1, item2) => item2.Date < item1.Date ? item2 : item1).ToArray();

            foreach (PlannedDB plan in planned)
                if (plan.Date <= DateTime.Now)
                {
                    if (plan.Regularity.Value.IsNullSpan())
                    {
                        Transactions.Add(plan.CreateTransaction());
                        Planned.Remove(plan);
                        continue;
                    }

                    do
                    {
                        Transactions.Add(plan.CreateTransaction());
                        plan.Date += plan.Regularity.Value;
                    }
                    while (plan.Date <= DateTime.Now);

                    plan.SaveChanges();
                }
                else break;
        }


        static DBData()
        {
            Currencies.AddRange(DBClient.GetClients(new CurrencyDB()));
            Categories.AddRange(DBClient.GetClients(new CategoryDB()));
            Wallets.AddRange(DBClient.GetClients(new WalletDB()));
            Transactions.AddRange(DBClient.GetClients(new TransactionDB()));
            Planned.AddRange(DBClient.GetClients(new PlannedDB()));
            Templates.AddRange(DBClient.GetClients(new TemplateDB()));
            Settings.AddRange(DBClient.GetClients(new SettingDB()));

            Currencies.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Categories.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Wallets.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Transactions.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Planned.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Templates.PreviewRemovedItems += Clients_PreviewRemovedItems;
            Settings.PreviewRemovedItems += Clients_PreviewRemovedItems;

            Currencies.AddingItems += Clients_AddingItems;
            Categories.AddingItems += Clients_AddingItems;
            Wallets.AddingItems += Clients_AddingItems;
            Transactions.AddingItems += Clients_AddingItems;
            Planned.AddingItems += Clients_AddingItems;
            Templates.AddingItems += Clients_AddingItems;
            Settings.AddingItems += Clients_AddingItems;

            CheckPlanned();
        }
    }
}
