using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CashApp.UIElements
{
    public partial class NumberField : UserControl
    {
        public event EventHandler ValueChanged;

        bool isBlock = false;


        double _value;
        public double Value
        {
            get => _value;
            set
            {
                if (value < Minimum | value > Maximum)
                    throw new ArgumentOutOfRangeException(value.ToString());

                _value = value;

                XText.Text = value.ToString();
            }
        }

        public double Step { get; set; } = 1;

        double _minimum = 0;
        public double Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;

                if (Value < value)
                {
                    isBlock = true;
                    Value = value;
                    isBlock = false;
                }
            }
        }

        double _maximum = 100;
        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;

                if (Value > value)
                {
                    isBlock = true;
                    Value = value;
                    isBlock = false;
                }
            }
        }

        public string Text => XText.Text;


        void XText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isBlock) return;


            isBlock = true;

            StringBuilder text = new StringBuilder(XText.Text);
            text.Replace('.', ',');

            foreach (char c in XText.Text)
                if (!char.IsDigit(c) & c != ',')
                    text.Replace(c.ToString(), null);

            XText.Text = text.ToString();

            if (XText.Text == "") XText.Text = Minimum.ToString();
            
            isBlock = false;


            _value = Convert.ToDouble(XText.Text);

            if (_value < Minimum)        _value = Minimum;
            else if (_value > Maximum)   _value = Maximum;


            ValueChanged?.Invoke(this, new EventArgs());
        }

        void XUp_Click(object sender, EventArgs e)
        {
            if (Value + Step <= Maximum)
                Value += Step;
            else
                Value = Maximum;

            ValueChanged?.Invoke(this, new EventArgs());
        }

        void XDown_Click(object sender, EventArgs e)
        {
            if (Value - Step >= Minimum)
                Value -= Step;
            else
                Value = Minimum;

            ValueChanged?.Invoke(this, new EventArgs());
        }


        public NumberField()
        {
            InitializeComponent();

            Value = Minimum;

            XText.TextChanged += XText_TextChanged;
            XUp.Click += XUp_Click;
            XDown.Click += XDown_Click;
        }
    }
}
