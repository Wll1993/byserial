﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Util
{
    //public class LocalizeExtension : MarkupExtension
    //{
    //    public LocalizeExtension(string key)
    //    {
    //        this.Key = key;
    //    }

    //    public string Key { get; set; }

    //    public string Context { get; set; }

    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        var keyToUse = Key;
    //        if (!string.IsNullOrWhiteSpace(Context))
    //            keyToUse = $"{Context}/{Key}";

    //        var binding = new ReflectionBindingExtension($"[{keyToUse}]")
    //        {
    //            Mode = BindingMode.OneWay,
    //            Source = Localizer.Instance,
    //        };

    //        return binding.ProvideValue(serviceProvider);
    //    }
    //}
}
