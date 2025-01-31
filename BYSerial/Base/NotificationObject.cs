﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Base
{
    public class NotificationObject : INotifyPropertyChanged
     {
         public event PropertyChangedEventHandler PropertyChanged;
 
         public void RaisePropertyChanged([CallerMemberName] string property="")
         {
            if (PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
         }      
    }
}
