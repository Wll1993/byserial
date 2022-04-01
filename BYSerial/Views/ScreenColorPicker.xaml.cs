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

namespace BYSerial.Views
{
    /// <summary>
    /// ScreenColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenColorPicker : Window
    {
        public enum WindowsMessages
        {
            WM_LBUTTONDOWN=0x0201,
            WM_LBUTTONUP=0x0202,
            WM_MOUSEMOVE=0x0200,
            WM_MOUSEWHEEL=0x020A,
            WM_RBUTTONDOWN=0x0204,
            WM_RBUTTONUP=0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int x, y; };

        [StructLayout(LayoutKind.Sequential)]
        public struct LowLevelMouseHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }
       
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;            
            public uint time;
            public IntPtr dwExtraInfo;
        }
        public enum HookType : int
        {
            /// <summary>
            /// Installs a hook procedure that records input messages posted to the system message queue.
            /// This hook is useful for recording macros.
            /// For more information, see the JournalRecordProc hook procedure.
            /// </summary>
            WH_JOURNALRECORD = 0,
            /// <summary>
            /// Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure.
            /// For more information, see the JournalPlaybackProc hook procedure.
            /// </summary>
            WH_JOURNALPLAYBACK = 1,
            /// <summary>
            /// Installs a hook procedure that monitors keystroke messages.
            /// For more information, see the KeyboardProc hook procedure.
            /// </summary>
            WH_KEYBOARD = 2,
            /// <summary>
            /// Installs a hook procedure that monitors messages posted to a message queue.
            /// For more information, see the GetMsgProc hook procedure.
            /// </summary>
            WH_GETMESSAGE = 3,
            /// <summary>
            /// Installs a hook procedure that monitors messages
            /// before the system sends them to the destination window procedure.
            /// For more information, see the CallWndProc hook procedure.
            /// </summary>
            WH_CALLWNDPROC = 4,
            /// <summary>
            /// Installs a hook procedure that receives notifications useful to a computer-based training (CBT) application.
            /// For more information, see the CBTProc hook procedure.
            /// </summary>
            WH_CBT = 5,
            /// <summary>Installs a hook procedure that monitors messages generated
            /// as a result of an input event in a dialog box, message box, menu, or scroll bar.
            /// The hook procedure monitors these messages for all applications in the same desktop as the calling thread.
            /// For more information, see the SysMsgProc hook procedure.
            /// </summary>
            WH_SYSMSGFILTER = 6,
            /// <summary>
            /// Installs a hook procedure that monitors mouse messages.
            /// For more information, see the MouseProc hook procedure.
            /// </summary>
            WH_MOUSE = 7,
            /// <summary>
            ///
            /// </summary>
            WH_HARDWARE = 8,
            /// <summary>
            /// Installs a hook procedure useful for debugging other hook procedures.
            /// For more information, see the DebugProc hook procedure.
            /// </summary>
            WH_DEBUG = 9,
            /// <summary>
            /// Installs a hook procedure that receives notifications useful to shell applications.
            /// For more information, see the ShellProc hook procedure.
            /// </summary>
            WH_SHELL = 10,
            /// <summary>
            /// Installs a hook procedure that will be called when the application's foreground thread is about to become idle.
            /// This hook is useful for performing low priority tasks during idle time.
            /// For more information, see the ForegroundIdleProc hook
            /// </summary>
            WH_FOREGROUNDIDLE = 11,
            /// <summary>
            /// Installs a hook procedure that monitors messages
            /// after they have been processed by the destination window procedure.
            /// For more information, see the CallWndRetProc hook procedure.
            /// </summary>
            WH_CALLWNDPROCRET = 12,
            /// <summary>
            /// Windows NT/2000/XP:
            /// Installs a hook procedure that monitors low-level keyboard input events.
            /// For more information, see the LowLevelKeyboardProc hook
            /// </summary>
            WH_KEYBOARD_LL = 13,
            /// <summary>
            /// Windows NT/2000/XP:
            /// Installs a hook procedure that monitors low-level mouse input events.
            /// For more information, see the LowLevelMouseProc hook procedure.
            /// </summary>
            WH_MOUSE_LL = 14
        }
       
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        // overload for use with LowLevelMouseProc
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType code, LowLevelMouseProc func, IntPtr hInstance, int threadID);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

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

        private LowLevelMouseProc _proc;
        private IntPtr _hookID=IntPtr.Zero;
        private MSLLHOOKSTRUCT _hookStruct;
        
        public ScreenColorPicker()
        {
            InitializeComponent();
        }

        private void btnPick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _hookID= SetHook(_proc);
                SetCursor();
                //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using(Process curProcess=Process.GetCurrentProcess())
            {
                using (ProcessModule curModel =curProcess.MainModule)
                {
                    _proc = HookCallback;
                    return SetWindowsHookEx(HookType.WH_MOUSE_LL, _proc, GetModuleHandle(curModel.ModuleName), 0);
                }
            }
            
        }
        private IntPtr HookCallback(int nCode,IntPtr wParam,IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && WindowsMessages.WM_MOUSEMOVE == (WindowsMessages)wParam)
                {
                    _hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //释放
                    //Marshal.FreeCoTaskMem(lParam);
                    SolidColorBrush brush = PickScreenColor.GetPixelColor(_hookStruct.pt.x, _hookStruct.pt.y);
                    borderColor.Background = brush;
                    sbR.Value = brush.Color.R;
                    sbG.Value = brush.Color.G;
                    sbB.Value = brush.Color.B;
                    sbA.Value = brush.Color.A;
                    txtRGB.Text = $"{sbA.Value},{sbR.Value},{sbG.Value},{sbB.Value}";
                    txtHEX.Text = brush.ToString();
                }
                if (nCode >= 0 && WindowsMessages.WM_RBUTTONDOWN == (WindowsMessages)wParam)
                {
                    UnhookWindowsHookEx(_hookID);
                    ResetMyCursor();
                    //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return CallNextHookEx(_hookID,nCode,wParam,lParam);
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
            UnhookWindowsHookEx(_hookID);
        }
    }
}
