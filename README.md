<h1 align="center">
BYSerial 
</h1>

## 微软Windows下的GUI串口调试工具  

- 欢迎使用BYSerial，这是一个用于Microsoft Windows的GUI串口调试工具。
- 有通用串口调试工具的一般功能。  
- 提供了编译好的版本，既可以参考源码编写自己的程序，又可以直接试用此工具。
- 如果您有任何问题，请在Issues发布，我将在方便的时候处理。谢谢你的使用。
- 欢迎提出想要的常用功能，方便的话，将添加上去。  
- 如果感兴趣，欢迎Watch和Star。  
  
## 系统环境  

已测试环境：  
Microsoft Window: WIN10 家庭中文版  版本2004 (内部版本19041)  
Microsoft Window: WIN10 专业版  版本20H2 (内部版本19042)

## 引用的第三方库  

HandyControl  https://gitee.com/handyorg/HandyControl  
Newtonsoft.Json https://github.com/JamesNK/Newtonsoft.Json  
LiveCharts https://v0.lvcharts.com/  

## 赞助者
[![](Img/张海long.png)](https://gitee.com/zhang-hai-long325520)
[![](Img/myyangang.png)](https://gitee.com/myyangang)
![](Img/y.png)
![](Img/Lf.png)
![](Img/顧.png)
  
界面
----
![](Img/main_ch2.png)    
![](Img/main_ch.png)    
![](Img/toolbox.png)    
![](Img/toolbox2.png)    
![](Img/ascii.png)    
![](Img/opt1.png)    
![](Img/opt2.png)    
![](Img/opt3.png)    
![](Img/opt4.png)    

## 功能介绍

### 已实现功能  
1.串口通讯调试  
2.TCP通讯调试  
3.常用小工具  
4.支持中英文双语切换  
5.自动检查版本更新  
6.支持添加自定义快捷命令  
7.支持ModbusRTU/ModbusAscii/ModbusTCP调试  
  
### 串口通讯详细功能：  
1.手动检测串口设备的增减  
2.支持流控  
3.接收发送编码方式同时支持ASCII和HEX方式  
4.ASCII模式时，可设置结束符，例如回车换行等  
5.HEX模式时，支持自动计算标准ModbusRTU的CRC16  
6.发送支持循环发送  
7.接收区显示支持显示发送和显示接收，并可设置发送和接收的字符串颜色  
8.接收区显示支持显示发送和接收的时间，时间格式可自定义  
9.底部显示串口状态，总接收字节数和总发送字节数。各字节数可手动清零  
10.接收区字符串可一键清空
11.记录发送历史，支持记录最新的10条历史记录  
12.可将接收区显示的字符实时保存到本地txt文档  

### TCP通讯调试功能：  
1.支持TCP Client/TCP Server  
2.TCP Server时，可显示当前连接客户端列表  
3.TCP通讯采取异步方式通讯  
4.支持串口通讯功能中的3-12项  
5.支持UTF8编码，可发送中文  
6.不支持TCP连接断开的自动侦测  

### 小工具  
1.通用校验方法中包含常用的LRC,XOR,CheckSum,FCS,Modbus-CRC16等校验的计算  
2.数据转换包含整数和小数与16进制HEX的转换  
3.图片与base64互转  
4.数据采集中常用的模拟量与工程量转换计算  
5.ASCII码表  
6.C# 颜色对照表  
7.拾取屏幕颜色。该功能使用鼠标hook实现。通过hook技术可实现拦截或者修改键盘鼠标等的操作，对于有这方面需求的可以参考  

## 检查更新
检查更新方式：  
利用gitee作为更新检查的服务器，将版本号和下载连接写在gitee项目文件中，实现自动检查更新并提供下载连接的功能  

## 快捷命令列表  
对于调试时，常用的命令可以在此处设置，方便快速调试  

## ModbusRTU/ModbusAscii/ModbusTCP调试  
暂时嵌入第三方Modbus调试工具，后期再用C#重新实现一遍  

## 相关开源项目  
跨平台（linux/windows)串口通讯源码开源连接  
xuyuanbao/BaoYuanSerial: A GUI Serial Debug Tool for Linux/Microsfot Window (github.com)  
https://github.com/xuyuanbao/BaoYuanSerial