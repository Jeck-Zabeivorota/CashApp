using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using CashApp.Instruments;

namespace CashApp.UIElements
{
    [ContentProperty("Items")]
	public partial class DropButton : UserControl
	{
		// FIELDS

		readonly TextBlock XSelectItem, XArrow;


		public readonly TrackList<TextBlock> Items = new TrackList<TextBlock>();

		public string SelectItem
		{
			get => XSelectItem.Text;
			set
			{
				_selectIndex = Items.FindIndex(item => item.Text == value);

				if (_selectIndex == -1) throw new ArgumentException("Item is not exists");

				XSelectItem.Text = value;

				SelectionChanged?.Invoke(value, new EventArgs());
			}
		}

		int _selectIndex = -1;
		public int SelectIndex
		{
			get => _selectIndex;
			set
            {
				if (value < 0 | value >= Items.Count) throw new IndexOutOfRangeException(value.ToString());

				_selectIndex = value;
				XSelectItem.Text = Items[value].Text;

				SelectionChanged?.Invoke(SelectItem, new EventArgs());
			}
		}

		public CornerRadius CornerRadius
        {
			get => XBody.CornerRadius;
			set => XBody.CornerRadius = value;
        }

		public new Brush Background
		{
			get => XBody.Background;
			set => XBody.Background = value;
		}

		public new double Width
        {
			get => base.Width;
			set
            {
				base.Width = value;
				XDropMenu.Width = Width;
			}
        }

		public double ItemHeight
        {
			get => XDropMenu.ItemHeight;
			set => XDropMenu.ItemHeight = value;
        }


		// EVENTS

		public event EventHandler SelectionChanged;


		// METHODS

		void DropList_Opened(object sender, EventArgs e)
		{
			CornerRadius radius = XBody.CornerRadius;
			radius.BottomLeft = 0;
			radius.BottomRight = 0;
			XBody.CornerRadius = radius;

			XArrow.Text = "˄";

			XBody.Background = Colors.Accent3;
		}

		void DropList_Closed(object sender, EventArgs e)
		{
			CornerRadius radius = XBody.CornerRadius;
			radius.BottomLeft = radius.TopLeft;
			radius.BottomRight = radius.TopRight;
			XBody.CornerRadius = radius;

			XArrow.Text = "˅";

			XBody.Background = Colors.Accent1;
		}


		void Item_Selected(object sender, EventArgs e)
		{
			SelectItem = (sender as TextBlock).Text;
			SelectionChanged?.Invoke(sender, new EventArgs());
		}

		void Items_AddingItems(object sender, TextBlock[] items, int startIndex)
		{
			XDropMenu.Items.InsertRange(startIndex, items);
			
			if (SelectItem == "") SelectItem = items[0].Text;
		}

		void Items_RemovedItems(object sender, TextBlock[] items, int startIndex)
		{
			XDropMenu.Items.RemoveRange(startIndex, items.Length);

			TextBlock select = Items.Find(item => item.Text == SelectItem);

			if (select == null) XSelectItem.Text = null;
		}


		// CONSTRUCTORS

		public DropButton()
		{
			InitializeComponent();

			Grid grid = (Grid)XBody.Child;

			XSelectItem = (TextBlock)grid.Children[0];
			XArrow = (TextBlock)grid.Children[1];


            Items.AddingItems += Items_AddingItems;
            Items.RemovedItems += Items_RemovedItems;

			XDropMenu.PlacementTarget = XBody;
			XBody.Click += (sender, e) => XDropMenu.IsOpen = true;
			XDropMenu.SelectionChanged += Item_Selected;
		}
    }
}
