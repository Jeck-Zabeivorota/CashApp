using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using CashApp.Instruments;

namespace CashApp.UIElements
{
    public interface ITab
    {
        UIElement Container { get; }
        UIElement Button { get; }
    }


    public class Tab : ITab
    {
        readonly TextBlock _button;
        public string Capture
        {
            get => _button.Text;
            set => _button.Text = value;
        }


        UIElement ITab.Button => _button;

        public UIElement Container { get; set; }


        public Tab(string capture, UIElement container)
        {
            Container = container;

            _button = new TextBlock
            {
                Text = capture,
                Foreground = Colors.MainText,
                Margin = new Thickness { Left = 10, Right = 10 },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }
    }


    [ContentProperty("Items")]
    public partial class TabsPanel : UserControl
    {
        // FIELDS

        public readonly TrackList<ITab> Items = new TrackList<ITab>();


        ITab _selectTab;
        public ITab SelectItem
        {
            get => _selectTab;
            set
            {
                if (_selectTab == value) return;

                if (value == null)
                {
                    _selectTab = null;
                    XButtons.SelectItem = null;
                    XContainer.Child = null;
                    return;
                }

                if (!Items.Contains(value)) throw new ArgumentException("Tab is not found");

                _selectTab = value;
                XButtons.SelectItem = value.Button;
                XContainer.Child = value.Container;

                SelectedTab?.Invoke(this, value);
            }
        }

        public int SelectIndex
        {
            get => XButtons.SelectIndex;
            set => XButtons.SelectIndex = value;
        }


        public CornerRadius ButtonsCornerRadius
        {
            get => XButtons.CornerRadius;
            set => XButtons.CornerRadius = value;
        }

        public Brush ButtonsContainerBackground
        {
            get => XButtons.Background;
            set => XButtons.Background = value;
        }


        // EVENTS

        public delegate void SelectedTabHandler(object sender, ITab tab);
        public event SelectedTabHandler SelectedTab;


        // METHODS

        void XButtons_SelectionChanged(object sender, UIElement item)
        {
            SelectItem = Items.Find(tab => tab.Button == item);
        }


        void Items_AddingItems(object sender, ITab[] items, int startIndex)
        {
            XButtons.Items.InsertRange(startIndex, items.Select(item => item.Button).ToArray());
        }

        void Items_RemovedItems(object sender, ITab[] items, int startIndex)
        {
            XButtons.Items.RemoveRange(startIndex, items.Length);

            if (!Items.Contains(SelectItem))
                _selectTab = null;
        }


        // CONSTRUCTORS

        public TabsPanel()
        {
            InitializeComponent();

            Items.AddingItems += Items_AddingItems;
            Items.RemovedItems += Items_RemovedItems;

            XButtons.SelectionChanged += XButtons_SelectionChanged;
        }
    }
}
