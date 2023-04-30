using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.IO;
using MQTTnet.Server;
using System.Windows.Interop;
using MQTTnet.Packets;

namespace IoTClientDeskTop.Controls
{
    /// <summary>
    /// MQTTControl.xaml 的交互逻辑
    /// </summary>
    public partial class MQTTControl : UserControl
    {
        public MQTTControl()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if(btn_Start.Content.ToString()=="启动")
            {
                but_start_ClickAsync(sender, e);
            }
            else
            {
                but_Stop_Click(sender, e);
            }
        }

        private async void btn_Subscribe_Click(object sender, RoutedEventArgs e)
        {
            // Subscribe to a topic
            var topic = txt_subscribe_topic.Text?.Trim();
            if (string.IsNullOrWhiteSpace(topic))
            {
                WriteLine_1("### 请输入Topic ###");
                return;
            }
            var mqttSubscribeOptions = factory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(topic);
                   })
               .Build();
           var result= await mqttClient.SubscribeAsync(mqttSubscribeOptions);

            WriteLine_1($"### 订阅 ###\r\n result:{result.ReasonString}");
        }

        private async void btn_Publish_Click(object sender, RoutedEventArgs e)
        {
            var topic = txt_publish_topic.Text?.Trim();
            var payload = txt_publish_payload.Text?.Trim();
            if (string.IsNullOrWhiteSpace(topic))
            {
                WriteLine_1("### 请输入Topic ###");
                return;
            }
            var applicationMessage = new MqttApplicationMessageBuilder()
                           .WithTopic(topic)
                           .WithPayload(payload)
                           .Build();
            var result = await mqttClient.PublishAsync(applicationMessage);
            
            WriteLine_2($"topic:{topic} payload:{payload} {result.ReasonCode}");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btn_Subscribe.IsEnabled = false;
            btn_Publish.IsEnabled = false;            
            txt_ClientID.Text = Guid.NewGuid().ToString();
            checkBox1_Click(null, null);
        }

        private IMqttClient mqttClient;
        private MqttFactory factory;
        private async void but_start_ClickAsync(object sender, EventArgs even)
        {
            try
            {
                but_Stop_Click(null, null);
                btn_Start.IsEnabled = false;
                factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                var mqttClientOptions = new MqttClientOptionsBuilder()
                                 .WithClientId(txt_ClientID.Text?.Trim())
                                 //.WithTcpServer(txt_Address.Text?.Trim(), int.Parse(txt_Port.Text?.Trim()))
                                 .WithCredentials(txt_UserName.Text, txt_Password.Text);

                if ((bool)checkBox1.IsChecked)
                {
                    if (!File.Exists(txt_ca_file.Text))
                    {
                        MessageBox.Show($"没有找到文件:{txt_ca_file.Text}");
                        btn_Start.IsEnabled = true;
                        return;
                    }
                    if (!File.Exists(txt_pfx_file.Text))
                    {
                        MessageBox.Show($"没有找到文件:{txt_pfx_file.Text}");
                        btn_Start.IsEnabled = true;
                        return;
                    }
                    var caCert = X509Certificate.CreateFromCertFile(txt_ca_file.Text);
                    var clientCert = new X509Certificate2(txt_pfx_file.Text);
                    mqttClientOptions = mqttClientOptions.WithTls(new MqttClientOptionsBuilderTlsParameters()
                    {
                        UseTls = true,
                        SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                        CertificateValidationHandler = (o) =>
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() => {
                                btn_Start.IsEnabled = true;
                            }));
                            return true;
                        },
                        Certificates = new List<X509Certificate>(){
                                    caCert, clientCert
                                 }
                    });
                }

                if (comboBox1.SelectedIndex == 0)
                {
                    mqttClientOptions = mqttClientOptions.WithTcpServer(txt_Address.Text?.Trim(), int.Parse(txt_Port.Text?.Trim()));
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    mqttClientOptions = mqttClientOptions.WithWebSocketServer($"{txt_Address.Text?.Trim()}:{txt_Port.Text?.Trim()}/mqtt").WithTls();
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    mqttClientOptions = mqttClientOptions.WithWebSocketServer($"{txt_Address.Text?.Trim()}:{txt_Port.Text?.Trim()}/mqtt");
                }
                var options = mqttClientOptions.Build();
                await mqttClient.ConnectAsync(options);
                mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;                
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;               
                mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            }
            catch (Exception ex)
            {
                WriteLine_1($"err：{ex.Message}");
            }
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            return Task.Run(() => {
                WriteLine_1("### 连接到服务 ###");
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btn_Publish.IsEnabled = true;
                    btn_Subscribe.IsEnabled = true;
                    btn_Start.IsEnabled = true;
                    btn_Start.Content = "停止";
                }));
            });
        }

        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            return Task.Run(() => {
                WriteLine_1("### 收到消息 ###");
                WriteLine_1($"+ Topic = {arg.ApplicationMessage.Topic}");
                try
                {
                    WriteLine_1($"+ Payload = {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
                }
                catch { }
                WriteLine_1($"+ QoS = {arg.ApplicationMessage.QualityOfServiceLevel}");
                WriteLine_1($"+ Retain = {arg.ApplicationMessage.Retain}");
                WriteLine_1();
            });
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            return Task.Run(() => {
                WriteLine_1("### 服务器断开连接 ###");            
            });
        }

        private async void but_Stop_Click(object sender, EventArgs e)
        {
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected)
                    await mqttClient.DisconnectAsync();
                mqttClient.Dispose();
            }
            btn_Subscribe.IsEnabled = false;
            btn_Publish.IsEnabled = false;
            btn_Start.Content = "启动";
        }
        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            txt_Port.Text = (bool)checkBox1.IsChecked ? "8883" : "1883";
            txt_pfx_file.IsEnabled = (bool)checkBox1.IsChecked;
            txt_ca_file.IsEnabled = (bool)checkBox1.IsChecked;
        }

        private void WriteLine_1(string msg = "")
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txt_msg.AppendText($"{msg} \r\n");
            }));            
        }
        private void WriteLine_2(string msg = "")
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txt_msg2.AppendText($"{msg} \r\n");
            }));
        }
    }
}
