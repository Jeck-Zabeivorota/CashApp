using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CashApp.UIElements
{
    public partial class ImageSelector : Border
    {
		// FIELDS

		bool isPressed = false;

		public ImageSource Source
        {
			get => XImage.Source;
			set
			{
				XImage.Source = value;
				ImageChanged?.Invoke(this, new EventArgs());
			}
        }


		// EVENTS

		public event EventHandler ImageChanged;


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

			var dialog = new OpenFileDialog();

			dialog.Filter = "Image Files(*.PNG;*.JPG;*.BMP)|*.PNG;*.JPG;*.BMP|All files (*.*)|*.*";

			if (dialog.ShowDialog() == DialogResult.OK)
            {
				XImage.Source = new BitmapImage(new Uri(dialog.FileName));
				ImageChanged?.Invoke(this, new EventArgs());
			}
		}


		public void ImageFromPath(string path)
        {
			if (!File.Exists(path)) throw new FileNotFoundException(path);

			Source = new BitmapImage(new Uri(path));
		}


		// CONSTRUCTORS

		public ImageSelector()
        {
            InitializeComponent();
        }
    }
}
