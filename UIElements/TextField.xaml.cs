using System.Windows.Controls;
using System.Windows.Shapes;

namespace CashApp.UIElements
{
    public partial class TextField : TextBox
    {
        readonly Rectangle XUnderLine;

        public double Underline_Thickness
        {
            get => XUnderLine.Height;
            set => XUnderLine.Height = value;
        }

        public TextField()
        {
            InitializeComponent();

            ApplyTemplate();

            XUnderLine = (Rectangle)Template.FindName("PART_UnderLine", this);
        }
    }
}
