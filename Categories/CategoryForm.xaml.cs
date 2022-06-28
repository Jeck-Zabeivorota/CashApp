using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Categories
{
    public partial class CategoryForm : Window
    {
        // FIELDS

        readonly TextBlock PROFIT, LOSS;
        const string GENERAL = "<Общий>";

        Field<TextBlock> XWallet;
        Field<ChoiceChips> XType;
        Field<TextField> XName;
        Field<ColorPicker> XBackcolor;

        public CategoryDB CategoryData;
        bool Done = false;


        // METHODS

        void XDone_Click(object sender, EventArgs e)
        {
            if (XName.Input.Text == null)
            {
                MsgBox.Show("Не все поля заполнены!", "Категория", MsgIcon.Error, "Ок");
                return;
            }

            AmountType type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;

            if (CategoryData.Id == null && DBData.Categories.Find(c => c.Name == XName.Input.Text && c.Type == type) != null)
            {
                MsgBox.Show("Такая категория уже существует!", "Категория", MsgIcon.Error, "Ок");
                return;
            }


            CategoryData.Type = type;

            CategoryData.Name = XName.Input.Text;

            CategoryData.Backcolor = (SolidColorBrush)XBackcolor.Input.Background;


            if (CategoryData.Id == null)
                DBData.Categories.Add(CategoryData);
            else
                CategoryData.SaveChanges();


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


            // CATEGORY

            XName = new Field<TextField>("Название:", new TextField { Height = 20, Width = 120 });


            // BACKCOLOR

            XBackcolor = new Field<ColorPicker>("Цвет:", new ColorPicker { Height = 20, Width = 20 });


            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(XWallet.Body);
            properties.Children.Add(XType.Body);
            properties.Children.Add(XName.Body);
            properties.Children.Add(XBackcolor.Body);
        }


        public bool ShowDialog_Create(long walletId = 0)
        {
            CategoryData = new CategoryDB { WalletId = walletId };
            XWallet.Input.Text = walletId == 0 ? GENERAL : DBData.Wallets.Find(w => w.Id == walletId).Capture;
            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Change(CategoryDB data)
        {
            if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");


            XWallet.Input.Text = data.WalletId == 0 ? GENERAL : DBData.Wallets.Find(w => w.Id == data.WalletId).Capture;

            XType.Input.SelectItem = data.Type == AmountType.Profit ? PROFIT : LOSS;

            XName.Input.Text = data.Name;

            XBackcolor.Input.Background = data.Backcolor;


            ((TextBlock)XDone.Child).Text = "Сохранить";
            CategoryData = data;

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public CategoryForm()
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
