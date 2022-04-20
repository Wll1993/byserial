using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BYSerial.Models;
using System.Windows.Forms;

namespace BYSerial.Views
{
    /// <summary>
    /// ScreenColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenColorPicker : Window
    {
        private MouseHook _mouseHook;      

        #region 光标设置
        /// <summary>
        /// 设置系统指针函数（用hcur替换id定义的光标）
        /// </summary>
        /// <param name="hcur"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [DllImport("User32.DLL")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        public const uint OCR_NORMAL = 32512;
        public const uint OCR_IBEAM = 32513;
        /// <summary>
        /// 查询或设置的系统级参数函数
        /// </summary>
        /// <param name="uiAction"></param>
        /// <param name="uiParam"></param>
        /// <param name="pvParam"></param>
        /// <param name="fWinIni"></param>
        /// <returns></returns>
        [DllImport("User32.DLL")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        public const uint SPI_SETCURSORS = 87;
        public const uint SPIF_SENDWININICHANGE = 2;
        #endregion
               
        public ScreenColorPicker()
        {
            InitializeComponent();
        }
        ScreenShot _shot;
        private void btnPick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mouseHook=new MouseHook();
                _mouseHook.OnMouseActivity += _mouseHook_OnMouseActivity;
                _mouseHook.Start();
                SetCursor();

                PrintScreen();
                System.Drawing.Bitmap bitmap=GetScreenImage();
                _shot = new ScreenShot();
                Screen[] screen=Screen.AllScreens;
                int width = 0;int height = 0;
                for(int i = 0; i < screen.Length; i++)
                {
                    width+=screen[i].WorkingArea.Width;
                    height+=screen[i].WorkingArea.Height;
                }
                _shot.Width = width;
                _shot.Height = height;
                _shot.Image.Source = BitmapToBitmapSource(bitmap);
                _shot.Show();
                this.Topmost = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #region 截图
        [DllImport("user32.dll")]

        static extern void keybd_event

      (

          byte bVk,// 虚拟键值  

          byte bScan,// 硬件扫描码  

          uint dwFlags,// 动作标识  

          IntPtr dwExtraInfo// 与键盘动作关联的辅加信息  

      );



        /// <summary>
        /// 模拟Print Screen键盘消息，截取全屏图片。
        /// </summary>

        public void PrintScreen()

        {

            keybd_event((byte)0x2c, 0, 0x0, IntPtr.Zero);//down

            System.Windows.Forms.Application.DoEvents();

            keybd_event((byte)0x2c, 0, 0x2, IntPtr.Zero);//up

            System.Windows.Forms.Application.DoEvents();

        }
        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource bitmapSource;
            try
            {
                if(bitmap == null) return null;
                IntPtr ip = bitmap.GetHbitmap();//从GDI+ Bitmap创建GDI位图对象                
                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                return bitmapSource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        /// <summary>
        /// 从剪贴板获取图片
        /// </summary>
        /// <returns></returns>

        private System.Drawing.Bitmap GetScreenImage()

        {

            //IDataObject newobject = null;

            System.Drawing.Bitmap NewBitmap = null;
            try

            {
                if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    NewBitmap = (System.Drawing.Bitmap)(System.Windows.Forms.Clipboard.GetImage().Clone());
                }
                return NewBitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        #endregion
        private void _mouseHook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if(e.Button == MouseButtons.None)
                {
                    SolidColorBrush brush = PickScreenColor.GetPixelColor(e.X, e.Y);
                    borderColor.Background = brush;
                    sbR.Value = brush.Color.R;
                    sbG.Value = brush.Color.G;
                    sbB.Value = brush.Color.B;
                    sbA.Value = brush.Color.A;
                    txtRGB.Text = $"{sbA.Value},{sbR.Value},{sbG.Value},{sbB.Value}";
                    txtHEX.Text = brush.ToString();
                    
                }
                else if(e.Button == MouseButtons.Left && e.Clicks==1)
                {
                    _mouseHook.Stop();
                    ResetMyCursor();
                    if(_shot!=null)
                    {
                        _shot.Close();
                        _shot = null;
                        GC.Collect();
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 设置鼠标样式
        /// </summary>
        private void SetCursor()
        {           
            System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursors.Hand;
            //此处可以写多个
            SetSystemCursor(cursor.Handle, OCR_NORMAL);
        }
        /// <summary>
        /// 恢复鼠标样式
        /// </summary>
        private void ResetMyCursor()
        {
            //恢复为系统默认图标
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
        }



        #region scrollbar
        private void sbB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetColor();
        }

        private void sbG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetColor();
        }

        private void sbR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetColor();
        }

        private void sbA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GetColor();
        }
        private void GetColor()
        {
            int a = (int)sbA.Value;
            int r = (int)sbR.Value;
            int g = (int)sbG.Value;
            int b = (int)sbB.Value;
            txtA.Text = a.ToString();
            txtR.Text = r.ToString();
            txtG.Text = g.ToString();
            txtB.Text = b.ToString();
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
            borderColor.Background = scb;
            txtRGB.Text = $"{a},{r},{g},{b}";
            txtHEX.Text = scb.ToString();
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_mouseHook!=null) _mouseHook.Stop();
        }
    }
}
