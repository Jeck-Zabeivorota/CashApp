using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Wallets
{
    public partial class WalletForm : Window
    {
        // CALSSES

        class ButgetFields
        {
            public readonly Field<NumberField> Budget, Days, Weeks, Mounts;
            public readonly Field<DateSelector> StartDate;

            public DateSpan DateSpan
            {
                get => new DateSpan((uint)Days.Input.Value, (uint)Weeks.Input.Value, (uint)Mounts.Input.Value, 0);
                set
                {
                    Days.Input.Value = value.Days;
                    Weeks.Input.Value = value.Weeks;
                    Mounts.Input.Value = value.Mounts;
                }
            }

            public readonly StackPanel Body = new StackPanel { Visibility = Visibility.Collapsed };


            public string Addition
            {
                get => $"{Budget.Input.Value};{StartDate.Input.Date:dd.MM.yyyy};{Days.Input.Value};{Weeks.Input.Value};{Mounts.Input.Value}";
                set
                {
                    string[] lines = value.Split(';');

                    Budget.Input.Value = double.Parse(lines[0]);
                    StartDate.Input.Date = DateTime.Parse(lines[1]);

                    Days.Input.Value = uint.Parse(lines[2]);
                    Weeks.Input.Value = uint.Parse(lines[3]);
                    Mounts.Input.Value = uint.Parse(lines[4]);
                }
            }


            public ButgetFields()
            {
                Budget = new Field<NumberField>("Бюджет:", new NumberField { Height = 25, Width = 100, Maximum = 1000000000 });

                StartDate = new Field<DateSelector>("Начало:", new DateSelector { Height = 25 });

                TextBlock capture = new TextBlock
                {
                    Text = "Период:",
                    Foreground = Colors.MainText,
                    Margin = new Thickness { Top = 8, Bottom = 8 }
                };

                Days = new Field<NumberField>("Дней:", new NumberField { Height = 25, Width = 70, Maximum = 1000 });
                Days.Body.Margin = new Thickness { Left = 20 };

                Weeks = new Field<NumberField>("Недель:", new NumberField { Height = 25, Width = 70, Maximum = 1000 });
                Weeks.Body.Margin = new Thickness { Left = 20 };

                Mounts = new Field<NumberField>("Месяцев:", new NumberField { Height = 25, Width = 70, Maximum = 1000 });
                Mounts.Body.Margin = new Thickness { Left = 20 };


                Body.Children.Add(Budget.Body);
                Body.Children.Add(StartDate.Body);
                Body.Children.Add(capture);
                Body.Children.Add(Days.Body);
                Body.Children.Add(Weeks.Body);
                Body.Children.Add(Mounts.Body);
            }
        }


        // FIELDS

        const string SIMPLE_WALLET = "Простой",
                     LIMIT_WALLET = "Лимитный",
                     BUTGET_WALLET = "Бюджет",
                     GROUP_WALLET = "Группа";
        const string MAIN = "<Главная>";

        Field<TextBlock> XGroup;
        Field<DropButton> XType, XCurrency;
        Field<TextField> XCapture;
        Field<NumberField> XAmount, XLimit;
        Field<ColorPicker> XBackcolor;
        Field<ImageSelector> XIcon;
        ButgetFields Butget;

        public WalletDB WalletData;
        bool Done = false;

        IWallet XWalletMaket
        {
            get => (IWallet)XWalletContainer.Child;
            set => XWalletContainer.Child = (UIElement)value;
        }


        // METHODS

        void SetWalletData(WalletDB data)
        {
            switch (XType.Input.SelectItem)
            {
                case SIMPLE_WALLET:
                    data.Type = WalletType.Simple;
                    data.Addition = " ";
                    break;

                case LIMIT_WALLET:
                    data.Type = WalletType.Limit;
                    data.Addition = XLimit.Input.Text;
                    break;

                case BUTGET_WALLET:
                    data.Type = WalletType.Budget;
                    data.Addition = Butget.Addition;
                    break;

                case GROUP_WALLET:
                    data.Type = WalletType.Group;
                    data.Addition = " ";
                    break;
            }


            data.Amount = XAmount.Input.Value;

            if (XCurrency.Input.SelectItem != null && XType.Input.SelectItem != GROUP_WALLET)
                data.CurrencyId = DBData.Currencies.Find(c => c.Currency == XCurrency.Input.SelectItem).Id;
            else
                data.CurrencyId = 0;

            data.Capture = XCapture.Input.Text;

            data.Backcolor = (SolidColorBrush)XBackcolor.Input.Background;

            data.Icon = XIcon.Input.Source;
        }

        void XType_SelectionChanged(object sender, EventArgs e)
        {
            WalletDB data = new WalletDB();
            SetWalletData(data);
            
            XAmount.Body.Visibility = Visibility.Visible;
            XCurrency.Body.Visibility = Visibility.Visible;
            XLimit.Body.Visibility = Visibility.Collapsed;
            Butget.Body.Visibility = Visibility.Collapsed;


            switch (data.Type)
            {
                case WalletType.Limit:
                    XLimit.Body.Visibility = Visibility.Visible;
                    break;

                case WalletType.Budget:
                    Butget.Body.Visibility = Visibility.Visible;
                    break;

                case WalletType.Group:
                    XAmount.Body.Visibility = Visibility.Collapsed;
                    XCurrency.Body.Visibility = Visibility.Collapsed;
                    break;
            }

            XWalletMaket = data.CreateWallet();
        }

        void XDone_Click(object sender, EventArgs e)
        {
            if (XCapture.Input.Text == null | (XCurrency.Input.Visibility == Visibility.Visible & XCurrency.Input.SelectItem == null) | XIcon.Input.Source == null)
            {
                MsgBox.Show("Не все поля заполнены!", "Кошелек", MsgIcon.Error, "Ok");
                return;
            }


            SetWalletData(WalletData);

            if (WalletData.Id == null)
                DBData.Wallets.Add(WalletData);
            else
                WalletData.SaveChanges();


            Done = true;
            Close();
        }


        void BuildFields()
        {
            // GROUP

            XGroup = new Field<TextBlock>("Группа:", new TextBlock { Foreground = Colors.SecondText1 });


            // TYPE
            
            XType = new Field<DropButton>("Тип:", new DropButton { Height = 20, Width = 100 });

            string[] items = { SIMPLE_WALLET, LIMIT_WALLET, BUTGET_WALLET, GROUP_WALLET };
            XType.Input.Items.AddRange( items.Select(item => new TextBlock { Text = item, Foreground = Colors.MainText }) );

            XType.Input.SelectionChanged += XType_SelectionChanged;


            // CAPTURE

            XCapture = new Field<TextField>("Название:", new TextField { Height = 20, Width = 120 });
            XCapture.Input.TextChanged += (sender, e) => XWalletMaket.Capture = XCapture.Input.Text;


            // AMOUNT

            XAmount = new Field<NumberField>("Баланс:", new NumberField { Height = 20, Width = 100, Maximum = 1000000000 });
            XAmount.Input.ValueChanged += (sender, e) => XWalletMaket.Amount = XAmount.Input.Value;


            // CURRENCY
            
            XCurrency = new Field<DropButton>("Валюта:", new DropButton { Height = 20, Width = 100 });
            
            XCurrency.Input.Items.AddRange(DBData.Currencies.Select(c => new TextBlock { Text = c.Currency, Foreground = Colors.MainText }));
            
            XCurrency.Input.SelectionChanged +=
                (sender, e) => XWalletMaket.CurrencyId = (long)DBData.Currencies.Find(c => c.Currency == XCurrency.Input.SelectItem).Id;
            

            // BACKCOLOR

            XBackcolor = new Field<ColorPicker>("Цвет:", new ColorPicker { Height = 20, Width = 20 });
            XBackcolor.Input.ColorChanged += (sender, e) => XWalletMaket.Backcolor = (SolidColorBrush)XBackcolor.Input.Background;


            // ICON

            XIcon = new Field<ImageSelector>("Иконка:", new ImageSelector { Height = 20, Width = 20 });
            XIcon.Input.ImageChanged += (sender, e) => XWalletMaket.Icon = XIcon.Input.Source;


            //// ADDITION \\\\


            // LIMIT

            XLimit = new Field<NumberField>("Лимит:", new NumberField { Height = 20, Width = 100, Maximum = 1000000000 });
            XLimit.Body.Visibility = Visibility.Collapsed;
            XLimit.Input.ValueChanged += (sender, e) => ((LimitWallet)XWalletMaket).Limit = XLimit.Input.Value;


            // REGULARITY

            Butget = new ButgetFields();

            Butget.Budget.Input.ValueChanged +=
                (sender, e) => ((BudgetWallet)XWalletMaket).Budget = Butget.Budget.Input.Value;

            Butget.StartDate.Input.SelectionChanged +=
                (sender, e) => ((BudgetWallet)XWalletMaket).StartDate = Butget.StartDate.Input.Date;

            void changeSpan(object sender, EventArgs e) => ((BudgetWallet)XWalletMaket).Period = Butget.DateSpan;

            Butget.Days.Input.ValueChanged += changeSpan;
            Butget.Weeks.Input.ValueChanged += changeSpan;
            Butget.Mounts.Input.ValueChanged += changeSpan;


            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(XGroup.Body);
            properties.Children.Add(XType.Body);
            properties.Children.Add(XCapture.Body);
            properties.Children.Add(XAmount.Body);
            properties.Children.Add(XCurrency.Body);
            properties.Children.Add(XBackcolor.Body);
            properties.Children.Add(XIcon.Body);
            properties.Children.Add(XLimit.Body);
            properties.Children.Add(Butget.Body);
        }


        public bool ShowDialog_Create(long groupId)
        {
            WalletData = new WalletDB { GroupId = groupId };

            XWalletMaket = new SimpleWallet();

            XGroup.Input.Text = groupId == 0 ? MAIN : DBData.Wallets.Find(w => w.Id == groupId).Capture;

            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Change(WalletDB data)
        {
            if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");


            WalletData = data;
            XWalletMaket = data.CreateWallet();

            XType.Input.IsEnabled = false;

            switch (data.Type)
            {
                case WalletType.Simple:
                    XType.Input.SelectItem = SIMPLE_WALLET;
                    break;

                case WalletType.Limit:
                    XType.Input.SelectItem = LIMIT_WALLET;
                    XLimit.Input.Value = double.Parse(data.Addition);
                    break;

                case WalletType.Budget:
                    XType.Input.SelectItem = BUTGET_WALLET;
                    Butget.Addition = data.Addition;
                    break;

                case WalletType.Group:
                    XType.Input.SelectItem = GROUP_WALLET;
                    break;
            }


            XGroup.Input.Text = data.GroupId == 0 ? MAIN : DBData.Wallets.Find(w => w.Id == data.GroupId).Capture;


            if (data.CurrencyId != 0)
            {
                XAmount.Input.Value = (double)data.Amount;
                XCurrency.Input.SelectItem = DBData.Currencies.Find(c => c.Id == data.CurrencyId).Currency;
            }

            XCapture.Input.Text = data.Capture;

            XBackcolor.Input.Background = data.Backcolor;

            XIcon.Input.Source = data.Icon;


            ((TextBlock)XDone.Child).Text = "Сохранить";

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public WalletForm()
        {
            InitializeComponent();

            BuildFields();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseDown += (sender, e) => DragMove();

            XCancel.Click += (sender, e) => Close();
            XDone.Click += XDone_Click;
        }
    }
}
