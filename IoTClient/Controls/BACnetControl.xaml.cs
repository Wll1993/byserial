using System;
using System.Collections.Generic;
using System.IO.BACnet;
using IoTClientDeskTop.Model;
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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using HandyControl.Controls;
using IoTClient.Enums;
using IoTServer.Servers.BACnet;
using Microsoft.Win32;
using Talk.NPOI;
using IoTClientDeskTop.Common;

namespace IoTClientDeskTop.Controls
{
    /// <summary>
    /// BACnetControl.xaml 的交互逻辑
    /// </summary>
    public partial class BACnetControl : UserControl
    {
        public BACnetControl()
        {
            InitializeComponent();
            btnExport.IsEnabled = false;
            cmbIP.Items.Clear();
            List<string> ips = GetIpList();
            foreach(string ip in ips)
            {
                cmbIP.Items.Add(ip);
            }           
            cmbPriority.Items.Clear();
            for (int i = 0; i < 16; i++)
            {
                cmbPriority.Items.Add((i + 1).ToString());
            }
            
        }
        private static List<BacNode> devicesList = new List<BacNode>();
        private BacnetClient Bacnet_client;

        public List<string> GetIpList()
        {
            List<string> str= NetworkInterface.GetAllNetworkInterfaces()
                       .Where(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up)
                       //.OrderByDescending(c => c.Speed)
                       .SelectMany(c => c.GetIPProperties().UnicastAddresses.Where(t => t.Address.AddressFamily == AddressFamily.InterNetwork).Select(t => t.Address.ToString()))
                       .OrderBy(t => t.StartsWith("192.168") ? 0 : 1).ThenBy(t => t)
                       .ToList();
            return str;
        }
        private bool _IsLoaded=false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbIP.SelectedIndex = 0;
            cmbPriority.SelectedIndex = 15;
            _IsLoaded = true;
        }

