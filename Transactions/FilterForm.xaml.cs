using System;
using System.Windows;
using System.Windows.Controls;

using CashApp.Instruments;
using CashApp.UIElements;
using CashApp.DataBase;
using System.Linq;

namespace CashApp.Transactions
{
    public partial class FilterForm : Window
    {
        // CLASSES

        class DateField
        {
            readonly Field<Check> XCheck;
            readonly Field<DateSelector> XStartDate, XEndDate;

            public readonly StackPanel Body = new StackPanel();

            public void SetDate(FilterData data)
            {
                if (XCheck.Input.IsChecked)
                {
                    data.StartDate = XStartDate.Input.Date;
                    data.EndDate = XEndDate.Input.Date;
                }
                else data.StartDate = data.EndDate = null;
            }
            public void GetDate(FilterData data)
            {
                if (data.StartDate == null && data.EndDate == null)
                {
                    XCheck.Input.IsChecked = false;
                    return;
                }

                XCheck.Input.IsChecked = true;
                XStartDate.Input.Date = data.StartDate.Value;
                XEndDate.Input.Date = data.EndDate.Value;
            }


            void Check_Checked(object sender, EventArgs e)
            {
                if (XCheck.Input.IsChecked)
                {
                    XStartDate.Body.Visibility = Visibility.Visible;
                    XEndDate.Body.Visibility = Visibility.Visible;
                }
                else
                {
                    XStartDate.Body.Visibility = Visibility.Collapsed;
                    XEndDate.Body.Visibility = Visibility.Collapsed;
                }
            }


            public DateField()
            {
                XCheck = new Field<Check>("По дате:", new Check()) { CaptureWidth = 90 };
                XCheck.Input.Checked += Check_Checked;

                XStartDate = new Field<DateSelector>("Начало:", new DateSelector { Height = 25 });
                XStartDate.Body.Margin = new Thickness { Left = 20 };
                XStartDate.Body.Visibility = Visibility.Collapsed;

                XEndDate = new Field<DateSelector>("Конец:", new DateSelector { Height = 25 });
                XEndDate.Body.Margin = new Thickness { Left = 20 };
                XEndDate.Body.Visibility = Visibility.Collapsed;


                Body.Children.Add(XCheck.Body);
                Body.Children.Add(XStartDate.Body);
                Body.Children.Add(XEndDate.Body);
            }
        }

        class TypeField
        {
            readonly TextBlock PROFIT, LOSS;

            readonly Field<Check> XCheck;
            readonly Field<ChoiceChips> XType;

            public readonly StackPanel Body = new StackPanel();


            public void SetDate(FilterData data)
            {
                if (XCheck.Input.IsChecked)
                    data.Type = XType.Input.SelectItem == PROFIT ? AmountType.Profit : AmountType.Loss;
                else
                    data.Type = null;
            }
            public void GetDate(FilterData data)
            {
                if (data.Type == null)
                {
                    XCheck.Input.IsChecked = false;
                    return;
                }

                XCheck.Input.IsChecked = true;
                XType.Input.SelectItem = data.Type == AmountType.Profit ? PROFIT : LOSS;
            }

            void Check_Checked(object sender, EventArgs e)
            {
                if (XCheck.Input.IsChecked)
                    XType.Body.Visibility = Visibility.Visible;
                else
                    XType.Body.Visibility = Visibility.Collapsed;
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


            public TypeField()
            {
                XCheck = new Field<Check>("По типу:", new Check()) { CaptureWidth = 90 };
                XCheck.Input.Checked += Check_Checked;

                PROFIT = CreateCapture("Доход");
                LOSS = CreateCapture("Расход");

                XType = new Field<ChoiceChips>("Тип:", new ChoiceChips());
                XType.Body.Margin = new Thickness { Left = 20 };
                XType.Body.Visibility = Visibility.Collapsed;
                XType.Input.Items.Add(PROFIT, LOSS);


                Body.Children.Add(XCheck.Body);
                Body.Children.Add(XType.Body);
            }
        }

        class CategoryField
        {
            readonly Field<Check> XCheck;
            readonly Field<DropButton> XCategory;

            public readonly StackPanel Body = new StackPanel();


            public void SetDate(FilterData data)
            {
                if (XCheck.Input.IsChecked)
                {
                    AmountType type = XCategory.Input.SelectItem[1] == '+' ? AmountType.Profit : AmountType.Loss;
                    string name = XCategory.Input.SelectItem.Substring(4);
                    data.CategoryId = DBData.Categories.Find(c => c.Name == name && c.Type == type).Id;
                }
                else
                    data.CategoryId = null;
            }
            public void GetDate(FilterData data)
            {
                if (data.CategoryId == null)
                {
                    XCheck.Input.IsChecked = false;
                    return;
                }

                XCheck.Input.IsChecked = true;

                CategoryDB category = DBData.Categories.Find(c => c.Id == data.CategoryId);
                string begin = category.Type == AmountType.Profit ? "(+) " : "(-) ";
                XCategory.Input.SelectItem = begin + category.Name;
            }


            void Check_Checked(object sender, EventArgs e)
            {
                if (XCheck.Input.IsChecked)
                    XCategory.Body.Visibility = Visibility.Visible;
                else
                    XCategory.Body.Visibility = Visibility.Collapsed;
            }


            public CategoryField()
            {
                XCheck = new Field<Check>("По категории:", new Check()) { CaptureWidth = 90 };
                XCheck.Input.Checked += Check_Checked;

                XCategory = new Field<DropButton>("Категория:", new DropButton { Height = 20, Width = 100 });
                XCategory.Body.Margin = new Thickness { Left = 20 };
                XCategory.Body.Visibility = Visibility.Collapsed;


                if (DBData.Categories.Count > 0)
                {
                    XCategory.Input.Items.AddRange(DBData.Categories.Select(c => new TextBlock
                    {
                        Text = (c.Type == AmountType.Profit ? "(+) " : "(-) ") + c.Name,
                        Foreground = Colors.MainText
                    }).ToArray());
                }
                else XCheck.Input.IsEnabled = false;


                Body.Children.Add(XCheck.Body);
                Body.Children.Add(XCategory.Body);
            }
        }


        // FIELDS

        readonly DateField Date = new DateField();
        readonly TypeField Type = new TypeField();
        readonly CategoryField Category = new CategoryField();

        public FilterData FilterData;

        bool Done = false;


        // METHODS

        void XDone_Click(object sender, EventArgs e)
        {
            Date.SetDate(FilterData);
            Type.SetDate(FilterData);
            Category.SetDate(FilterData);

            Done = true;
            Close();
        }


        void BuildFields()
        {
            // ADD FIELDS

            StackPanel properties = (StackPanel)_XProperties_.Content;

            properties.Children.Add(Date.Body);
            properties.Children.Add(Type.Body);
            properties.Children.Add(Category.Body);
        }


        public bool ShowDialog_Change(FilterData data)
        {
            FilterData = data;

            Date.GetDate(data);
            Type.GetDate(data);
            Category.GetDate(data);

            ShowDialog();
            return Done;
        }


        // CONSTRUCTORS

        public FilterForm()
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
