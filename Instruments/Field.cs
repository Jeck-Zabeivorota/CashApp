using System.Windows;
using System.Windows.Controls;

namespace CashApp.Instruments
{
    class Field<Tinput>
    {
        readonly public Grid Body;
        readonly public TextBlock Capture;
        readonly public Tinput Input;

        public double CaptureWidth
        {
            get => Body.ColumnDefinitions[0].Width.Value;
            set => Body.ColumnDefinitions[0].Width = new GridLength(value);
        }

        public Thickness CaptureMargin
        {
            get => Capture.Margin;
            set => Capture.Margin = value;
        }

        public Field(string capture, Tinput input)
        {
            Body = new Grid();
            Body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
            Body.ColumnDefinitions.Add(new ColumnDefinition());


            Capture = new TextBlock
            {
                Text = capture,
                Foreground = Colors.MainText,
                Margin = new Thickness { Top = 8, Bottom = 8 },
                VerticalAlignment = VerticalAlignment.Center
            };
            Body.Children.Add(Capture);


            Input = input;
            var FEInput = input as FrameworkElement;

            FEInput.SetValue(Grid.ColumnProperty, 1);
            FEInput.HorizontalAlignment = HorizontalAlignment.Left;
            FEInput.VerticalAlignment = VerticalAlignment.Center;

            Body.Children.Add(FEInput);
        }
    }
}
