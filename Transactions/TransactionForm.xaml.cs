using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Templates;
using CashApp.Instruments;

namespace CashApp.Transactions
{
    public partial class TransactionForm : Window
    {
        // FIELDS

        readonly TextBlock PROFIT, LOSS;
        const string NONE = "<Ничего>";

        Field<TextBlock> XWallet;
        Field<ChoiceChips> XType;
        Field<DropButton> XCategory;
        Field<NumberField> XAmount;
        Field<DateSelector> XDate;
        Field<MultilineTextBox> XDescription;

        List<CategoryDB> Categories;
        readonly DropMenu TemplatesMenu = new DropMenu { Width = 180 };

        public TransactionDB TransactionData;
        bool Done = false;


        // METHODS

        void SetCategories()
        {
            AmountType type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

            Categories = DBData.Categories.Where(c => c.Type == type && (c.WalletId == 0 || c.WalletId == TransactionData.WalletId)).ToList();

            XCategory.Input.Items.Clear();
            XCategory.Input.Items.Add(new TextBlock { Text = NONE, Foreground = Colors.MainText });
            XCategory.Input.Items.AddRange(Categories.Select(c => new TextBlock { Text = c.Name, Foreground = Colors.MainText }).ToArray());
        }

        void TemplatesMenu_SelectionChanged(object sender, EventArgs e)
        {
            Grid grid = (Grid)sender;

            string templateName = ((TextBlock)grid.Children[0]).Text;
            AmountType type = ((TextBlock)grid.Children[1]).Text[0] == '+' ? AmountType.Profit : AmountType.Loss;

            TemplateDB template = DBData.Templates.Find(t => t.Name == templateName && t.Type == type);

            SetFields(new TransactionDB
            {
                Type = template.Type,
                Amount = template.Amount,
                CategoryId = template.CategoryId,
                WalletId = TransactionData.WalletId,
                Date = TransactionData.Date ?? DateTime.Now
            });
        }


        Grid CreateTemplateMenuItem(TemplateDB data)
        {
            Grid body = new Grid();

            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());


            TextBlock name = new TextBlock
            {
                Text = data.Name,
                Foreground = Colors.MainText,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            body.Children.Add(name);


            Balance balance = new Balance((double)data.Amount, null);

            TextBlock amount = new TextBlock
            {
                Text = $"{(data.Type == AmountType.Profit ? '+' : '-')}{balance}",
                Foreground = data.Type == AmountType.Profit ? Brushes.LimeGreen : Brushes.Tomato,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            amount.SetValue(Grid.ColumnProperty, 1);
            body.Children.Add(amount);


            return body;
        }

        void XTemplates_Click(object sender, EventArgs e)
        {
            TemplatesMenu.Items.Clear();

            AmountType type = sender == XProfitsTemplates ? AmountType.Profit : AmountType.Loss;

            TemplatesMenu.Items.AddRange( (from t in DBData.Templates where t.Type == type select CreateTemplateMenuItem(t)).ToArray() );

            if (TemplatesMenu.Items.Count > 0)
            {
                TemplatesMenu.IsOpen = false;
                TemplatesMenu.IsOpen = true;
            }
            else MsgBox.Show("Шаблоны отсутствуют", "Транзакция", MsgIcon.Info, "Ок");
        }

        void XCreateTemplate_Click(object sender, EventArgs e)
        {
            TemplateDB template = new TemplateDB
            {
                Type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss,
                Amount = XAmount.Input.Value,
                WalletId = TransactionData.WalletId
            };

            if (XCategory.Input.SelectItem != NONE)
                template.CategoryId = Categories.Find(c => c.Name == XCategory.Input.SelectItem && c.Type == template.Type).Id;
            else
                template.CategoryId = 0;


            new TemplateForm().ShowDialog_Create(template);
        }

        void XDone_Click(object sender, EventArgs e)
        {
            TransactionData.Type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

            TransactionData.Amount = XAmount.Input.Value;

            if (XCategory.Input.SelectItem != NONE)
                TransactionData.CategoryId = Categories.Find(c => c.Name == XCategory.Input.SelectItem && c.Type == TransactionData.Type).Id;
            else
                TransactionData.CategoryId = 0;

            TransactionData.Date = XDate.Input.Date;

            TransactionData.Description = XDescription.Input.Text;


            if (TransactionData.Id == null)
                DBData.Transactions.Add(TransactionData);
            else
                TransactionData.SaveChanges();


            Done = true;
            Close();
        }


        TextBlock CreateCapture(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = Colors.MainText,
                Margin = new Thickness { Left = 10, Right = 10 },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        void BuildFields()
        {
            // WALLET

            XWallet = new Field<TextBlock>("Кошелек:", new TextBlock { Foreground = Colors.SecondText1 });


            // TYPE

            XType = new Field<ChoiceChips>("Тип:", new ChoiceChips());

            XType.Input.Items.Add(PROFIT, LOSS);

            XType.Input.SelectionChanged += (sender, item) => SetCategories();


            // AMOUNT

            XAmount = new Field<NumberField>("Сумма:", new NumberField { Height = 20, Width = 100, Maximum = 1000000000 });
            

            // CATEGORY

            XCategory = new Field<DropButton>("Категория:", new DropButton { Height = 20, Width = 100 });


            // DATE

            XDate = new Field<DateSelector>("Дата:", new DateSelector { Height = 25 });


            // DESCRIPTION

            XDescription = new Field<MultilineTextBox>("Описание:", new MultilineTextBox { Height = 60, Width = 120 })
            { CaptureMargin = new Thickness { Top = 8, Bottom = 60 } };


            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(XWallet.Body);
            properties.Children.Add(XType.Body);
            properties.Children.Add(XAmount.Body);
            properties.Children.Add(XCategory.Body);
            properties.Children.Add(XDate.Body);
            properties.Children.Add(XDescription.Body);
        }


        void SetFields(TransactionDB data)
        {
            XWallet.Input.Text = DBData.Wallets.Find(w => w.Id == data.WalletId).Capture;

            XType.Input.SelectItem = data.Type == AmountType.Profit ? PROFIT : LOSS;

            XAmount.Input.Value = (double)data.Amount;

            SetCategories();
            if (data.CategoryId != 0)
                XCategory.Input.SelectItem = Categories.Find(c => c.Id == data.CategoryId).Name;

            XDate.Input.Date = data.Date.Value;

            XDescription.Input.Text = data.Description == " " ? null : data.Description;
        }

        public bool ShowDialog_Create(long walletId)
        {
            TransactionData = new TransactionDB { WalletId = walletId };

            XWallet.Input.Text = DBData.Wallets.Find(w => w.Id == walletId).Capture;
            SetCategories();

            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Change(TransactionDB data)
        {
            if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");

            TransactionData = data;
            SetFields(data);

            ((TextBlock)XDone.Child).Text = "Сохранить";

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public TransactionForm()
        {
            InitializeComponent();

            PROFIT = CreateCapture("Доход");
            LOSS = CreateCapture("Расход");

            BuildFields();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseDown += (sender, e) => DragMove();

            XCreateTemplate.Click += XCreateTemplate_Click;
            XProfitsTemplates.Click += XTemplates_Click;
            XLossesTemplates.Click += XTemplates_Click;
            XCancel.Click += (sender, e) => Close();
            XDone.Click += XDone_Click;

            TemplatesMenu.SelectionChanged += TemplatesMenu_SelectionChanged;
        }
    }
}
