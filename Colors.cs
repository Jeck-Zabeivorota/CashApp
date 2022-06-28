using System.Windows;
using System.Windows.Media;

namespace CashApp
{
    public static class Colors
    {
        public static Brush Deep;
        public static Brush Background;

        public static Brush Accent1;
        public static Brush Accent2;
        public static Brush Accent3;

        public static Brush MainText;
        public static Brush SecondText1;
        public static Brush SecondText2;
        public static Brush SecondText3;

        public static void SetColors(ResourceDictionary resource)
        {
            Deep = (SolidColorBrush)resource["Deep"];
            Background = (SolidColorBrush)resource["Background"];

            Accent1 = (SolidColorBrush)resource["Accent1"];
            Accent2 = (SolidColorBrush)resource["Accent2"];
            Accent3 = (SolidColorBrush)resource["Accent3"];

            MainText = (SolidColorBrush)resource["MainText"];
            SecondText1 = (SolidColorBrush)resource["SecondText1"];
            SecondText2 = (SolidColorBrush)resource["SecondText2"];
            SecondText3 = (SolidColorBrush)resource["SecondText3"];
        }
    }
}
