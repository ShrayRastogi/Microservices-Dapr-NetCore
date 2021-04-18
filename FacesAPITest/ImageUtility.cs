using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesAPITest
{
    public class ImageUtility
    {
        public static byte[] ConvertToBytes(string imagePath)
        {
            MemoryStream ms = new();
            using(FileStream stream = new(imagePath, FileMode.Open))
            {
                stream.CopyTo(ms);
            }
            var bytes = ms.ToArray();
            return bytes;
        }

        public static void FromBytesToArray(byte[] bytes, string fileName)
        {
            using var ms = new MemoryStream(bytes);
            Image img = Image.Load(ms.ToArray());
            img.Save(fileName + ".jpg");
        }
    }
}
