using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public enum MsgIcon { None, Info, Question, Error, Warning };

    public partial class MsgBox : Window
    {
        string Result;
        

        void Button_Click(object sender, EventArgs e)
        {
            Result = ((TextBlock)((FlatButton)sender).Child).Text;
            Close();
        }

        static FlatButton CreateButton(string text)
        {
            return new FlatButton
            {
                Margin = new Thickness { Left = 10, Bottom = 5 },
                Height = 20,
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = Colors.MainText,
                    Margin = new Thickness { Left = 15, Right = 15 },
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
        }


        public static string Show(string message, string title, MsgIcon icon, params string[] buttons)
        {
            MsgBox msg = new MsgBox();

            msg.XTop.Title = title;

            msg.XMessage.Text = message;

            switch (icon)
            {
                case MsgIcon.Info:
                    msg.XEllipse.Fill = Brushes.DeepSkyBlue;
                    msg.XSymbol.Text = "i";
                    break;

                case MsgIcon.Question:
                    msg.XEllipse.Fill = Brushes.DeepSkyBlue;
                    msg.XSymbol.Text = "?";
                    break;

                case MsgIcon.Error:
                    msg.XEllipse.Fill = Brushes.Tomato;
                    msg.XSymbol.Text = "!";
                    break;

                case MsgIcon.Warning:
                    msg.XEllipse.Fill = Brushes.Gold;
                    msg.XSymbol.Text = "⚠";
                    break;
            }

            foreach (string button in buttons)
            {
                FlatButton fb = CreateButton(button);
                fb.Click += msg.Button_Click;
                msg.XButtons.Children.Add(fb);
            }


            msg.ShowDialog();

            return msg.Result;
        }


        MsgBox()
        {
            InitializeComponent();

            XTop.CloseButton.Visibility = Visibility.Collapsed;
            XTop.MinimizeButton.Visibility = Visibility.Collapsed;
            XTop.Body.MouseDown += (sender, e) => DragMove();
        }
    }
}
