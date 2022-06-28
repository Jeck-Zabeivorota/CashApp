using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public partial class Check : Border
    {
        bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    _isChecked = true;
                    XThumb.HorizontalAlignment = HorizontalAlignment.Right;
                    Background = CheckedColor;
                }
                else
                {
                    _isChecked = false;
                    XThumb.HorizontalAlignment = HorizontalAlignment.Left;
                    Background = UncheckedColor;
                }

                Checked?.Invoke(this, new EventArgs());
            }
        }

        public Brush CheckedColor = Brushes.LimeGreen;

        public Brush UncheckedColor = Brushes.Tomato;


        public event EventHandler Checked;


        void Check_MouseUp(object sender, MouseButtonEventArgs e) => IsChecked = !IsChecked;


        public Check()
        {
            InitializeComponent();
        }
    }
}
