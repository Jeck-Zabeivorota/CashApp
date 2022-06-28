using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashApp.DataBase;
using CashApp.Instruments;
using CashApp.UIElements;

namespace CashApp.Templates
{
    public partial class TemplatesInform : Window
    {
        // FIELDS

        readonly Tab TabProfits = new Tab("Доходные", new ListPanel());
        readonly Tab TabLosses = new Tab("Расходные", new ListPanel());
        ListPanel SelectList;


        // METHODS

        Grid CreateItem(TemplateDB data)
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

        void DownloadTemplates()
        {
            ListPanel profits = (ListPanel)TabProfits.Container,
                      losses = (ListPanel)TabLosses.Container;

            profits.Items.Clear();
            losses.Items.Clear();

            foreach (TemplateDB template in DBData.Templates)
                (template.Type == AmountType.Profit ? profits : losses).Items.Add(CreateItem(template));
        }

        void XAdd_Click(object sender, EventArgs e)
        {
            TemplateForm window = new TemplateForm();

            if (!window.ShowDialog_Create()) return;


            if (window.TemplateData.Type == AmountType.Profit)
                XTemplates.SelectItem = TabProfits;
            else
                XTemplates.SelectItem = TabLosses;

            SelectList.Items.Add(CreateItem(window.TemplateData));
        }

        void XDelete_Click(object sender, EventArgs e)
        {
            if (SelectList.SelectItem == null)
            {
                MsgBox.Show("Шаблон не выбран!", "Шаблоны", MsgIcon.Error, "Oк");
                return;
            }


            string templateName = ((TextBlock)((Grid)SelectList.SelectItem).Children[0]).Text;

            if (MsgBox.Show($"Удалить \"{templateName}\"?", "Шаблоны", MsgIcon.Question, "Да", "Нет") == "Нет") return;


            TemplateDB template = DBData.Templates.Find(t => t.Name == templateName);
            template.Remove();

            SelectList.Items.RemoveAt(SelectList.SelectIndex);
            DBData.Templates.Remove(template);
        }

        void XChange_Click(object sender, EventArgs e)
        {
            if (SelectList.SelectItem == null)
            {
                MsgBox.Show("Шаблон не выбран!", "Шаблоны", MsgIcon.Error, "Ок");
                return;
            }

            Grid item = (Grid)SelectList.SelectItem;
            string templateName = ((TextBlock)item.Children[0]).Text;
            AmountType type = SelectList == TabProfits.Container ? AmountType.Profit : AmountType.Loss;

            TemplateDB template = DBData.Templates.Find(t => t.Name == templateName && t.Type == type);

            TemplateForm window = new TemplateForm();

            if (window.ShowDialog_Change(template))
                DownloadTemplates();
        }


        // CONSTRUCTORS

        public TemplatesInform()
        {
            InitializeComponent();

            XTop.CloseButton.Click += (sender, e) => Close();
            XTop.MinimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            XTop.Body.MouseDown += (sender, e) => DragMove();


            XAdd.Click += XAdd_Click;
            XDelete.Click += XDelete_Click;
            XChange.Click += XChange_Click;


            XTemplates.SelectedTab += (sender, tab) => SelectList = (ListPanel)tab.Container;
            XTemplates.Items.Add(TabProfits, TabLosses);

            DownloadTemplates();
        }
    }
}
