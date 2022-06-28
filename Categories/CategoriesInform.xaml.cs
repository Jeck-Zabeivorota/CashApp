using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

using CashApp.DataBase;
using CashApp.UIElements;

namespace CashApp.Categories
{
    public partial class CategoriesInform : Window
    {
        // FIELDS

        readonly Tab TabProfits = new Tab("Доходные", new ListPanel());
        readonly Tab TabLosses = new Tab("Расходные", new ListPanel());
        ListPanel SelectList;

        readonly MainWindow Window;


        // METHODS

        StackPanel CreateItem(CategoryDB category)
        {
            StackPanel body = new StackPanel { Orientation = Orientation.Horizontal };

            Ellipse ellipse = new Ellipse
            {
                Fill = category.Backcolor,
                Width = 8,
                Height = 8,
                VerticalAlignment = VerticalAlignment.Center
            };
            body.Children.Add(ellipse);

            TextBlock text = new TextBlock
            {
                Text = category.Name,
                Foreground = Colors.MainText,
                Margin = new Thickness { Left = 10 },
                VerticalAlignment = VerticalAlignment.Center
            };
            body.Children.Add(text);

            return body;
        }

        void DownloadCategories()
        {
            ListPanel profits = (ListPanel)TabProfits.Container,
                      losses = (ListPanel)TabLosses.Container;

            profits.Items.Clear();
            losses.Items.Clear();

            foreach (CategoryDB category in DBData.Categories)
                (category.Type == AmountType.Profit ? profits : losses).Items.Add(CreateItem(category));
        }

        void XAdd_Click(object sender, EventArgs e)
        {
            CategoryForm window = new CategoryForm();

            if (!window.ShowDialog_Create()) return;


            if (window.CategoryData.Type == AmountType.Profit)
                XCategories.SelectItem = TabProfits;
            else
                XCategories.SelectItem = TabLosses;

            SelectList.Items.Add(CreateItem(window.CategoryData));
        }

        void XDelete_Click(object sender, EventArgs e)
        {
            if (SelectList.SelectItem == null)
            {
                MsgBox.Show("Категория не выбранная!", "Категории", MsgIcon.Error, "Ок");
                return;
            }

            string categoryName = ((SelectList.SelectItem as StackPanel).Children[1] as TextBlock).Text;

            if (MsgBox.Show($"Удалить \"{categoryName}\"?", "Категории", MsgIcon.Question, "Да", "Нет") == "Нет") return;


            CategoryDB category = DBData.Categories.Find(c => c.Name == categoryName);

            SelectList.Items.Remove(SelectList.SelectItem);
            DBData.Categories.Remove(category);

            Window.Transactions.DownloadTransactions();
            Window.StatisticProfits.DownloadStatistics();
            Window.StatisticLosses.DownloadStatistics();
        }

        void XChange_Click(object sender, EventArgs e)
        {
            if (SelectList.SelectItem == null)
            {
                MsgBox.Show("Категория не выбранная!", "Категории", MsgIcon.Error, "Ок");
                return;
            }


            string categoryName = ((TextBlock)((StackPanel)SelectList.SelectItem).Children[1]).Text;
            CategoryDB category = DBData.Categories.Find(c => c.Name == categoryName);

            CategoryForm window = new CategoryForm();

            if (!window.ShowDialog_Change(category)) return;


            DownloadCategories();

            Window.Transactions.DownloadTransactions();
            Window.StatisticProfits.DownloadStatistics();
            Window.StatisticLosses.DownloadStatistics();
        }


        // CONSTRUCTORS

        public CategoriesInform(MainWindow window)
        {
            InitializeComponent();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseDown += (sender, e) => DragMove();


            XAdd.Click += XAdd_Click;
            XDelete.Click += XDelete_Click;
            XChange.Click += XChange_Click;


            Window = window;

            XCategories.SelectedTab += (sender, tab) => SelectList = (ListPanel)tab.Container;
            XCategories.Items.Add(TabProfits, TabLosses);

            DownloadCategories();
        }
    }
}
