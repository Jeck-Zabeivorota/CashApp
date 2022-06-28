using System.Windows.Controls;

using CashApp.DataBase;
using CashApp.Instruments;

namespace CashApp.Statistics
{
    public partial class CategoryItem : Grid
    {
        public CategoryItem(CategoryDB category, double amount, string currency, double amountsSum)
        {
            InitializeComponent();


            XPoint.Fill = category.Backcolor;

            XCategory.Text = category.Name;

            double raito = amount / amountsSum;

            XInfo.Text = $"{new Balance(amount, currency)} | {raito:P1}";
        }
    }
}
