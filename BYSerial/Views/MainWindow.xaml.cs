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
using BYSerial.ViewModels;

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
        }

        private void txtRich_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRich.ScrollToEnd();
        }

        private void devPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            if(devPort.SelectedIndex==(devPort.Items.Count-1))
            {
                viewModel.IsSerialTest = Visibility.Collapsed;                
            }
            else
            {
                viewModel.IsSerialTest = Visibility.Visible;
            }
        }

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
        }

        private void CbbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            viewModel. AddHisToSendText();
        }
    }
}
