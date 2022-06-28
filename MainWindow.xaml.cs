using System;
using System.Windows.Controls;
using System.Windows;
using System.Linq;

using CashApp.Wallets;
using CashApp.Transactions;
using CashApp.Planned;
using CashApp.Statistics;
using CashApp.Currencies;
using CashApp.Categories;
using CashApp.Templates;

using CashApp.DataBase;
using CashApp.Instruments;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace CashApp
{
    public partial class MainWindow : Window
    {
        // FIELDS

        readonly SettingDB Theme;

        public WalletsBlock Wallets => XWallets;
        public TransactionsBlock Transactions => XTransactions;
        public StatisticBlock StatisticProfits => XProfits;
        public StatisticBlock StatisticLosses => XLosses;


        // METHODS

        void XCategories_Click(object sender, EventArgs e) => new CategoriesInform(this).ShowDialog();

        void XCurrencies_Click(object sender, EventArgs e) => new CurrenciesInform(this).ShowDialog();

        void XPlanned_Click(object sender, EventArgs e) => new PlannedInform().ShowDialog();

        void XTemplates_Click(object sender, EventArgs e) => new TemplatesInform().ShowDialog();

        void XTheme_Click(object sender, EventArgs e)
        {
            TextBlock themeText = (TextBlock)XTheme.Child;

            if (themeText.Text == "☼")
            {
                themeText.Text = "☾";
                Application.Current.Resources = new ResourceDictionary { Source = new Uri("pack://application:,,,/LightColors.xaml") };
                Theme.Value = "Light";
            }
            else
            {
                themeText.Text = "☼";
                Application.Current.Resources = new ResourceDictionary { Source = new Uri("pack://application:,,,/DarkColors.xaml") };
                Theme.Value = "Dark";
            }

            Theme.SaveChanges();
            Colors.SetColors(Application.Current.Resources);

            XWallets.InitializeMainGroup();
            XTransactions.DownloadTransactions();
            XProfits.DownloadStatistics();
            XLosses.DownloadStatistics();
        }


        SettingDB FindOrCreateSetting(Setting name, string defaultValue)
        {
            SettingDB setting = DBData.Settings.Find(s => s.Name == name);

            if (setting == null)
            {
                setting = new SettingDB { Name = name, Value = defaultValue };
                DBData.Settings.Add(setting);
            }

            return setting;
        }

        void InitializeBlocks()
        {
            XWallets.Window = this;
            XWallets.InitializeMainGroup();

            XTransactions.Window = this;
            XTransactions.DownloadTransactions();

            XProfits.Type = AmountType.Profit;
            XLosses.Type = AmountType.Loss;
            XProfits.DownloadStatistics();
            XLosses.DownloadStatistics();
        }
        

        // CONSTRUCTORS

        public MainWindow()
        {
            InitializeComponent();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseLeftButtonDown += (sender, e) => DragMove();


            XCategories.Click += XCategories_Click;
            XCurrencies.Click += XCurrencies_Click;
            XPlanned.Click += XPlanned_Click;
            XTemplates.Click += XTemplates_Click;
            XTheme.Click += XTheme_Click;


            Theme = FindOrCreateSetting(Setting.Theme, "Light");

            if (Theme.Value == "Light")
                Colors.SetColors(Application.Current.Resources);
            else
                XTheme_Click(XTheme, new EventArgs());


            InitializeBlocks();
        }
    }
}
