using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;

namespace BYSerial.Models
{
    public class PickScreenColor
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static SolidColorBrush GetPixelColor(int x,int y)
        {
            SolidColorBrush brush = Brushes.Transparent;
            IntPtr hdc=GetDC(IntPtr.Zero);
            uint pixel=GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                    (byte)((pixel & 0x0000FF00) >> 8),
                    (byte)((pixel & 0x00FF0000) >> 16));
            brush=new SolidColorBrush(color);
            return brush;
        }

    }
}
