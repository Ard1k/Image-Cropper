/*
    This program will crop images in half and save both sides.
    Can crop all images in the same directory where the .exe file is executed
    Or crop the images droped on the .exe file...
    If the images width are at least or greater than the width of "minImageWidth" and its extensions are in the "fileExtensions" list.
    If no images dropped the program asks if you want to delete the original files.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Image_Cropper
{
  class Program
  {
    static void Main(string[] args)
    {
      //Source images file extension allowed.
      List<string> fileExtensionsList = new List<string> { ".png", ".jpg" };
      string[] imageFiles;

      Console.WriteLine("<<< IMAGE CROPPER TOOL >>>\n");
      Console.Write("\n\n");

      // Directory path where the .exe file is executed.
      //string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      string dir = @"D:\@Git\lsrp\image-server\data\uploads\vehicles";

      // Get all subdirectories
      string[] subdirectories = Directory.GetDirectories(dir);

      foreach (string subdir in subdirectories)
      {
        Console.WriteLine($"Subfolder: " + Path.GetFileName(subdir));

        imageFiles = Directory.GetFiles(Path.Combine(dir, subdir), "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => fileExtensionsList.Contains(Path.GetExtension(s.ToLower()))).ToArray();

        //Crop Images
        CropImagesCenter(imageFiles, dir, subdir);
      }

      Console.WriteLine("\n\nAll images cropped!\n");
      Console.Read();

      void CropImagesCenter(string[] images, string currentDir, string subFolder)
      {
        if (images.Length > 0)
        {
          Console.Write("\n");

          foreach (string img in images)
          {
            //Fast way to check image sizes.
            using (Stream stream = File.OpenRead(img))
            {
              using (Image sourceImage = Image.FromStream(stream, false, false))
              {
                CropImage(img, currentDir, subFolder);
              }
            }
          }
        }
      }

      void CropImage(string image, string currentDir, string subFolder)
      {
        //Creates a bitmap image from source image.
        Bitmap srcImg = Image.FromFile(image) as Bitmap;

        Rectangle cropRect = new Rectangle(srcImg.Size.Width/4, srcImg.Size.Height/4, srcImg.Size.Width /2, srcImg.Size.Height /2);
        Bitmap targetImg = new Bitmap(cropRect.Width, cropRect.Height);

        //Crop left side of the image.
        using (Graphics g = Graphics.FromImage(targetImg))
        {
          g.DrawImage(srcImg, new Rectangle(0, 0, targetImg.Width, targetImg.Height),
                    cropRect,
                    GraphicsUnit.Pixel);
        }

        var savePath = Path.Combine(dir, "@cropped", Path.GetFileName(subFolder));
        Directory.CreateDirectory(savePath);
        targetImg.Save(Path.Combine(savePath, "medium_" + Path.GetFileName(image)), ImageFormat.Png);

        var scaledImg = ScaleBitmap(targetImg, 0.5f);
        scaledImg.Save(Path.Combine(savePath, "mini_" + Path.GetFileName(image)), ImageFormat.Png);

        Console.WriteLine(Path.GetFileName(image) + " Cropped!");

        //Release the bitmap image used.
        srcImg.Dispose();
      }

      Bitmap ScaleBitmap(Bitmap original, float scale)
      {
        int newWidth = (int)(original.Width * scale);
        int newHeight = (int)(original.Height * scale);
        Bitmap resized = new Bitmap(newWidth, newHeight);

        using (Graphics g = Graphics.FromImage(resized))
        {
          g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          g.DrawImage(original, 0, 0, newWidth, newHeight);
        }

        return resized;
      }

    }
  }
}