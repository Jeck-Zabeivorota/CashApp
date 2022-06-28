using CashApp.UIElements;
using System.Windows.Controls;

namespace CashApp
{
    public partial class WindowTop : Border
    {
        public string Title
        {
            get => (XBody.Child as TextBlock).Text;
            set => (XBody.Child as TextBlock).Text = value;
        }

        public Border Body => XBody;
        public FlatButton CloseButton => XCloseButton;
        public FlatButton MinimizeButton => XMinButton;


        public WindowTop()
        {
            InitializeComponent();
        }
    }
}
