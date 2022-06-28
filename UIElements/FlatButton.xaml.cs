using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public partial class FlatButton : Border
    {
        bool isPressed = false;

        Brush DefaultColor;


        Brush _mouseOver_Color;
        public Brush MouseOver_Color
        {
            get => _mouseOver_Color ?? Colors.Accent2;
            set => _mouseOver_Color = value;
        }


        Brush _mouseDown_Color;
        public Brush MouseDown_Color
        {
            get => _mouseDown_Color ?? Colors.Accent3;
            set => _mouseDown_Color = value;
        }


        public event EventHandler Click;


        void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            DefaultColor = Background;
            Background = MouseOver_Color;
        }

        void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            isPressed = false;
            Background = DefaultColor;
        }

        void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPressed = true;
            Background = MouseDown_Color;
        }

        void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isPressed) return;

            Background = MouseOver_Color;
            Click?.Invoke(this, new EventArgs());
        }


        public FlatButton()
        {
            InitializeComponent();
        }
    }
}
