using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using CashApp.Instruments;

namespace CashApp.UIElements
{
    [ContentProperty("Items")]
    public partial class ChoiceChips : Border
    {
        // FIELDS

        public StackPanel XItems;

        public readonly TrackList<UIElement> Items = new TrackList<UIElement>();


        Border _selectItemBorder;
        public UIElement SelectItem
        {
            get => _selectItemBorder?.Child;
            set
            {
                if (_selectItemBorder != null)
                {
                    if (_selectItemBorder.Child == value) return;
                    _selectItemBorder.Background = Brushes.Transparent;
                }

                if (value == null)
                {
                    _selectItemBorder = null;
                    _selectIndex = -1;
                    return;
                }


                if (!Items.Contains(value)) throw new ArgumentException("Item is not exists");

                _selectIndex = ItemsIndexOf(value);
                _selectItemBorder = (Border)XItems.Children[_selectIndex];

                _selectItemBorder.Background = SelectColor;

                SelectionChanged?.Invoke(this, value);
            }
        }


        int _selectIndex = -1;
        public int SelectIndex
        {
            get => _selectIndex;
            set
            {
                if (value != -1)
                    SelectItem = Items[value];
                else
                    SelectItem = null;
            }
        }


        Brush _selectColor;
        public Brush SelectColor
        {
            get => _selectColor ?? Colors.Accent3;
            set => _selectColor = value;
        }


        double _itemHeight = 20;
        public double ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;

                foreach (Border item in XItems.Children)
                    item.Height = value;
            }
        }


        public new CornerRadius CornerRadius
        {
            get => base.CornerRadius;
            set
            {
                base.CornerRadius = value;

                foreach (Border button in XItems.Children)
                    button.CornerRadius = value;
            }
        }


        // EVENTS

        public delegate void SelectionChangedHandler(object sender, UIElement item);
        public event SelectionChangedHandler SelectionChanged;


        // METHODS

        int ItemsIndexOf(UIElement item)
        {
            int i = 0;
            foreach (Border border in XItems.Children)
            {
                if (border.Child == item)
                    return i;
                i++;
            }

            return -1;
        }

        Border CreateItemBorder(UIElement item)
        {
            Border button = new Border
            {
                Child = item,
                Height = ItemHeight,
                Background = Brushes.Transparent,
                CornerRadius = CornerRadius
            };
            button.MouseUp += (sender, e) => SelectItem = ((Border)sender).Child;

            return button;
        }

        void Items_AddingItems(object sender, UIElement[] items, int startIndex)
        {
            UIElement[] lasts = new UIElement[XItems.Children.Count - startIndex];

            for (int i = 0; i < lasts.Length; i++)
                lasts[0] = XItems.Children[startIndex + i];

            XItems.Children.RemoveRange(startIndex, lasts.Length);


            foreach (UIElement item in items)
                XItems.Children.Add(CreateItemBorder(item));

            foreach (UIElement item in lasts)
                XItems.Children.Add(item);


            if (SelectItem == null)
                SelectItem = ((Border)XItems.Children[0]).Child;
        }

        void Items_RemovedItems(object sender, UIElement[] items, int startIndex)
        {
            XItems.Children.RemoveRange(startIndex, items.Length);

            if (!Items.Contains(SelectItem))
            {
                _selectItemBorder = null;
                _selectIndex = -1;
            }
        }


        // CONSTRUCTORS

        public ChoiceChips()
        {
            InitializeComponent();

            XItems = (StackPanel)_XButtons_.Content;

            Items.AddingItems += Items_AddingItems;
            Items.RemovedItems += Items_RemovedItems;
        }
    }
}
