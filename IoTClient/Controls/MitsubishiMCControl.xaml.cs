using IoTClient.Clients.Modbus;
using IoTClient.Clients.PLC;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Models;
using IoTClientDeskTop.Common;
using IoTServer.Common;
using IoTServer.Servers.Modbus;
using IoTServer.Servers.PLC;
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

namespace IoTClientDeskTop.Controls
{
    /// <summary>
    /// MitsubishiMCControl.xaml 的交互逻辑
    /// </summary>
    public partial class MitsubishiMCControl : UserControl
    {
        private MitsubishiClient client;
        private IIoTServer server;
        private MitsubishiVersion version;
        public MitsubishiMCControl(MitsubishiVersion version)
        {
            InitializeComponent();
            btnRead.IsEnabled = false;
            btnWrite.IsEnabled = false;
            btnSendData.IsEnabled = false;
            this.version = version;
            var config = ConnectionConfig.GetConfig();
            switch (version)
            {
                case MitsubishiVersion.A_1E:
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiA1E_IP)) txt_ip.Text = config.MitsubishiA1E_IP;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiA1E_Port)) txt_port.Text = config.MitsubishiA1E_Port;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiA1E_Address)) txt_address.Text = config.MitsubishiA1E_Address;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiA1E_Value)) txt_value.Text = config.MitsubishiA1E_Value;
                    chbShowPackage.IsEnabled = config.MitsubishiA1E_ShowPackage;
                    switch (config.MitsubishiA1E_Datatype)
                    {
                        case "rd_bit": rd_bit.IsChecked = true; break;                        
                        case "rd_short": rd_short.IsChecked = true; break;
                        case "rd_ushort": rd_ushort.IsChecked = true; break;
                        case "rd_int": rd_int.IsChecked = true; break;
                        case "rd_uint": rd_uint.IsChecked = true; break;
                        case "rd_long": rd_long.IsChecked = true; break;
                        case "rd_ulong": rd_ulong.IsChecked = true; break;
                        case "rd_float": rd_float.IsChecked = true; break;
                        case "rd_double": rd_double.IsChecked = true; break;
                    };
                    break;
                case MitsubishiVersion.Qna_3E:
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiQna3E_IP)) txt_ip.Text = config.MitsubishiQna3E_IP;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiQna3E_Port)) txt_port.Text = config.MitsubishiQna3E_Port;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiQna3E_Address)) txt_address.Text = config.MitsubishiQna3E_Address;
                    if (!string.IsNullOrWhiteSpace(config.MitsubishiQna3E_Value)) txt_value.Text = config.MitsubishiQna3E_Value;
                    chbShowPackage.IsEnabled = config.MitsubishiQna3E_ShowPackage;
                    switch (config.MitsubishiQna3E_Datatype)
                    {
                        case "rd_bit": rd_bit.IsChecked = true; break;
                        case "rd_short": rd_short.IsChecked = true; break;
                        case "rd_ushort": rd_ushort.IsChecked = true; break;
                        case "rd_int": rd_int.IsChecked = true; break;
                        case "rd_uint": rd_uint.IsChecked = true; break;
                        case "rd_long": rd_long.IsChecked = true; break;
                        case "rd_ulong": rd_ulong.IsChecked = true; break;
                        case "rd_float": rd_float.IsChecked = true; break;
                        case "rd_double": rd_double.IsChecked = true; break;
                    };
                    break;
            }
        }
        private void AppendText(string content)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                txt_content.AppendText($"[{DateTime.Now.ToLongTimeString()}]{content}\r\n");
            }));
        }

        private void AppendEmptyText()
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                txt_content.AppendText($"\r\n");
            }));
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnOpen.Content.ToString() == "连接")
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        try
                        {
                            btnOpen.Content = "连接中...";
                            btnOpen.IsEnabled = false;
                            client?.Close();
                            client = new MitsubishiClient(version,txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()));
                            var result = client.Open();
                            if (result.IsSucceed)
                            {
                                btnRead.IsEnabled = true;
                                btnWrite.IsEnabled = true;
                                btnSendData.IsEnabled = true;                                
                                btnOpen.Content = "关闭";
                                AppendText($"连接成功\t\t\t\t耗时：{result.TimeConsuming}ms");
                            }
                            else
                            {
                                MessageBox.Show($"连接失败：{result.Err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            btnOpen.IsEnabled = true;
                        }
                    }));
                }
                else
                {
                    client?.Close();
                    AppendText("关闭连接");
                    btnSendData.IsEnabled = false;
                    btnWrite.IsEnabled = false;
                    btnRead.IsEnabled = false;                    
                    btnOpen.Content = "连接";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            dynamic result = null;
            try
            {               
                if ((bool)rd_bit.IsChecked)
                {
                    result = client.ReadBoolean(txt_address.Text);
                }
                else if ((bool)rd_short.IsChecked)
                {
                    result = client.ReadInt16(txt_address.Text);
                }
                else if ((bool)rd_ushort.IsChecked)
                {
                    result = client.ReadUInt16(txt_address.Text);
                }
                else if ((bool)rd_int.IsChecked)
                {
                    result = client.ReadInt32(txt_address.Text);
                }
                else if ((bool)rd_uint.IsChecked)
                {
                    result = client.ReadUInt32(txt_address.Text);
                }
                else if ((bool)rd_long.IsChecked)
                {
                    result = client.ReadInt64(txt_address.Text);
                }
                else if ((bool)rd_ulong.IsChecked)
                {
                    result = client.ReadUInt64(txt_address.Text);
                }
                else if ((bool)rd_float.IsChecked)
                {
                    result = client.ReadFloat(txt_address.Text);
                }
                else if ((bool)rd_double.IsChecked)
                {
                    result = client.ReadDouble(txt_address.Text);
                }

                if (result.IsSucceed)
                    AppendText($"[读取 {txt_address.Text?.Trim()} 成功]：{result.Value}\t\t耗时：{result.TimeConsuming}ms");
                else
                    AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                if ((bool)chbShowPackage.IsChecked)
                {
                    AppendText($"[请求报文]{result.Requst}");
                    AppendText($"[响应报文]{result.Response}\r\n");
                }

                var config = ConnectionConfig.GetConfig();
                switch (version)
                {
                    case MitsubishiVersion.A_1E:
                        config.MitsubishiA1E_IP = txt_ip.Text;
                        config.MitsubishiA1E_Port = txt_port.Text;
                        config.MitsubishiA1E_Address = txt_address.Text;
                        config.MitsubishiA1E_Value = txt_value.Text;
                        config.MitsubishiA1E_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.MitsubishiA1E_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.MitsubishiA1E_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.MitsubishiA1E_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.MitsubishiA1E_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.MitsubishiA1E_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.MitsubishiA1E_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.MitsubishiA1E_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.MitsubishiA1E_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.MitsubishiA1E_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.MitsubishiA1E_Datatype = "rd_double";
                        break;
                    case MitsubishiVersion.Qna_3E:
                        config.MitsubishiQna3E_IP = txt_ip.Text;
                        config.MitsubishiQna3E_Port = txt_port.Text;
                        config.MitsubishiQna3E_Address = txt_address.Text;
                        config.MitsubishiQna3E_Value = txt_value.Text;
                        config.MitsubishiQna3E_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.MitsubishiQna3E_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.MitsubishiQna3E_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.MitsubishiQna3E_Datatype = "rd_short";
                        else if ((bool)chbShowPackage.IsChecked) config.MitsubishiQna3E_Datatype = "rd_ushort";
                        else if ((bool)chbShowPackage.IsChecked) config.MitsubishiQna3E_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.MitsubishiQna3E_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.MitsubishiQna3E_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.MitsubishiQna3E_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.MitsubishiQna3E_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.MitsubishiQna3E_Datatype = "rd_double";
                        break;
                }
                config.SaveConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_value.Text))
            {
                MessageBox.Show("请输入值");
                return;
            }
            string address = txt_address.Text.Trim();
            string txtvalue=txt_value.Text.Trim();
            try
            {
                
                dynamic result = null;
                if ((bool)rd_bit.IsChecked)
                {
                    if (!bool.TryParse(txtvalue, out bool bit))
                    {
                        if (txtvalue == "0")
                            bit = false;
                        else if (txtvalue == "1")
                            bit = true;
                        else
                        {
                            MessageBox.Show("请输入 True 或 False");
                            return;
                        }
                    }
                    result = client.Write(address,bit);
                }
                else if ((bool)rd_short.IsChecked)
                {
                    result = client.Write(address, short.Parse(txtvalue));
                }
                else if ((bool)rd_ushort.IsChecked)
                {
                    result = client.Write(address, ushort.Parse(txtvalue));
                }
                else if ((bool)rd_int.IsChecked)
                {
                    result = client.Write(address, int.Parse(txtvalue));
                }
                else if ((bool)rd_uint.IsChecked)
                {
                    result = client.Write(address, uint.Parse(txtvalue));
                }
                else if ((bool)rd_long.IsChecked)
                {
                    result = client.Write(address, long.Parse(txtvalue));
                }
                else if ((bool)rd_ulong.IsChecked)
                {
                    result = client.Write(address, ulong.Parse(txtvalue));
                }
                else if ((bool)rd_float.IsChecked)
                {
                    result = client.Write(address, float.Parse(txtvalue));
                }
                else if ((bool)rd_double.IsChecked)
                {
                    result = client.Write(address, double.Parse(txtvalue));
                }               

                if (result.IsSucceed)
                    AppendText($"[写入 {address?.Trim()} 成功]：{txt_value.Text?.Trim()} OK\t\t耗时：{result.TimeConsuming}ms");
                else
                    AppendText($"[写入 {address?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                if ((bool)chbShowPackage.IsChecked)
                {
                    AppendText($"[请求报文]{result.Requst}");
                    AppendText($"[响应报文]{result.Response}\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 启动仿真服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnServerOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnServerOpen.Content.ToString() == "本地模拟服务")
                {
                    server?.Stop();
                    switch (version)
                    {
                        case MitsubishiVersion.A_1E:
                            server = new MitsubishiA1EServer(int.Parse(txt_port.Text.Trim()));
                            break;
                        case MitsubishiVersion.Qna_3E:
                            server = new MitsubishiQna3EServer(int.Parse(txt_port.Text.Trim()));
                            break;

                    }
                    server.Start();
                    AppendText($"开启仿真模拟服务");
                    btnServerOpen.Content = "关闭服务";
                }
                else
                {
                    server?.Stop();
                    AppendText("关闭仿真服务");
                    btnServerOpen.Content = "本地模拟服务";
                }

            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        private void btnSendData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_dataPackage.Text))
                {
                    MessageBox.Show("请输入要发送的报文");
                    return;
                }
                var dataPackageString = txt_dataPackage.Text.Replace(" ", "");
                if (dataPackageString.Length % 2 != 0)
                {
                    MessageBox.Show("请输入正确的的报文");
                    return;
                }

                var dataPackage = DataConvert.StringToByteArray(txt_dataPackage.Text?.Trim(), false);
                var msg = client.SendPackageReliable(dataPackage).Value;
                AppendText($"[请求报文]{string.Join(" ", dataPackage.Select(t => t.ToString("X2")))}\r");
                AppendText($"[响应报文]{string.Join(" ", msg.Select(t => t.ToString("X2")))}\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                client.Close();
                client.Open();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            DataPersist.Clear();
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                txt_content.Text = "";
            }));
            AppendText($"数据清空成功\r\n");
        }
    }
}
