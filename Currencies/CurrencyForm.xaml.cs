using System;
using System.Windows;
using System.Windows.Controls;

using CashApp.DataBase;
using CashApp.UIElements;
using CashApp.Instruments;

namespace CashApp.Currencies
{
    public partial class CurrencyForm : Window
    {
        // FIELDS

        Field<TextField> XCurrency, XName;

        public CurrencyDB CurrencyData;
        bool Done = false;


        // METHODS

        void XDone_Click(object sender, EventArgs e)
        {
            if (XCurrency.Input.Text == null && XName.Input.Text == null)
            {
                MsgBox.Show("Не все поля заполнены!", "Валюта", MsgIcon.Error, "Ок");
                return;
            }

            if (XCurrency.Input.Text.Length > 5)
            {
                MsgBox.Show("Знак содержит больше 5 символов!", "Валюта", MsgIcon.Error, "Ок");
                return;
            }

            bool isFind = DBData.Currencies.Exists(c => (c.Name == XName.Input.Text || c.Currency == XCurrency.Input.Text) && c != CurrencyData);

            if (isFind)
            {
                MsgBox.Show("Валюта с таким названием или\nзнаком уже существует!", "Валюта", MsgIcon.Error, "Ок");
                return;
            }


            CurrencyData.Currency = XCurrency.Input.Text;

            CurrencyData.Name = XName.Input.Text;


            if (CurrencyData.Id == null)
                DBData.Currencies.Add(CurrencyData);
            else
                CurrencyData.SaveChanges();


            Done = true;
            Close();
        }

        void BuildFields()
        {
            // CURRENCY

            XCurrency = new Field<TextField>("Знак:", new TextField { Height = 20, Width = 50 });


            // NAME

            XName = new Field<TextField>("Название:", new TextField { Height = 20, Width = 120 });


            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(XCurrency.Body);
            properties.Children.Add(XName.Body);
        }


        public bool ShowDialog_Create()
        {
            CurrencyData = new CurrencyDB();
            ShowDialog();
            return Done;
        }

        public bool ShowDialog_Change(CurrencyDB data)
        {
            if (data.Id == null) throw new ArgumentNullException("\"Id\" is null");


            XCurrency.Input.Text = data.Currency;

            XName.Input.Text = data.Name;


            ((TextBlock)XDone.Child).Text = "Сохранить";
            CurrencyData = data;

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public CurrencyForm()
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
