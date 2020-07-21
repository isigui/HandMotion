using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HandMotion
{
    public interface IViewModel : INotifyPropertyChanged
    {
        bool IsBusy { get; set; }
    }
}
