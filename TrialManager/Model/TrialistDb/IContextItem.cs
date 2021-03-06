﻿using System.ComponentModel;

namespace TrialManager.Model.TrialistDb
{
    public interface IContextItem : INotifyPropertyChanged
    {
        string Status { get; set; }
        string Name { get; set; }
    }
}
