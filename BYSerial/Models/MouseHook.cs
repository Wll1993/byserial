using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BYSerial.Models
{
    /// <summary>
    /// 鼠标全局钩子
    /// </summary>
    public class MouseHook
    {
        private enum HookType
        {
            WH_CALLWNDPROC = 4, //安装在系统将消息发送到目标窗口过程之前监视邮件的挂钩过程。
            WH_CALLWNDPROCRET = 12, //安装在目标窗口过程处理完邮件后监视消息的挂钩过程。
            WH_CBT = 5, //安装接收对 CBT 应用程序有用的通知的挂钩过程。
            WH_DEBUG = 9, //安装用于调试其他挂钩过程的挂钩过程。
            WH_FOREGROUNDIDLE = 11, //安装将在应用程序的前景线程即将变为空闲时调用的挂钩过程。此挂钩对于在空闲时间执行低优先级任务非常有用。
            WH_GETMESSAGE = 3, //安装用于监视张贴到消息队列的邮件的挂钩过程。
            WH_HARDWARE = 8, //安装一个挂接过程, 用于张贴以前由 WH_JOURNALRECORD 挂钩过程记录的消息。
            WH_JOURNALPLAYBACK = 1, //安装一个挂接过程, 用于张贴以前由 WH_JOURNALRECORD 挂钩过程记录的消息。
            WH_JOURNALRECORD = 0,//安装用于记录张贴到系统消息队列中的输入消息的挂钩过程。
            WH_KEYBOARD = 2,//安装监视击键消息的挂钩过程。
            WH_MOUSE = 7,//安装监视鼠标消息的挂钩过程。
            WH_MSGFILTER = (-1), //安装一个钩子过程, 用于监视对话框、消息框、菜单或滚动条中由于输入事件而生成的消息。
            WH_SHELL = 10,//安装接收对 shell 应用程序有用的通知的挂钩过程。
            WH_SYSMSGFILTER = 6,//安装一个钩子过程, 用于监视对话框、消息框、菜单或滚动条中由于输入事件而生成的消息。挂钩过程监视与调用线程在同一桌面上的所有应用程序的这些消息。
            WH_KEYBOARD_LL = 13,//安装用于监视低级键盘输入事件的挂钩过程。
            WH_MOUSE_LL = 14,//安装用于监视低级别鼠标输入事件的挂钩过程。
        }
      
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;

       
        /// <summary>
        /// 点
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public const int WH_MOUSE_LL = 14; // mouse hook constant

        // 装置钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


        // 全局的鼠标事件
        public event MouseEventHandler OnMouseActivity;

        // 钩子回调函数
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        // 声明鼠标钩子事件类型
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄

        /// <summary>
        /// 构造函数
        /// </summary>
        public MouseHook()
        {

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MouseHook()
        {
            Stop();
        }

        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void Start() //ProcessKeyHandle clientMethod
        {
            //客户端委托事件
           // _clientMethod= clientMethod;
            // 安装鼠标钩子
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.
                _mouseHookProcedure = new HookProc(MouseHookProc);

                _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);

                //假设装置失败停止钩子
                if (_hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void Stop()
        {
            bool retMouse = true;

            if (_hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
            }

            // 假设卸下钩子失败
            if (!(retMouse))
                throw new Exception("UnhookWindowsHookEx failed.");
        }

        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 假设正常执行而且用户要监听鼠标的消息
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;

                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case WM_MOUSEMOVE:
                        button = MouseButtons.None;
                        clickCount = 0;
                        break;
                }

                // 从回调函数中得到鼠标的信息
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);

                // 假设想要限制鼠标在屏幕中的移动区域能够在此处设置
                // 后期须要考虑实际的x、y的容差
                if (!Screen.PrimaryScreen.Bounds.Contains(e.X, e.Y))
                {
                    //return 1;
                }
                OnMouseActivity(this, e);
                if (wParam == WM_RBUTTONUP) return 1;
            }

            // 启动下一次钩子
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }
    }
}
