using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Templates
{
    public partial class TemplateForm : Window
    {
        // FIELDS

        readonly TextBlock PROFIT, LOSS;
        const string NONE = "<Ничего>", GENERAL = "<Общий>";

        Field<TextBlock> XWallet;
        Field<TextField> XName;
        Field<ChoiceChips> XType;
        Field<DropButton> XCategory;
        Field<NumberField> XAmount;

        List<CategoryDB> Categories = new List<CategoryDB>();

        public TemplateDB TemplateData;
        bool Done = false;


        // METHODS

        void SetCategories()
        {
            AmountType type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

            Categories = DBData.Categories.Where(c => c.Type == type && (c.WalletId == 0 || c.WalletId == TemplateData.WalletId)).ToList();

            XCategory.Input.Items.Clear();
            XCategory.Input.Items.Add(new TextBlock { Text = NONE, Foreground = Colors.MainText });
            XCategory.Input.Items.AddRange( Categories.Select(c => new TextBlock { Text = c.Name, Foreground = Colors.MainText }).ToArray() );
        }

        void XDone_Click(object sender, EventArgs e)
        {
            AmountType type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

            if (TemplateData.Id == null && DBData.Templates.Find(t => t.Name == XName.Input.Text && t.Type == type) != null)
            {
                MsgBox.Show("Шаблон с таким именем уже существует!", "Шаблон", MsgIcon.Error, "Ок");
                return;
            }


            TemplateData.Name = XName.Input.Text;

            TemplateData.Type = type;

            TemplateData.Amount = XAmount.Input.Value;

            if (XCategory.Input.SelectItem != NONE)
                TemplateData.CategoryId = Categories.Find(c => c.Name == XCategory.Input.SelectItem && c.Type == type).Id;
            else
                TemplateData.CategoryId = 0;


            if (TemplateData.Id == null)
                DBData.Templates.Add(TemplateData);
            else
                TemplateData.SaveChanges();


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


            // NAME

            XName = new Field<TextField>("Название:", new TextField { Height = 20, Width = 120 });


            // TYPE

            XType = new Field<ChoiceChips>("Тип:", new ChoiceChips());

            XType.Input.Items.Add(PROFIT, LOSS);

            XType.Input.SelectionChanged += (sender, item) => SetCategories();


            // AMOUNT

            XAmount = new Field<NumberField>("Сумма:", new NumberField { Height = 20, Width = 100, Maximum = 1000000000 });


            // CATEGORY

            XCategory = new Field<DropButton>("Категория:", new DropButton { Height = 20, Width = 100 });


            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(XWallet.Body);
            properties.Children.Add(XName.Body);
            properties.Children.Add(XType.Body);
            properties.Children.Add(XAmount.Body);
            properties.Children.Add(XCategory.Body);
        }


        void SetFields(TemplateDB data)
        {
            XWallet.Input.Text = data.WalletId == 0 ? GENERAL : DBData.Wallets.Find(w => w.Id == data.WalletId).Capture;

            XName.Input.Text = data.Name;

            XType.Input.SelectItem = data.Type == AmountType.Profit ? PROFIT : LOSS;

            XAmount.Input.Value = (double)data.Amount;

            SetCategories();
            if (data.CategoryId != 0)
                XCategory.Input.SelectItem = Categories.Find(c => c.Id == data.CategoryId).Name;
        }

        public bool ShowDialog_Create(long walletId = 0)
        {
            TemplateData = new TemplateDB { WalletId = walletId };

            XWallet.Input.Text = walletId == 0 ? GENERAL : DBData.Wallets.Find(w => w.Id == walletId).Capture;
            SetCategories();

            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Create(TemplateDB data)
        {
            if (data.Id != null) throw new ArgumentException("\"Id\" is not null");

            TemplateData = data;
            SetFields(data);

            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Change(TemplateDB data)
        {
            if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");

            TemplateData = data;
            SetFields(data);

            ((TextBlock)XDone.Child).Text = "Сохранить";

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public TemplateForm()
        {
            InitializeComponent();

            PROFIT = CreateCapture("Доход");
            LOSS = CreateCapture("Расход");

            BuildFields();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseDown += (sender, e) => DragMove();

            XCancel.Click += (sender, e) => Close();
            XDone.Click += XDone_Click;
        }
    }
}
