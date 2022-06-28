using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public partial class Link : TextBlock
    {
        bool isPressed = false;
        Brush DefaultColor;


        Brush _mouseOver_Color;
        public Brush MouseOver_Color
        {
            get => _mouseOver_Color ?? Colors.SecondText2;
            set => _mouseOver_Color = value;
        }


        Brush _mouseDown_Color;
        public Brush MouseDown_Color
        {
            get => _mouseDown_Color ?? Colors.SecondText3;
            set => _mouseDown_Color = value;
        }


        public event EventHandler Click;


        void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            DefaultColor = Foreground;
            Foreground = MouseOver_Color;
        }

        void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            Foreground = DefaultColor;
            isPressed = false;
        }

        void Link_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPressed = true;
            Foreground = MouseDown_Color;
        }

        void Link_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isPressed) return;

            Foreground = MouseOver_Color;
            Click?.Invoke(this, new EventArgs());
        }


        public Link()
        {
            InitializeComponent();
        }
    }
}
