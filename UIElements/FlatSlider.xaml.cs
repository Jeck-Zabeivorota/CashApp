using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CashApp.UIElements
{
    public partial class FlatSlider : Slider
    {
        readonly Border Thumb, Track;


        public Brush Thumb_Background
        {
            get => Thumb.Background;
            set => Thumb.Background = value;
        }

        public CornerRadius Thumb_CornerRadius
        {
            get => Thumb.CornerRadius;
            set => Thumb.CornerRadius = value;
        }


        public Brush Track_Background
        {
            get => Track.Background;
            set => Track.Background = value;
        }

        public CornerRadius Track_CornerRadius
        {
            get => Track.CornerRadius;
            set => Track.CornerRadius = value;
        }


        Thickness DefaultThumbThickness;
        void Slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DefaultThumbThickness = Thumb.BorderThickness;
            Thumb.BorderThickness = new Thickness(4);
        }
        void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thumb.BorderThickness = DefaultThumbThickness;
        }


        public FlatSlider()
        {
            InitializeComponent();

            ApplyTemplate();
            Track = (Border)Template.FindName("PART_Border", this);
            Thumb thumb = (Template.FindName("PART_Track", this) as Track).Thumb;

            thumb.ApplyTemplate();
            Thumb = (Border)thumb.Template.FindName("PART_Border", thumb);
        }
    }
}
