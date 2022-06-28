using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public partial class ScrollPanel : ScrollViewer
    {
        // FIELDS

        ScrollBar XVerticalScrollBar;
        ScrollBar XHorizontalScrollBar;

        Border XVerticalThumbBorder;
        Border XHorizontalThumbBorder;


        // SCROLL BARS

        public double ScrollBars_Size
        {
            get => XVerticalScrollBar.Width;
            set
            {
                XVerticalScrollBar.Width = value;
                XVerticalScrollBar.MinWidth = value;

                XHorizontalScrollBar.Height = value;
                XHorizontalScrollBar.MinHeight = value;
            }
        }


        // THUMBS

        public Brush Thumb_Background
        {
            get => XVerticalThumbBorder.Background;
            set
            {
                XVerticalThumbBorder.Background = value;
                XHorizontalThumbBorder.Background = value;
            }
        }

        public CornerRadius Thumb_CornerRadius
        {
            get => XVerticalThumbBorder.CornerRadius;
            set
            {
                XVerticalThumbBorder.CornerRadius = value;
                XHorizontalThumbBorder.CornerRadius = value;
            }
        }


        // CONSTRUCTORS AND EVENTS

        public ScrollPanel()
        {
            InitializeComponent();

            ApplyTemplate();

            XVerticalScrollBar = (ScrollBar)Template.FindName("PART_VerticalScrollBar", this);
            XHorizontalScrollBar = (ScrollBar)Template.FindName("PART_HorizontalScrollBar", this);
            XHorizontalScrollBar.ApplyTemplate();
            XVerticalScrollBar.ApplyTemplate();

            Track track;

            track = (Track)XVerticalScrollBar.Template.FindName("PART_Track", XVerticalScrollBar);
            track.Thumb.ApplyTemplate();
            XVerticalThumbBorder = (Border)track.Thumb.Template.FindName("PART_Thumb", track.Thumb);

            track = (Track)XHorizontalScrollBar.Template.FindName("PART_Track", XHorizontalScrollBar);
            track.Thumb.ApplyTemplate();
            XHorizontalThumbBorder = (Border)track.Thumb.Template.FindName("PART_Thumb", track.Thumb);
        }
    }
}
