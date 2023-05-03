//using HandyControl.Controls;
using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClientDeskTop.Common;
using IoTServer.Common;
using IoTServer.Servers.PLC;
using SharpPcap;
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
using Talk.Linq.Extensions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace IoTClientDeskTop.Controls
{
    /// <summary>
    /// SiemensControl.xaml 的交互逻辑
    /// </summary>
    public partial class SiemensControl : UserControl
    {
        SiemensClient client;
        SiemensServer server;
        private SiemensVersion version;
        public SiemensControl(SiemensVersion version)
        {
            InitializeComponent();
            this.version = version;
            btnRead.IsEnabled = false;
            btnWrite.IsEnabled = false;
            btnSendData.IsEnabled = false;
            var config = ConnectionConfig.GetConfig();
            //txt_content.
            string str1 = "1、读取地址支持批量读取，如V2634、V2638、V2642。\r\n"
                        + "2、关于PLC地址：VB263、VW263、VD263中的B、W、D分别表示byte、word、doubleword数据类型，\r\n"
                        + "分别对应C#中的byte、ushort(UInt16)、uint(UInt32)类型。此工具直接传入地址（如:V263）即可。\r\n"
                        + "3、写入时，地址不支持批量，如果地址写入了批量的，只读取第一个地址";
            string str2 = "1、读取地址支持批量读取，如DB1.DBW2、DB1.DBW4、DB1.DBW6。\r\n"
                        + "2、关于PLC地址：DB1.DBX1.0中DBX1.0的X代表bit(C# bit)，DB1.DBB1中的DBB1的第二个B表示byte(C# byte)，\r\n"
                        + "DB1.DBW2中DBW2的W表示word(C# ushort)，DB1.DBD4中DBD4的第二个D代表doubleword(C# uint)数据类型，\r\n"
                        + "此工具直接传入地址（如:DB1.1.0, DB1.1,DB1.2,DB1.4.）即可。\r\n"
                        + "3、写入时，地址不支持批量，如果地址写入了批量的，只读取第一个地址";
            string str3 = "V1.0";
            string str4 = "DB1.DBX1.0";
            switch (version)
            {
                case SiemensVersion.S7_200:
                    if (!string.IsNullOrWhiteSpace(config.S7200_IP)) txt_ip.Text = config.S7200_IP;
                    if (!string.IsNullOrWhiteSpace(config.S7200_Port)) txt_port.Text = config.S7200_Port;
                    if (!string.IsNullOrWhiteSpace(config.S7200_Address)) txt_address.Text = config.S7200_Address;
                    if (!string.IsNullOrWhiteSpace(config.S7200_Value)) txt_value.Text = config.S7200_Value;
                    if (!string.IsNullOrWhiteSpace(config.S7200_Slot)) txt_slot.Text = config.S7200_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S7200_Rack)) txt_rack.Text = config.S7200_Rack;
                    chbShowPackage.IsChecked = config.S7200_ShowPackage;
                    InitDataType(config.S7200_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str1);
                    txt_address.ToolTip = str1;
                    txt_address.Text = str3;
                    break;
                case SiemensVersion.S7_200Smart:
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_IP)) txt_ip.Text = config.S7200Smart_IP;
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_Port)) txt_port.Text = config.S7200Smart_Port;
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_Address)) txt_address.Text = config.S7200Smart_Address;
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_Value)) txt_value.Text = config.S7200Smart_Value;
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_Slot)) txt_slot.Text = config.S7200Smart_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S7200Smart_Rack)) txt_rack.Text = config.S7200Smart_Rack;
                    chbShowPackage.IsChecked = config.S7200Smart_ShowPackage;
                    InitDataType(config.S7200Smart_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str1);
                    txt_address.ToolTip = str1;
                    txt_address.Text = str3;
                    break;
                case SiemensVersion.S7_300:
                    if (!string.IsNullOrWhiteSpace(config.S7300_IP)) txt_ip.Text = config.S7300_IP;
                    if (!string.IsNullOrWhiteSpace(config.S7300_Port)) txt_port.Text = config.S7300_Port;
                    if (!string.IsNullOrWhiteSpace(config.S7300_Address)) txt_address.Text = config.S7300_Address;
                    if (!string.IsNullOrWhiteSpace(config.S7300_Value)) txt_value.Text = config.S7300_Value;
                    if (!string.IsNullOrWhiteSpace(config.S7300_Slot)) txt_slot.Text = config.S7300_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S7300_Rack)) txt_rack.Text = config.S7300_Rack;
                    chbShowPackage.IsChecked = config.S7300_ShowPackage;
                    InitDataType(config.S7300_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str2);
                    txt_address.ToolTip = str2;
                    txt_address.Text = str4;
                    break;
                case SiemensVersion.S7_400:
                    if (!string.IsNullOrWhiteSpace(config.S7400_IP)) txt_ip.Text = config.S7400_IP;
                    if (!string.IsNullOrWhiteSpace(config.S7400_Port)) txt_port.Text = config.S7400_Port;
                    if (!string.IsNullOrWhiteSpace(config.S7400_Address)) txt_address.Text = config.S7400_Address;
                    if (!string.IsNullOrWhiteSpace(config.S7400_Value)) txt_value.Text = config.S7400_Value;
                    if (!string.IsNullOrWhiteSpace(config.S7400_Slot)) txt_slot.Text = config.S7400_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S7400_Rack)) txt_rack.Text = config.S7400_Rack;
                    chbShowPackage.IsChecked = config.S7400_ShowPackage;
                    InitDataType(config.S7400_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str2);
                    txt_address.ToolTip = str2;
                    txt_address.Text = str4;
                    break;
                case SiemensVersion.S7_1200:
                    if (!string.IsNullOrWhiteSpace(config.S71200_IP)) txt_ip.Text = config.S71200_IP;
                    if (!string.IsNullOrWhiteSpace(config.S71200_Port)) txt_port.Text = config.S71200_Port;
                    if (!string.IsNullOrWhiteSpace(config.S71200_Address)) txt_address.Text = config.S71200_Address;
                    if (!string.IsNullOrWhiteSpace(config.S71200_Value)) txt_value.Text = config.S71200_Value;
                    if (!string.IsNullOrWhiteSpace(config.S71200_Slot)) txt_slot.Text = config.S71200_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S71200_Rack)) txt_rack.Text = config.S71200_Rack;
                    chbShowPackage.IsChecked = config.S71200_ShowPackage;
                    InitDataType(config.S71200_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str2);
                    txt_address.ToolTip = str2;
                    txt_address.Text = str4;
                    break;
                case SiemensVersion.S7_1500:
                    if (!string.IsNullOrWhiteSpace(config.S71500_IP)) txt_ip.Text = config.S71500_IP;
                    if (!string.IsNullOrWhiteSpace(config.S71500_Port)) txt_port.Text = config.S71500_Port;
                    if (!string.IsNullOrWhiteSpace(config.S71500_Address)) txt_address.Text = config.S71500_Address;
                    if (!string.IsNullOrWhiteSpace(config.S71500_Value)) txt_value.Text = config.S71500_Value;
                    if (!string.IsNullOrWhiteSpace(config.S71500_Slot)) txt_slot.Text = config.S71500_Slot;
                    if (!string.IsNullOrWhiteSpace(config.S71500_Rack)) txt_rack.Text = config.S71500_Rack;
                    chbShowPackage.IsChecked = config.S71500_ShowPackage;
                    InitDataType(config.S71500_Datatype);
                    HandyControl.Controls.InfoElement.SetPlaceholder(txt_content, str2);
                    txt_address.ToolTip = str2;
                    txt_address.Text = str4;
                    break;
            }
        }

        private void InitDataType(string str)
        {
            switch (str)
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
                    server = new SiemensServer(int.Parse(txt_port.Text.Trim()));
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
                            var slot = byte.Parse(txt_slot.Text?.Trim());
                            var rack = byte.Parse(txt_rack.Text?.Trim());
                            client = new SiemensClient(version, txt_ip.Text?.Trim(), int.Parse(txt_port.Text.Trim()), slot, rack);
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
                            if ((bool)chbShowPackage.IsChecked)
                            {
                                AppendText($"[请求报文]{result.Requst}");
                                if (result.Response.IsAny())
                                    AppendText($"[响应报文]{result.Response}");
                                if (result.Requst2.IsAny())
                                    AppendText($"[请求报文]{result.Requst2}");
                                if (result.Response2.IsAny())
                                    AppendText($"[响应报文]{result.Response2}\r\n");
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
            var addressAndReadNumber = txt_address.Text.Split(',', '、', '，');
            try
            {
                //批量读取
                if (addressAndReadNumber.Length >= 2)
                {
                    DataTypeEnum datatype = DataTypeEnum.None;
                    if ((bool)rd_byte.IsChecked) datatype = DataTypeEnum.Byte;
                    else if ((bool)rd_bit.IsChecked) datatype = DataTypeEnum.Bool;
                    else if ((bool)rd_short.IsChecked) datatype = DataTypeEnum.Int16;
                    else if ((bool)rd_ushort.IsChecked) datatype = DataTypeEnum.UInt16;
                    else if ((bool)rd_int.IsChecked) datatype = DataTypeEnum.Int32;
                    else if ((bool)rd_uint.IsChecked) datatype = DataTypeEnum.UInt32;
                    else if ((bool)rd_long.IsChecked) datatype = DataTypeEnum.Int64;
                    else if ((bool)rd_ulong.IsChecked) datatype = DataTypeEnum.UInt64;
                    else if ((bool)rd_float.IsChecked) datatype = DataTypeEnum.Float;
                    else if ((bool)rd_double.IsChecked) datatype = DataTypeEnum.Double;

                    Dictionary<string, DataTypeEnum> addresses = new Dictionary<string, DataTypeEnum>();
                    foreach (var item in addressAndReadNumber)
                    {
                        addresses.Add(item, datatype);
                    }

                    result = client.BatchRead(addresses);

                    if (result.IsSucceed)
                    {
                        AppendEmptyText();
                        foreach (var item in result.Value)
                        {
                            AppendText($"[读取 {item.Key} 成功]：{item.Value}\t\t耗时：{result.TimeConsuming}ms");
                        }
                    }
                    else
                        AppendText($"[读取 {txt_address.Text?.Trim()} 失败]：{result.Err}\t\t耗时：{result.TimeConsuming}ms");
                }
                //单个读取
                else
                {
                    if ((bool)rd_byte.IsChecked)
                    {
                        result = client.ReadByte(txt_address.Text);
                    }
                    else if ((bool)rd_bit.IsChecked)
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
                }

                if ((bool)chbShowPackage.IsChecked )
                {
                    AppendText($"[请求报文]{result.Requst}");
                    AppendText($"[响应报文]{result.Response}\r\n");
                }

                var config = ConnectionConfig.GetConfig();
                switch (version)
                {
                    case SiemensVersion.S7_200:
                        config.S7200_IP = txt_ip.Text;
                        config.S7200_Port = txt_port.Text;
                        config.S7200_Address = txt_address.Text;
                        config.S7200_Value = txt_value.Text;
                        config.S7200_Slot = txt_slot.Text;
                        config.S7200_Rack = txt_rack.Text;
                        config.S7200_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S7200_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S7200_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S7200_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S7200_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S7200_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S7200_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S7200_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S7200_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S7200_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S7200_Datatype = "rd_double";
                        break;
                    case SiemensVersion.S7_200Smart:
                        config.S7200Smart_IP = txt_ip.Text;
                        config.S7200Smart_Port = txt_port.Text;
                        config.S7200Smart_Address = txt_address.Text;
                        config.S7200Smart_Value = txt_value.Text;
                        config.S7200Smart_Slot = txt_slot.Text;
                        config.S7200Smart_Rack = txt_rack.Text;
                        config.S7200Smart_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S7200Smart_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S7200Smart_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S7200Smart_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S7200Smart_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S7200Smart_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S7200Smart_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S7200Smart_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S7200Smart_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S7200Smart_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S7200Smart_Datatype = "rd_double";
                        break;
                    case SiemensVersion.S7_300:
                        config.S7300_IP = txt_ip.Text;
                        config.S7300_Port = txt_port.Text;
                        config.S7300_Address = txt_address.Text;
                        config.S7300_Value = txt_value.Text;
                        config.S7300_Slot = txt_slot.Text;
                        config.S7300_Rack = txt_rack.Text;
                        config.S7300_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S7300_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S7300_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S7300_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S7300_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S7300_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S7300_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S7300_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S7300_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S7300_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S7300_Datatype = "rd_double";
                        break;
                    case SiemensVersion.S7_400:
                        config.S7400_IP = txt_ip.Text;
                        config.S7400_Port = txt_port.Text;
                        config.S7400_Address = txt_address.Text;
                        config.S7400_Value = txt_value.Text;
                        config.S7400_Slot = txt_slot.Text;
                        config.S7400_Rack = txt_rack.Text;
                        config.S7400_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S7400_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S7400_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S7400_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S7400_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S7400_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S7400_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S7400_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S7400_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S7400_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S7400_Datatype = "rd_double";
                        break;
                    case SiemensVersion.S7_1200:
                        config.S71200_IP = txt_ip.Text;
                        config.S71200_Port = txt_port.Text;
                        config.S71200_Address = txt_address.Text;
                        config.S71200_Value = txt_value.Text;
                        config.S71200_Slot = txt_slot.Text;
                        config.S71200_Rack = txt_rack.Text;
                        config.S71200_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S71200_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S71200_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S71200_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S71200_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S71200_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S71200_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S71200_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S71200_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S71200_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S71200_Datatype = "rd_double";
                        break;
                    case SiemensVersion.S7_1500:
                        config.S71500_IP = txt_ip.Text;
                        config.S71500_Port = txt_port.Text;
                        config.S71500_Address = txt_address.Text;
                        config.S71500_Value = txt_value.Text;
                        config.S71500_Slot = txt_slot.Text;
                        config.S71500_Rack = txt_rack.Text;
                        config.S71500_ShowPackage = (bool)chbShowPackage.IsChecked;
                        config.S71500_Datatype = string.Empty;
                        if ((bool)rd_bit.IsChecked) config.S71500_Datatype = "rd_bit";
                        else if ((bool)rd_short.IsChecked) config.S71500_Datatype = "rd_short";
                        else if ((bool)rd_ushort.IsChecked) config.S71500_Datatype = "rd_ushort";
                        else if ((bool)rd_int.IsChecked) config.S71500_Datatype = "rd_int";
                        else if ((bool)rd_uint.IsChecked) config.S71500_Datatype = "rd_uint";
                        else if ((bool)rd_long.IsChecked) config.S71500_Datatype = "rd_long";
                        else if ((bool)rd_ulong.IsChecked) config.S71500_Datatype = "rd_ulong";
                        else if ((bool)rd_float.IsChecked) config.S71500_Datatype = "rd_float";
                        else if ((bool)rd_double.IsChecked) config.S71500_Datatype = "rd_double";
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
            string address = txt_address.Text.Trim().Split('-')[0]; ;
            string txtvalue = txt_value.Text.Trim();
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
                    result = client.Write(address, bit);
                }
                else if ((bool)rd_byte.IsChecked)
                {
                    result = client.Write(address, byte.Parse(txtvalue));
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

    }
}
