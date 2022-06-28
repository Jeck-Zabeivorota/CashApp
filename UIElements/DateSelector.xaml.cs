using System;
using System.Windows.Controls;

namespace CashApp.UIElements
{
    public partial class DateSelector : UserControl
    {
        public event EventHandler SelectionChanged;


        bool isBlock = false;

        public DateTime Date
        {
            get => new DateTime((int)XYear.Value, (int)XMonth.Value, (int)XDay.Value);
            set
            {
                isBlock = true;

                XYear.Value = value.Year;
                XMonth.Value = value.Month;
                XDay.Value = value.Day;

                isBlock = false;


                SelectionChanged?.Invoke(this, new EventArgs());
            }
        }


        void Date_ValueChanged(object sender, EventArgs e)
        {
            XDay.Maximum = DateTime.DaysInMonth((int)XYear.Value, (int)XMonth.Value);

            if (!isBlock) SelectionChanged?.Invoke(this, new EventArgs());
        }


        public DateSelector()
        {
            InitializeComponent();


            DateTime now = DateTime.Now;

            XDay.Maximum = DateTime.DaysInMonth(now.Year, now.Month);
            XDay.Value = now.Day;

            XMonth.Value = now.Month;

            XYear.Value = now.Year;


            XDay.ValueChanged += Date_ValueChanged;
            XMonth.ValueChanged += Date_ValueChanged;
            XYear.ValueChanged += Date_ValueChanged;
        }
    }
}
