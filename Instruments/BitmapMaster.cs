using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CashApp.Instruments
{
    public static class BitmapMaster
    {
		public static BitmapSource Resize(BitmapSource source, int newWidth, int newHeigth)
        {
			return new TransformedBitmap(source, new ScaleTransform(newWidth / source.Width, newHeigth / source.Height));
		}

		public static byte[] SourceToBytes(BitmapSource source)
		{
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(source));

			MemoryStream ms = new MemoryStream();
			encoder.Save(ms);
			byte[] buff = ms.ToArray();
			ms.Close();

			return buff;
		}

		public static BitmapSource BytesToSource(byte[] source)
		{
			return BitmapFrame.Create(new MemoryStream(source));
		}
	}
}
