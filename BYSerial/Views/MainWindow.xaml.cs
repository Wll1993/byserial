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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BYSerial.Util;
using BYSerial.ViewModels;
using BYSerial.Models;

namespace BYSerial.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _IsLoaded = false;
        private MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.SendPara.IsLoop=false;
            GlobalPara.SaveCurCfg();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void txtRich_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRich.ScrollToEnd();
        }

        //private void devPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!_IsLoaded) return;
        //    if(devPort.SelectedIndex==(devPort.Items.Count-1))
        //    {
        //        viewModel.IsSerialTest = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        viewModel.IsSerialTest = Visibility.Visible;
        //    }
        //}

        private void tcpMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            try
            {
                if (tcpMode.SelectedIndex == 0)
                {
                    viewModel.TcpPara.IsTcpClient = Visibility.Visible;
                }
                else
                {
                    viewModel.TcpPara.IsTcpClient = Visibility.Collapsed;
                }
                if (viewModel.IsStopCan)
                {
                    viewModel.OnStopCommand.ExecuteAction(null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _IsLoaded=true;
            try
            {                
                if(GlobalPara.IsCheckUpdate)
                {                    
                    App.Current.Dispatcher.BeginInvoke(new Action(() => {
                        int i = 0;
                        if (Util.Update.InternetGetConnectedState(out i, 0))
                        {
                            BYSerial.Util.Update.CheckUpdate();
                        }
                    }));
                   
                }               
                this.Title+= "_V:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        private void CbbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            viewModel. AddHisToSendText();
        }
        
       

        private void Button_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                int id = viewModel.CurSelectedFastCmdModel.RowID;
                string caption = "";
                if (btn.Content != null)
                {
                    caption = btn.Content.ToString();
                }
                FastCmdSet fcs = new FastCmdSet(viewModel.CurSelectedFastCmdModel);
                bool bret = (bool)fcs.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"错误提示");
            }
        }
    }
}
