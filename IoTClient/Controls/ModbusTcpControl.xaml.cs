using IoTClient.Clients.Modbus;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Models;
using IoTClientDeskTop.Common;
using IoTServer.Common;
using IoTServer.Servers.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace IoTClientDeskTop.Controls
{    
    /// <summary>
    /// ModbusTcpControl.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusTcpControl : UserControl
    {
        IModbusClient client;
        ModbusTcpServer server;
        public ModbusTcpControl()
        {
            InitializeComponent();
            btnRead.IsEnabled = false;
            btnWrite.IsEnabled = false;
            btnSendData.IsEnabled = false;
            var config = ConnectionConfig.GetConfig();
            if (!string.IsNullOrWhiteSpace(config.ModBusTcp_IP)) txt_ip.Text = config.ModBusTcp_IP;
            if (!string.IsNullOrWhiteSpace(config.ModBusTcp_Port)) txt_port.Text = config.ModBusTcp_Port;
            if (!string.IsNullOrWhiteSpace(config.ModBusTcp_Address)) txt_address.Text = config.ModBusTcp_Address;
            if (!string.IsNullOrWhiteSpace(config.ModBusTcp_Value)) txt_value.Text = config.ModBusTcp_Value;
            if (!string.IsNullOrWhiteSpace(config.ModBusTcp_StationNumber)) txtStationNum.Text = config.ModBusTcp_StationNumber;
            cmb_EndianFormat.SelectedItem = config.ModBusTcp_EndianFormat.ToString();
            switch (config.ModBusTcp_Datatype)
            {
                case "rd_coil": rd_coil.IsChecked = true; break;
                case "rd_discrete": rd_discrete.IsChecked = true; break;
                case "rd_short": rd_short.IsChecked = true; break;
                case "rd_ushort": rd_ushort.IsChecked = true; break;
                case "rd_int": rd_int.IsChecked = true; break;
                case "rd_uint": rd_uint.IsChecked = true; break;
                case "rd_long": rd_long.IsChecked = true; break;
                case "rd_ulong": rd_ulong.IsChecked = true; break;
                case "rd_float": rd_float.IsChecked = true; break;
                case "rd_double": rd_double.IsChecked = true; break;
            };
            chbShowPackage.IsChecked = config.ModBusTcp_ShowPackage;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            cmb_EndianFormat.SelectedIndex = 0;           
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
        private void ControlEnabledFalse()
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                cmb_EndianFormat.IsEnabled = false;
                txt_ip.IsEnabled = false;
                txt_port.IsEnabled = false;
                txtStationNum.IsEnabled = false;
            }));
        }
        private void ControlEnabledTrue()
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                cmb_EndianFormat.IsEnabled = true;
                txt_ip.IsEnabled = true;
                txt_port.IsEnabled = true;
                txtStationNum.IsEnabled = true;
            }));
        }
        EndianFormat format = EndianFormat.ABCD;
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
                            switch (cmb_EndianFormat.SelectedIndex)
                            {
                                case 0:
                                    format = EndianFormat.ABCD;
                                    break;
                                case 1:
                                    format = EndianFormat.BADC;
                                    break;
                                case 2:
                                    format = EndianFormat.CDAB;
                                    break;
                                case 3:
                                    format = EndianFormat.DCBA;
                                    break;
                            }
                            var plcadd = (bool)chk_plcadd.IsChecked;

                            
                            client = new ModbusTcpClient(txt_ip.Text?.Trim(), int.Parse(txt_port.Text?.Trim()), format: format, plcAddresses: plcadd);
                            var result = client.Open();
                            if (result.IsSucceed)
                            {                                
                                AppendText($"连接成功\t\t\t\t耗时：{result.TimeConsuming}ms");
                                ControlEnabledFalse();                               
                                btnRead.IsEnabled = true;
                                btnWrite.IsEnabled = true;
                                btnSendData.IsEnabled = true;
                                chk_plcadd.IsEnabled = false;
                                btnOpen.Content = "关闭";                                    
                                
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
                    chk_plcadd.IsEnabled = true;
                    ControlEnabledTrue();
                    btnOpen.Content = "连接";
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
                    server = new ModbusTcpServer(502);
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

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            byte.TryParse(txtStationNum.Text?.Trim(), out byte stationNumber);
            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("请输入地址");
                return;
            }
            dynamic result = null;
            try
            {
                var addressAndReadLength = txt_address.Text.Split('-');
                var addressAndReadNumber = txt_address.Text.Split(',', '、', '，');
                //批量读取
                if (addressAndReadLength.Length == 2)
                {
                    var address = int.Parse(addressAndReadLength[0]);
                    var readNumber = ushort.Parse(addressAndReadLength[1]);
                    ushort bLength = 1;
                    if ((bool)rd_coil.IsChecked || (bool)rd_discrete.IsChecked || (bool)rd_short.IsChecked || (bool)rd_ushort.IsChecked)
                        bLength = 1;
                    else if ((bool)rd_int.IsChecked || (bool)rd_uint.IsChecked || (bool)rd_float.IsChecked)
                        bLength = 2;
                    else if ((bool)rd_long.IsChecked || (bool)rd_ulong.IsChecked || (bool)rd_double.IsChecked)
                        bLength = 4;

                    var readLength = Convert.ToUInt16(bLength * readNumber);
                    byte functionCode;
                    if ((bool)rd_coil.IsChecked) functionCode = 1;
                    else if ((bool)rd_discrete.IsChecked) functionCode = 2;
                    else functionCode = 3;

                    result = client.Read(address.ToString(), stationNumber, functionCode, readLength: readLength, false);

                    if (result.IsSucceed)
                    {
                        AppendEmptyText();
                        byte[] rValue = result.Value;
                        rValue = rValue.Reverse().ToArray();
                        for (int i = 0; i < readNumber; i++)
                        {
                            var cAddress = (address + i * bLength).ToString();
                            if ((bool)rd_coil.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadCoil(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_discrete.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadDiscrete(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_short.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadInt16(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_ushort.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadUInt16(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_int.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadInt32(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_uint.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadUInt32(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_long.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadInt64(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_ulong.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadUInt64(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_float.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadFloat(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                            else if ((bool)rd_double.IsChecked)
                                AppendText($"[读取 {address + i * bLength} 成功]：{client.ReadDouble(address.ToString(), cAddress, rValue).Value}\t\t耗时：{result.TimeConsuming}ms");
                        }
                    }
                    else
                    { 
                        AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                    }
                }
                //批量读取
                else if (addressAndReadNumber.Length >= 2)
                {
                    DataTypeEnum datatype = DataTypeEnum.None;
                    byte functionCode = 3;
                    //线圈
                    if ((bool)rd_coil.IsChecked)
                    {
                        datatype = DataTypeEnum.Bool;
                        functionCode = 1;
                    }
                    //离散
                    else if ((bool)rd_discrete.IsChecked)
                    {
                        datatype = DataTypeEnum.Bool;
                        functionCode = 2;
                    }
                    else if ((bool)rd_short.IsChecked) datatype = DataTypeEnum.Int16;
                    else if ((bool)rd_ushort.IsChecked) datatype = DataTypeEnum.UInt16;
                    else if ((bool)rd_int.IsChecked) datatype = DataTypeEnum.Int32;
                    else if ((bool)rd_uint.IsChecked) datatype = DataTypeEnum.UInt32;
                    else if ((bool)rd_long.IsChecked) datatype = DataTypeEnum.Int64;
                    else if ((bool)rd_ulong.IsChecked) datatype = DataTypeEnum.UInt64;
                    else if ((bool)rd_float.IsChecked) datatype = DataTypeEnum.Float;
                    else if ((bool)rd_double.IsChecked) datatype = DataTypeEnum.Double;

                    List<ModbusInput> addresses = new List<ModbusInput>();
                    foreach (var item in addressAndReadNumber)
                    {
                        addresses.Add(new ModbusInput()
                        {
                            Address = item,
                            DataType = datatype,
                            FunctionCode = functionCode,
                            StationNumber = stationNumber,
                        });
                    }

                    result = client.BatchRead(addresses);

                    if (result.IsSucceed)
                    {
                        AppendEmptyText();
                        foreach (var item in result.Value)
                        {
                            AppendText($"[读取 {item.Address} 成功]：{item.Value}\t\t耗时：{result.TimeConsuming}ms");
                        }
                    }
                    else
                        AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                }
                //单个读取
                else
                {
                    if ((bool)rd_coil.IsChecked)
                    {
                        result = client.ReadCoil(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_short.IsChecked)
                    {
                        result = client.ReadInt16(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_ushort.IsChecked)
                    {
                        result = client.ReadUInt16(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_int.IsChecked)
                    {
                        result = client.ReadInt32(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_uint.IsChecked)
                    {
                        result = client.ReadUInt32(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_long.IsChecked)
                    {
                        result = client.ReadInt64(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_ulong.IsChecked)
                    {
                        result = client.ReadUInt64(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_float.IsChecked)
                    {
                        result = client.ReadFloat(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_double.IsChecked)
                    {
                        result = client.ReadDouble(txt_address.Text, stationNumber);
                    }
                    else if ((bool)rd_discrete.IsChecked)
                    {
                        result = client.ReadDiscrete(txt_address.Text, stationNumber);
                    }

                    if (result.IsSucceed)
                        AppendText($"[读取 {txt_address.Text?.Trim()} 成功]：{result.Value}\t\t耗时：{result.TimeConsuming}ms");
                    else
                        AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                }

                var config = ConnectionConfig.GetConfig();
                config.ModBusRtu_Value = txt_value.Text;
                config.ModBusRtu_Address = txt_address.Text;
                config.ModBusRtu_StationNumber = txtStationNum.Text;
                config.ModBusRtu_ShowPackage = (bool)chbShowPackage.IsChecked;
                config.ModBusRtu_Datatype = string.Empty;
                if ((bool)rd_coil.IsChecked) config.ModBusRtu_Datatype = "rd_coil";
                else if ((bool)rd_discrete.IsChecked) config.ModBusRtu_Datatype = "rd_discrete";
                else if ((bool)rd_short.IsChecked) config.ModBusRtu_Datatype = "rd_short";
                else if ((bool)rd_ushort.IsChecked) config.ModBusRtu_Datatype = "rd_ushort";
                else if ((bool)rd_int.IsChecked) config.ModBusRtu_Datatype = "rd_int";
                else if ((bool)rd_uint.IsChecked) config.ModBusRtu_Datatype = "rd_uint";
                else if ((bool)rd_long.IsChecked) config.ModBusRtu_Datatype = "rd_long";
                else if ((bool)rd_ulong.IsChecked) config.ModBusRtu_Datatype = "rd_ulong";
                else if ((bool)rd_float.IsChecked) config.ModBusRtu_Datatype = "rd_float";
                else if ((bool)rd_double.IsChecked) config.ModBusRtu_Datatype = "rd_double";
                config.SaveConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if ((bool)chbShowPackage.IsChecked)
                {
                    AppendText($"[请求报文]{result?.Requst}");
                    AppendText($"[响应报文]{result?.Response}\r\n");
                }
            }
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            byte.TryParse(txtStationNum.Text?.Trim(), out byte stationNumber);
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
            try
            {
                var address = txt_address.Text?.Trim().Split('-')[0];
                dynamic result = null;
                if ((bool)rd_coil.IsChecked)
                {
                    if (!bool.TryParse(txt_value.Text?.Trim(), out bool coil))
                    {
                        if (txt_value.Text?.Trim() == "0")
                            coil = false;
                        else if (txt_value.Text?.Trim() == "1")
                            coil = true;
                        else
                        {
                            MessageBox.Show("请输入 True 或 False");
                            return;
                        }
                    }
                    result = client.Write(address, coil, stationNumber);
                }
                else if ((bool)rd_short.IsChecked)
                {
                    result = client.Write(address, short.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_ushort.IsChecked)
                {
                    result = client.Write(address, ushort.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_int.IsChecked)
                {
                    result = client.Write(address, int.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_uint.IsChecked)
                {
                    result = client.Write(address, uint.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_long.IsChecked)
                {
                    result = client.Write(address, long.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_ulong.IsChecked)
                {
                    result = client.Write(address, ulong.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_float.IsChecked)
                {
                    result = client.Write(address, float.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_double.IsChecked)
                {
                    result = client.Write(address, double.Parse(txt_value.Text?.Trim()), stationNumber);
                }
                else if ((bool)rd_discrete.IsChecked)
                {
                    AppendText($"离散类型只读");
                    return;
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
                txt_content.Text="";
            }));
            AppendText($"数据清空成功\r\n");
        }
    }
}
