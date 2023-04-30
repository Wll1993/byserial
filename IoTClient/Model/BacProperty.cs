using IoTClient.Enums;
using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTClientDeskTop.Model
{
    public class BacProperty
    {
        public BacnetObjectId ObjectId { get; set; }
        /// <summary>
        /// 点名
        /// </summary>
        public string Prop_Object_Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Prop_Present_Value { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public DataTypeEnum Prop_DataType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Prop_Description { get; set; }
    }
}
