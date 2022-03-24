using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BYSerial.Util
{
    public class BitmapWPF
    {

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }


        // BitmapImage --> Bitmap
        public static System.Drawing.Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
        }

        // RenderTargetBitmap --> BitmapImage
        public static BitmapImage ConvertRenderTargetBitmapToBitmapImage(RenderTargetBitmap wbm)
        {
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bmp.StreamSource = new MemoryStream(stream.ToArray()); //stream;
                bmp.EndInit();
                bmp.Freeze();
            }
            return bmp;
        }

        // RenderTargetBitmap --> BitmapImage
        public static BitmapImage RenderTargetBitmapToBitmapImage(RenderTargetBitmap rtb)
        {
            var renderTargetBitmap = rtb;
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
        // ImageSource --> Bitmap
        public static System.Drawing.Bitmap ImageSourceToBitmap(System.Windows.Media.ImageSource imageSource)
        {
            BitmapSource m = (BitmapSource)imageSource;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb); // 坑点：选Format32bppRgb将不带透明度

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }
        // BitmapImage --> byte[]
        public static byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] bytearray = null;
            try
            {
                Stream smarket = bmp.StreamSource; ;
                if (smarket != null && smarket.Length > 0)
                {
                    //设置当前位置
                    smarket.Position = 0;
                    using (BinaryReader br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return bytearray;
        }


        // byte[] --> BitmapImage
        public static BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
        public static System.Drawing.Bitmap ConvertByteArrayToBitmap(byte[] bytes)
        {
            System.Drawing.Bitmap img = null;
            try
            {
                if (bytes != null && bytes.Length != 0)
                {
                    MemoryStream ms = new MemoryStream(bytes);
                    img = new System.Drawing.Bitmap(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return img;
        }
        /// <summary>
        /// bitmap转WPFimg,使用png图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="bitmapimg"></param>
        /// <returns></returns>
        public static bool BitmapToBitmapImage(System.Drawing.Bitmap bitmap, out System.Windows.Media.Imaging.BitmapImage bitmapimg)
        {
            System.Windows.Media.Imaging.BitmapImage bitmapImage;

            try
            {

                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                    stream.Position = 0;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                    // Force the bitmap to load right now so we can dispose the stream.
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                }
                bitmapimg = bitmapImage;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public static bool BitmapToBitmapSource(System.Drawing.Bitmap bitmap, out System.Windows.Media.Imaging.BitmapSource bitmapSource)
        {
            try
            {
                IntPtr ip = bitmap.GetHbitmap();//从GDI+ Bitmap创建GDI位图对象                
                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool BitmapSourceToBitmap(BitmapSource bitmapSource, out System.Drawing.Bitmap bitmap)
        {
            try
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                bitmapSource.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);

                bitmap = bmp;
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将本地图片文件直接读出为字节数组
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="imgbytes"></param>
        /// <returns></returns>
        public static bool BitmapFileToBytes(string filename, out byte[] imgbytes)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    imgbytes = null;
                    return false;
                }
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] pic_byte = new byte[fs.Length];
                fs.Read(pic_byte, 0, pic_byte.Length);
                fs.Close();
                imgbytes = pic_byte;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>  
        /// 将源图像灰度化，并转化为8位灰度图像。  
        /// </summary>  
        /// <param name="original"> 源图像。 </param>  
        /// <returns> 8位灰度图像。 </returns>  
        public static System.Drawing.Bitmap RgbToGrayScale(System.Drawing.Bitmap original)
        {
            if (original != null)
            {
                // 将源图像内存区域锁定  
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, original.Width, original.Height);
                System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                     original.PixelFormat);

                // 获取图像参数  
                int width = bmpData.Width;
                int height = bmpData.Height;
                int stride = bmpData.Stride;  // 扫描线的宽度  
                int offset = stride - width * 3;  // 显示宽度与扫描线宽度的间隙  
                IntPtr ptr = bmpData.Scan0;   // 获取bmpData的内存起始位置  
                int scanBytes = stride * height;  // 用stride宽度，表示这是内存区域的大小  

                // 分别设置两个位置指针，指向源数组和目标数组  
                int posScan = 0, posDst = 0;
                byte[] rgbValues = new byte[scanBytes];  // 为目标数组分配内存  
                Marshal.Copy(ptr, rgbValues, 0, scanBytes);  // 将图像数据拷贝到rgbValues中  
                                                             // 分配灰度数组  
                byte[] grayValues = new byte[width * height]; // 不含未用空间。  
                                                              // 计算灰度数组  
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        double temp = rgbValues[posScan++] * 0.11 +
                            rgbValues[posScan++] * 0.59 +
                            rgbValues[posScan++] * 0.3;
                        grayValues[posDst++] = (byte)temp;
                    }
                    // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel  
                    posScan += offset;
                }

                // 内存解锁  
                Marshal.Copy(rgbValues, 0, ptr, scanBytes);
                original.UnlockBits(bmpData);  // 解锁内存区域  

                // 构建8位灰度位图  
                System.Drawing.Bitmap retBitmap = BuiltGrayBitmap(grayValues, width, height);
                return retBitmap;
            }
            else
            {
                return null;
            }
        }

        /// <summary>  
        /// 用灰度数组新建一个8位灰度图像。  
        /// 
        /// </summary>  
        /// <param name="rawValues"> 灰度数组(length = width * height)。 </param>  
        /// <param name="width"> 图像宽度。 </param>  
        /// <param name="height"> 图像高度。 </param>  
        /// <returns> 新建的8位灰度位图。 </returns>  
        private static System.Drawing.Bitmap BuiltGrayBitmap(byte[] rawValues, int width, int height)
        {
            // 新建一个8位灰度位图，并锁定内存区域操作  
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
                 System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            // 计算图像参数  
            int offset = bmpData.Stride - bmpData.Width;        // 计算每行未用空间字节数  
            IntPtr ptr = bmpData.Scan0;                         // 获取首地址  
            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度  
            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存  

            // 为图像数据赋值  
            int posSrc = 0, posScan = 0;                        // rawValues和grayValues的索引  
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grayValues[posScan++] = rawValues[posSrc++];
                }
                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel  
                posScan += offset;
            }

            // 内存解锁  
            Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpData);  // 解锁内存区域  

            // 修改生成位图的索引表，从伪彩修改为灰度  
            System.Drawing.Imaging.ColorPalette palette;
            // 获取一个Format8bppIndexed格式图像的Palette对象  
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
            {
                palette = bmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            }
            // 修改生成位图的索引表  
            bitmap.Palette = palette;

            return bitmap;
        }
        /// <summary>
        /// 将base64图片转为Image
        /// （Image包含bitmap和矢量图）
        /// </summary>
        /// <param name="imageStringBase64"></param>
        /// <returns>转换失败时，返回null</returns>
        public static Bitmap GetBitmapFromBase64String(string imageStringBase64)
        {
            Bitmap image = null;
            try
            {
                // 判断是否为空，为空时的不执行
                if (!string.IsNullOrEmpty(imageStringBase64))
                {
                    // 直接返Base64码转成数组
                    byte[] imageBytes = Convert.FromBase64String(imageStringBase64);
                    // 读入MemoryStream对象
                    MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
                    memoryStream.Write(imageBytes, 0, imageBytes.Length);
                    // 转成图片
                    image = (Bitmap)Bitmap.FromStream(memoryStream);
                    memoryStream.Flush();
                    memoryStream.Close();  //释放内存
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return image;
        }
        /// <summary>
        /// 获取图片的Base64编码字符串
        /// 支持图片格式：jpg,jpeg,bmp,png,gif,
        /// </summary>
        /// <param name="imagepath">图片路径</param>
        /// <returns></returns>
        public static string GetBase64String(string imagepath)
        {
            string result = "";
            try
            {
                Bitmap bmp = new Bitmap(imagepath);
                MemoryStream ms = new MemoryStream();
                string imagetype = Path.GetExtension(imagepath).ToLower();  //获取文件扩展名
                //(*.jpg;*.bmp;*png)|*.jpeg;*.jpg;*.bmp;*.png|AllFiles(*.*)|*.*";
                if (imagetype == ".jpg" || imagetype == ".jpeg")
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                }
                else if (imagetype == ".bmp")
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                }
                else if (imagetype == ".png")
                {
                    bmp.Save(ms, ImageFormat.Png);
                }
                else if (imagetype == ".gif")
                {
                    bmp.Save(ms, ImageFormat.Gif);
                }
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                // 直接返这个值放到数据就行了
                result = Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 获取图片的Base64编码字符串
        /// 支持图片格式：jpg,jpeg,bmp,png,gif,
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="imagetype">图片类型</param>
        /// <returns></returns>
        public static string GetBase64String(Bitmap bitmap, ImageFormat imagetype)
        {
            string result = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                //(*.jpg;*.bmp;*png)|*.jpeg;*.jpg;*.bmp;*.png|AllFiles(*.*)|*.*";
                if (imagetype == ImageFormat.Jpeg)
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                }
                else if (imagetype == ImageFormat.Bmp)
                {
                    bitmap.Save(ms, ImageFormat.Bmp);
                }
                else if (imagetype == ImageFormat.Png)
                {
                    bitmap.Save(ms, ImageFormat.Png);
                }
                else if (imagetype == ImageFormat.Gif)
                {
                    bitmap.Save(ms, ImageFormat.Gif);
                }
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                // 直接返这个值放到数据就行了
                result = Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static BitmapSource GetBitmapSource(string imgpath)
        {
            BitmapSource bmp = null;
            try
            {
                FileStream fs = new FileStream(imgpath, FileMode.Open, FileAccess.Read);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fs);
                fs.Close();
                BitmapToBitmapSource(bitmap, out bmp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bmp;
        }
    }
}
