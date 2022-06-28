using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace CashApp.UIElements
{
	public partial class ColorPicker : Border
	{
		// FIELDS

		bool isPressed = false;

		public new Brush Background
        {
			get => base.Background;
			set
            {
				base.Background = value;
				ColorChanged?.Invoke(this, new EventArgs());
            }
        }


		// EVENTS

		public event EventHandler ColorChanged;


		// METHODS

		void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			isPressed = false;
			BorderThickness = new Thickness(1);
		}
		void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isPressed = true;
			BorderThickness = new Thickness(2);
		}
		void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!isPressed) return;

			BorderThickness = new Thickness(1);

			ColorDialog dialog = new ColorDialog();

			if (dialog.ShowDialog() != DialogResult.OK) return;

			Background = new SolidColorBrush(Color.FromRgb(
				dialog.Color.R,
				dialog.Color.G,
				dialog.Color.B
			));

			ColorChanged?.Invoke(this, new EventArgs());
		}


		// CONSTRUCTORS

		public ColorPicker()
		{
			InitializeComponent();
		}
    }
}
