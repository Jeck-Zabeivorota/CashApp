using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

using CashApp.Instruments;

namespace CashApp.UIElements
{
    [ContentProperty("Items")]
	public partial class DropMenu : Popup
	{
		// FIELDS

		readonly StackPanel XItems;

		public readonly TrackList<UIElement> Items = new TrackList<UIElement>();


		public UIElement SelectItem { get; private set; }
		public int SelectIndex { get; private set; } = -1;


		double _itemHeight = 22;
		public double ItemHeight
		{
			get => _itemHeight;
			set
			{
				_itemHeight = value;

				foreach (FlatButton item in XItems.Children) item.Height = value;

				_XItems_.MaxHeight = value * MaxVisibleItemCount;
			}
		}


		int _maxVisibleItemCount;
		public int MaxVisibleItemCount
		{
			get => _maxVisibleItemCount;
			set
			{
				_maxVisibleItemCount = value;
				_XItems_.MaxHeight = ItemHeight * value;
			}
		}

		public CornerRadius CornerRadius
		{
			get => XBody.CornerRadius;
			set => XBody.CornerRadius = value;
		}


		// EVENTS

		public event EventHandler SelectionChanged;


		// METHODS

		public void AttachSubmenu(UIElement item, DropMenu submenu)
		{
			int index = Items.FindIndex(i => i == item);

			if (index == -1) throw new ArgumentException("Item is not found");

			submenu.PlacementTarget = XItems.Children[index];
			submenu.Placement = PlacementMode.Right;

			DropMenu _submenu = submenu;

			XItems.Children[index].MouseEnter += (sender, e) => { _submenu.IsOpen = false; _submenu.IsOpen = true; };
			submenu.XBody.MouseLeave += (sender, e) => _submenu.IsOpen = false;
		}


		void Item_Click(object sender, EventArgs e)
		{
			UIElement item = (sender as FlatButton).Child;

			SelectItem = item;
			SelectIndex = Items.FindIndex(i => i == item);

			IsOpen = false;

			SelectionChanged?.Invoke(item, new EventArgs());
		}

		FlatButton CreateItemButton(FrameworkElement item)
		{
			item.VerticalAlignment = VerticalAlignment.Center;
			item.Margin = new Thickness { Left = 10 };

			FlatButton button = new FlatButton
			{
				Child = item,
				CornerRadius = new CornerRadius(),
				Height = ItemHeight
			};
			button.Click += Item_Click;

			return button;
		}


		void RoundedItems(bool round)
		{
			if (XItems.Children.Count == 0) return;


			int lastItemIdx = XItems.Children.Count - 1;
			CornerRadius radius = round ? CornerRadius : new CornerRadius();


			(XItems.Children[0] as FlatButton).CornerRadius = new CornerRadius
			{
				TopLeft = radius.TopLeft,
				TopRight = radius.TopRight
			};

			(XItems.Children[lastItemIdx] as FlatButton).CornerRadius = new CornerRadius
			{
				BottomLeft = radius.BottomLeft,
				BottomRight = radius.BottomRight
			};
		}

		void SetWidth()
        {
			double newWidth = Width;

			foreach (FlatButton item in XItems.Children)
				if (item.ActualWidth > newWidth) newWidth = item.ActualWidth;

			Width = newWidth;
        }

		void Items_AddingItems(object sender, UIElement[] items, int startIndex)
		{
			RoundedItems(false);


			UIElement[] lasts = new UIElement[XItems.Children.Count - startIndex];

			for (int i = 0; i < lasts.Length; i++)
				lasts[0] = XItems.Children[startIndex + i];

			XItems.Children.RemoveRange(startIndex, lasts.Length);


			foreach (UIElement item in items)
				XItems.Children.Add(CreateItemButton((FrameworkElement)item));

			foreach (UIElement item in lasts)
				XItems.Children.Add(item);


			RoundedItems(true);
		}

		void Items_RemovedItems(object sender, UIElement[] startItems, int index)
		{
			RoundedItems(false);

			XItems.Children.RemoveRange(index, startItems.Length);

			RoundedItems(true);


			if (!Items.Contains(SelectItem))
			{
				SelectItem = null;
				SelectIndex = -1;
			}
		}


		// CONSTRUCTORS

		public DropMenu()
		{
			InitializeComponent();

			XItems = (StackPanel)_XItems_.Content;

			MaxVisibleItemCount = 10;

			Items.AddingItems += Items_AddingItems;
			Items.RemovedItems += Items_RemovedItems;
		}
	}
}
