using Spire.Barcode;
using System;
using System.Drawing;
using System.IO;
namespace Reusable.Barcode
{
    public static class Barcoder
    {
        public static string Generate_Code128(string value)
        {
            BarCodeGenerator generator = new BarCodeGenerator(new BarcodeSettings()
            {
                Data = value,
                Type = BarCodeType.Code128,
                Data2D = value,
                ShowText = false,
                TopText = value,
                BarHeight = 15
            });
            try
            {
                Image barcode = generator.GenerateImage();
                Image cropped = CropAtRect(barcode, new Rectangle()
                {
                    Height = 60,
                    Width = barcode.Width,
                    X = 0,
                    Y = 0,
                    Location = new Point()
                    {
                        X = 0,
                        Y = 17
                    }
                });
                //I will crop at frontend (so, using barcode instead of cropped)
                return Convert.ToBase64String(ConvertBitMapToByteArray(barcode));
            }
            catch (Exception e)
            {
                return "";
            }
        }
        private static byte[] ConvertBitMapToByteArray(Image image)
        {
            byte[] result = null;
            if (image != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    result = stream.ToArray();
                }
            }
            return result;
        }
        //Crop Image
        public static Image CropAtRect(this Image b, Rectangle r)
        {
            Image nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }
    }
}