        private void btnReScan_Click(object sender, RoutedEventArgs e)
        {
            txtMsgList.Text = "";
            btnReScan.IsEnabled = false;
            btnReScan.Content = "扫描中...";
            cmbIP.IsEnabled = false;
            btnExport.IsEnabled = false;
            devicesList = new List<BacNode>();
            listBox1.Items.Clear();
            Bacnet_client?.Dispose();
            //BACnet的默认端口47808
            string ip = cmbIP.SelectedItem.ToString();
            Bacnet_client = new BacnetClient(new BacnetIpUdpProtocolTransport(47808, false, localEndpointIp: ip));
            //写入优先级需要做界面设置
            cmbPriority.SelectedIndex = 15;
            Bacnet_client.OnIam -= new BacnetClient.IamHandler(handler_OnIam);
            Bacnet_client.OnIam += new BacnetClient.IamHandler(handler_OnIam);
            Bacnet_client.Start();
            Bacnet_client.WhoIs();
            Task.Run(async () =>
            {
                //Log("准备扫描...");
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(100);
                    Log($"等待扫描...[{9 - i}]");
                }
                if (listBox1.Items.Count == 1)
                    listBox1.SelectedIndex = 0;
                Scan();
                btnReScan.Dispatcher.Invoke(() =>
                {
                    btnReScan.IsEnabled = true;
                    btnReScan.Content = "重新扫描";
                    cmbIP.IsEnabled = true;
                    if (bacnetPropertyInfos.Any())
                        btnExport.IsEnabled = true;
                });
            });
        }
        private void handler_OnIam(BacnetClient sender, BacnetAddress adr, uint deviceId, uint maxAPDU, BacnetSegmentations segmentation, ushort vendorId)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (devicesList)
                {
                    foreach (BacNode bn in devicesList)
                        if (bn.GetAdd(deviceId) != null) return;   // Yes

                    devicesList.Add(new BacNode(adr, deviceId));   // add it 
                    listBox1.Items.Add(adr.ToString() + " " + deviceId);
                }
            }));
        }
        private List<BacnetPropertyInfo> bacnetPropertyInfos = new List<BacnetPropertyInfo>();
        /// <summary>
        /// 扫描
        /// </summary>
        private void Scan()
        {
            bacnetPropertyInfos = new List<BacnetPropertyInfo>();
            Log("开始扫描设备...");
            foreach (var device in devicesList)
            {
                //获取子节点个数
                var deviceCount = GetDeviceArrayIndexCount(device) + 1;
                //TODO 20 可设置 配置
                ScanPointsBatch(device, 20, deviceCount);
            }
            foreach (var device in devicesList)
            {
                LogEmpty();
                Log($"开始扫描属性,Address:{device.Address} DeviceId:{device.DeviceId}");
                ScanSubProperties(device);
                if (bacnetPropertyInfos.Any())
                    bacnetPropertyInfos.Add(new BacnetPropertyInfo());
            }
            Log("扫描完成");
        }

        /// <summary>
        /// 扫描设备
        /// </summary>
        public void ScanPointsBatch(BacNode device, uint deviceCount, uint count)
        {
            try
            {
                if (device == null) return;
                var pid = BacnetPropertyIds.PROP_OBJECT_LIST;
                var device_id = device.DeviceId;
                var bobj = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device_id);
                var adr = device.Address;
                if (adr == null) return;

                device.Properties.Clear();
                List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
                for (uint i = 0; i < count; i++)
                {
                    rList.Add(new BacnetPropertyReference((uint)pid, i));
                    if ((i != 0 && i % deviceCount == 0) || i == count - 1)//不要超了 MaxAPDU
                    {
                        IList<BacnetReadAccessResult> lstAccessRst = Bacnet_client.ReadPropertyMultipleRequest(adr, bobj, rList);
                        if (lstAccessRst?.Any() ?? false)
                        {
                            foreach (var aRst in lstAccessRst)
                            {
                                if (aRst.values == null) continue;
                                foreach (var bPValue in aRst.values)
                                {
                                    if (bPValue.value == null) continue;
                                    foreach (var bValue in bPValue.value)
                                    {
                                        var strBValue = "" + bValue.Value;
                                        //Log(pid + " , " + strBValue + " , " + bValue.Tag);

                                        var strs = strBValue.Split(':');
                                        if (strs.Length < 2) continue;
                                        var strType = strs[0];
                                        var strObjId = strs[1];
                                        var subNode = new BacProperty();
                                        BacnetObjectTypes otype;
                                        Enum.TryParse(strType, out otype);
                                        if (otype == BacnetObjectTypes.OBJECT_NOTIFICATION_CLASS || otype == BacnetObjectTypes.OBJECT_DEVICE) continue;
                                        subNode.ObjectId = new BacnetObjectId(otype, Convert.ToUInt32(strObjId));
                                        //添加属性
                                        device.Properties.Add(subNode);
                                    }
                                }
                            }
                        }
                        rList.Clear();
                    }
                }
            }
            catch (Exception exp)
            {
                Log("=== 【Err】" + exp.Message + " ===");
            }
        }

        //获取子节点个数
        public uint GetDeviceArrayIndexCount(BacNode device)
        {
            try
            {
                var adr = device.Address;
                if (adr == null) return 0;
                var bacnetValue = ReadScalarValue(adr,
                    new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, device.DeviceId),
                    BacnetPropertyIds.PROP_OBJECT_LIST, 0, 0);
                var rst = Convert.ToUInt32(bacnetValue.Value);
                return rst;
            }
            catch (Exception ex)
            {
                Log("=== 【Err】" + ex.Message + " ===");
            }
            return 0;
        }

        private BacnetValue ReadScalarValue(BacnetAddress adr, BacnetObjectId oid,
            BacnetPropertyIds pid, byte invokeId = 0, uint arrayIndex = uint.MaxValue)
        {
            try
            {
                BacnetValue NoScalarValue = Bacnet_client.ReadPropertyRequest(adr, oid, pid, arrayIndex);
                return NoScalarValue;
            }
            catch (Exception ex)
            {
                Log("=== 【Err】" + ex.Message + " ===");
            }
            return new BacnetValue();
        }

        /// <summary>
        /// 扫描属性
        /// </summary>
        /// <param name="device"></param>
        private void ScanSubProperties(BacNode device)
        {
            try
            {
                var adr = device.Address;
                if (adr == null) return;
                if (device.Properties == null) return;

                List<BacnetPropertyReference> rList = new List<BacnetPropertyReference>();
                rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_DESCRIPTION, uint.MaxValue));
                rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_REQUIRED, uint.MaxValue));
                rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_OBJECT_NAME, uint.MaxValue));
                rList.Add(new BacnetPropertyReference((uint)BacnetPropertyIds.PROP_PRESENT_VALUE, uint.MaxValue));

                List<BacnetReadAccessResult> lstAccessRst = new List<BacnetReadAccessResult>();
                var batchNumber = (int)numUpDown.Value;
                var batchCount = Math.Ceiling((float)device.Properties.Count / batchNumber);
                for (int i = 0; i < batchCount; i++)
                {
                    IList<BacnetReadAccessSpecification> properties = device.Properties.Skip(i * batchNumber).Take(batchNumber)
                        .Select(t => new BacnetReadAccessSpecification(t.ObjectId, rList)).ToList();
                    //批量读取
                    lstAccessRst.AddRange(Bacnet_client.ReadPropertyMultipleRequest(adr, properties));
                }

                if (lstAccessRst?.Any() ?? false)
                {
                    foreach (var aRst in lstAccessRst)
                    {
                        if (aRst.values == null) continue;
                        var subNode = device.Properties
                            .Where(t => t.ObjectId.Instance == aRst.objectIdentifier.Instance && t.ObjectId.Type == aRst.objectIdentifier.Type)
                            .FirstOrDefault();
                        foreach (var bPValue in aRst.values)
                        {
                            if (bPValue.value == null || bPValue.value.Count == 0) continue;
                            var pid = (BacnetPropertyIds)(bPValue.property.propertyIdentifier);
                            var bValue = bPValue.value.First();
                            var strBValue = "" + bValue.Value;
                            //Log(pid + " , " + strBValue + " , " + bValue.Tag);
                            switch (pid)
                            {
                                case BacnetPropertyIds.PROP_DESCRIPTION://描述
                                    {
                                        subNode.Prop_Description = bValue.ToString()?.Trim();
                                    }
                                    break;
                                case BacnetPropertyIds.PROP_OBJECT_NAME://点名
                                    {
                                        subNode.Prop_Object_Name = bValue.ToString()?.Trim();
                                    }
                                    break;
                                case BacnetPropertyIds.PROP_PRESENT_VALUE://值
                                    {
                                        subNode.Prop_Present_Value = bValue.Value;
                                        subNode.Prop_DataType = DataTypeConversion(aRst.objectIdentifier.Type);
                                    }
                                    break;
                            }
                        }
                        ShwoText(string.Format("地址:{0,-6} 值:{2,-8}  类型:{3,-8}  点名:{1}\t 描述:{4} ",
                            $"{subNode.ObjectId.Instance}_{(int)subNode.ObjectId.Type}",
                            subNode.Prop_Object_Name,
                            subNode.Prop_Present_Value,
                            subNode.Prop_DataType,
                            subNode.Prop_Description));

                        bacnetPropertyInfos.Add(new BacnetPropertyInfo()
                        {
                            IpAddress = $"{device.Address}:{device.DeviceId}",
                            Address = $"{subNode.ObjectId.Instance}_{(int)subNode.ObjectId.Type}",
                            DataType = subNode.Prop_DataType.ToString(),
                            Value = subNode.Prop_Present_Value.ToString(),
                            PropName = subNode.Prop_Object_Name,
                            Describe = subNode.Prop_Description,

                            ObjectType = aRst.objectIdentifier.Type.ToString(),
                            ReadWrite = aRst.objectIdentifier.Type == BacnetObjectTypes.OBJECT_ANALOG_INPUT ? "只读" : ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log("=== 【Err】" + ex.Message + " ===");
            }
        }

        private void LogEmpty()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtMsgList.AppendText($"\r\n");
            }));
        }
        private void Log(string str)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtMsgList.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}]:{str} \r\n");
            }));
        }
        private void ShwoText(string str)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtMsgList.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {str} \r\n");
            }));
        }

        private async void Read_ClickAsync(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                Log("=== 请在左边设备列表选择要操作的设备 ===");
                return;
            }
            var ipAddress = listBox1.SelectedItem.ToString().Split(' ')[0];
            var deviceId = listBox1.SelectedItem.ToString().Split(' ')[1];
            BacNode bacnet = devicesList.Where(t => t.Address.ToString() == ipAddress && t.DeviceId.ToString() == deviceId).FirstOrDefault();

            var address = txtAddress.Text?.Trim();
            var addressPart = address.Split('_');
            BacProperty rpop = null;

            if (addressPart.Length == 1)
            {
                rpop = bacnet?.Properties.Where(t => t.Prop_Object_Name == address).FirstOrDefault();
                //bacnet = devicesList.Where(t => t.Properties.Any(p => p.PROP_OBJECT_NAME == address)).FirstOrDefault();
            }
            else if (addressPart.Length == 2)
            {
                rpop = bacnet?.Properties
                    .Where(t => t.ObjectId.Instance == uint.Parse(addressPart[0]) && t.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1]))
                    .FirstOrDefault();
                //bacnet = devicesList
                //    .Where(t => t.Properties.Any(p => p.ObjectId.Instance == uint.Parse(addressPart[0]) && p.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1])))
                //    .FirstOrDefault();
            }
            else
            {
                Log("请输入正确的地址");
                return;
            }

            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }
            int retry = 0;//重试
        tag_retry:
            IList<BacnetValue> NoScalarValue = Bacnet_client.ReadPropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE);
            if (NoScalarValue?.Any() ?? false)
            {
                await Task.Delay(retry * 200);
                try
                {
                    var value = NoScalarValue[0].Value;
                    ShwoText(string.Format("[读取成功][{3}] 点:{0,-15} 值:{1,-10} 类型:{2}",
                        address,
                        value?.ToString(),
                        rpop?.Prop_DataType.ToString(),
                        retry));
                }
                catch (Exception ex)
                {
                    Log($"=== 【Err】读取失败.[{retry}]{ex.Message}" + " ===");
                }
            }
            else
            {
                retry++;
                if (retry < 4) goto tag_retry;
                Log($"=== 【Err】读取失败[{retry - 1}]" + " ===");
            }
        }

        private async void Write_ClickAsync(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                Log("=== 请在左边设备列表选择要操作的设备 ===");
                return;
            }
            var ipAddress = listBox1.SelectedItem.ToString().Split(' ')[0];
            var deviceId = listBox1.SelectedItem.ToString().Split(' ')[1];
            BacNode bacnet = devicesList.Where(t => t.Address.ToString() == ipAddress && t.DeviceId.ToString() == deviceId).FirstOrDefault();

            var address = txtAddress.Text?.Trim();
            var value = txtWriteValue.Text?.Trim();
            var addressPart = address.Split('_');

            BacProperty rpop = null;

            if (addressPart.Length == 1)
            {
                rpop = bacnet?.Properties.Where(t => t.Prop_Object_Name == address).FirstOrDefault();
                //bacnet = devicesList.Where(t => t.Properties.Any(p => p.PROP_OBJECT_NAME == address)).FirstOrDefault();
            }
            else if (addressPart.Length == 2)
            {
                rpop = bacnet?.Properties
                    .Where(t => t.ObjectId.Instance == uint.Parse(addressPart[0]) && t.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1]))
                    .FirstOrDefault();
                //bacnet = devicesList
                //    .Where(t => t.Properties.Any(p => p.ObjectId.Instance == uint.Parse(addressPart[0]) && p.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1])))
                //    .FirstOrDefault();
            }
            else
            {
                Log("请输入正确的地址");
                return;
            }

            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }
            var writePriority = uint.Parse(cmbPriority.SelectedItem.ToString());
            Bacnet_client.WritePriority = writePriority;
            List<BacnetValue> NoScalarValue = new List<BacnetValue>() { new BacnetValue(value.ToDataFormType(rpop.Prop_DataType)) };
            //如果是Bool类型，且原值是1、0枚举类型
            if (rpop.Prop_DataType == DataTypeEnum.Bool && (rpop.Prop_Present_Value?.ToString() == "1" || rpop.Prop_Present_Value?.ToString() == "0"))
            {
                var tempValue = value == "1" || value.ToLower() == "true" ? 1 : 0;
                NoScalarValue = new List<BacnetValue>() { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, tempValue) };
            }

            int retry = 0;//重试
        tag_retry:
            try
            {
                await Task.Delay(retry * 200);
                Bacnet_client.WritePropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE, NoScalarValue);
                ShwoText(string.Format("[写入成功][{2}] 点:{0,-15} 值:{1,-10} 优先级[{3}]", address, value, retry, writePriority));
            }
            catch (Exception ex)
            {
                //Bool写入如果类型错误，则可能是BACNET_APPLICATION_TAG_ENUMERATED （Bool类型值的存储可能是 True、False 或者 1、0）
                if (rpop.Prop_DataType == DataTypeEnum.Bool && ex.Message.EndsWith("ERROR_CODE_INVALID_DATA_TYPE"))
                {
                    var tempValue = value == "1" || value.ToLower() == "true" ? 1 : 0;
                    BacnetValue[] newNoScalarValue = { new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, tempValue) };
                    Bacnet_client.WritePropertyRequest(bacnet.Address, rpop.ObjectId, BacnetPropertyIds.PROP_PRESENT_VALUE, newNoScalarValue);
                    ShwoText(string.Format("[写入成功][{2}] 点:{0,-15} 值:{1,-10} 优先级[{3}]", address, value, retry, writePriority));
                }
                else
                {
                    retry++;
                    if (retry < 4) goto tag_retry;//强行重试
                    Log($"写入失败[{retry - 1}]:{ex.Message}");
                }
            }
        }

       

        private DataTypeEnum DataTypeConversion(BacnetObjectTypes bacnetObjectType)
        {
            DataTypeEnum type;
            switch (bacnetObjectType)
            {
                case BacnetObjectTypes.OBJECT_ANALOG_INPUT:
                case BacnetObjectTypes.OBJECT_ANALOG_OUTPUT:
                case BacnetObjectTypes.OBJECT_ANALOG_VALUE:
                    type = DataTypeEnum.Float;
                    break;
                case BacnetObjectTypes.OBJECT_BINARY_INPUT:
                case BacnetObjectTypes.OBJECT_BINARY_OUTPUT:
                case BacnetObjectTypes.OBJECT_BINARY_VALUE:
                    type = DataTypeEnum.Bool;
                    break;
                case BacnetObjectTypes.OBJECT_MULTI_STATE_INPUT:
                case BacnetObjectTypes.OBJECT_MULTI_STATE_OUTPUT:
                case BacnetObjectTypes.OBJECT_MULTI_STATE_VALUE:
                    type = DataTypeEnum.UInt32;
                    break;
                case BacnetObjectTypes.OBJECT_CHARACTERSTRING_VALUE:
                    type = DataTypeEnum.String;
                    break;
                default:
                    type = DataTypeEnum.None;
                    break;
            }
            return type;
        }

        private void but_export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置保存文件对话框的标题
            sfd.Title = "请选择要保存的文件路径";
            //设置保存文件的类型
            sfd.Filter = "Excel文件|*.xls";
            //文件名
            sfd.FileName = $"BACnet_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            if ((bool)sfd.ShowDialog())
            {
                try
                {
                    bacnetPropertyInfos?.ToExcel(sfd.FileName);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

       

        private async void btnRelease_ClickAsync(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                Log("=== 请在左边设备列表选择要操作的设备 ===");
                return;
            }
            var ipAddress = listBox1.SelectedItem.ToString().Split(' ')[0];
            var deviceId = listBox1.SelectedItem.ToString().Split(' ')[1];
            BacNode bacnet = devicesList.Where(t => t.Address.ToString() == ipAddress && t.DeviceId.ToString() == deviceId).FirstOrDefault();
            

            var address = txtAddress.Text?.Trim();
            var value = txtWriteValue.Text?.Trim();
            var addressPart = address.Split('_');
            BacProperty rpop = null;
            if (addressPart.Length == 1)
            {
                rpop = bacnet?.Properties.Where(t => t.Prop_Object_Name == address).FirstOrDefault();
            }
            else if (addressPart.Length == 2)
            {
                rpop = bacnet?.Properties
                    .Where(t => t.ObjectId.Instance == uint.Parse(addressPart[0]) && t.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1]))
                    .FirstOrDefault();
            }
            else
            {
                Log("请输入正确的地址");
                return;
            }
            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }
            var writePriority = uint.Parse(cmbPriority.SelectedItem.ToString());
            await PresentValueAsync(bacnet.Address, rpop.ObjectId, writePriority);
        }

        /// <summary>
        /// 释放值
        /// </summary>
        /// <param name="bacnetAddress"></param>
        /// <param name="objectId"></param>
        /// <param name="address"></param>
        /// <param name="writePriority"></param>
        private async Task PresentValueAsync(BacnetAddress bacnetAddress, BacnetObjectId objectId, uint writePriority)
        {
            List<BacnetValue> nullValue = new List<BacnetValue>() { new BacnetValue(null) };
            string address = $"{objectId.Instance}_{(uint)objectId.Type}";
            int retry = 0;//重试
        tag_retry:
            try
            {
                await Task.Delay(retry * 200);
                Bacnet_client.WritePriority = writePriority;
                Bacnet_client.WritePropertyRequest(bacnetAddress, objectId, BacnetPropertyIds.PROP_PRESENT_VALUE, nullValue);
                ShwoText(string.Format("[释放成功][{0}] 点:{1,-15} 优先级[{2}]", retry, address, writePriority));
            }
            catch (Exception ex)
            {
                retry++;
                if (retry < 2) goto tag_retry;//强行重试                
                ShwoText(string.Format(":[释放失败][{0}] 点:{1,-15} 优先级[{2}] {3}", retry, address, writePriority, ex.Message));
            }
        }

        private void btnLocal_Click(object sender, RoutedEventArgs e)
        {
            new BACnetServer().Start(cmbIP.SelectedItem.ToString());
            btnReScan_Click(null, null);
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            Write_ClickAsync(sender, e);
        }

        private void btnRelease_Click(object sender, RoutedEventArgs e)
        {
            btnRelease_ClickAsync(sender,e);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            but_export_Click(sender,e);
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            Read_ClickAsync(sender,e);
        }

        private void cmbPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bacnet_client != null)
            {
                Bacnet_client.WritePriority = uint.Parse(cmbPriority.SelectedItem.ToString());
            }                     
        }


        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if(listBox1.Items.Count==0) return;
            var ipAddress = listBox1.SelectedItem.ToString().Split(' ')[0];
            var deviceId = listBox1.SelectedItem.ToString().Split(' ')[1];
            BacNode bacnet = devicesList.Where(t => t.Address.ToString() == ipAddress && t.DeviceId.ToString() == deviceId).FirstOrDefault();
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key== Key.LeftShift)
            {
                if (System.Windows.MessageBox.Show($"确认要释放设备[{bacnet.Address}]所有点的值吗？", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    foreach (var objectId in bacnet?.Properties.Select(t => t.ObjectId))
                    {
                        for (uint i = 1; i <= 16; i++)
                        {
                             PresentValueAsync(bacnet.Address, objectId, i);
                        }
                    }
                }                
            }
            var address = txtAddress.Text?.Trim();
            var value = txtWriteValue.Text?.Trim();
            var addressPart = address.Split('_');
            BacProperty rpop = null;
            if (addressPart.Length == 1)
            {
                rpop = bacnet?.Properties.Where(t => t.Prop_Object_Name == address).FirstOrDefault();
            }
            else if (addressPart.Length == 2)
            {
                rpop = bacnet?.Properties
                    .Where(t => t.ObjectId.Instance == uint.Parse(addressPart[0]) && t.ObjectId.Type == (BacnetObjectTypes)int.Parse(addressPart[1]))
                    .FirstOrDefault();
            }
            else
            {
                Log("请输入正确的地址");
                return;
            }
            if (rpop == null)
            {
                Log("没有找到对应的点");
                return;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                for (uint i = 1; i <= 16; i++)
                {
                     PresentValueAsync(bacnet.Address, rpop.ObjectId, i);
                }
            }            
        }

        private void cmbIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_IsLoaded)
            {
                btnReScan_Click(null, null);
            }            
        }
    }
}
