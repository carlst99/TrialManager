﻿using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Views;

namespace UIConcepts.Wpf
{
    public partial class App : MvxApplication
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<Setup>();
        }
    }
}