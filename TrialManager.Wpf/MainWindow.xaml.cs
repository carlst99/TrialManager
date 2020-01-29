﻿using IntraMessaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using MvvmCross;
using MvvmCross.Platforms.Wpf.Views;
using System;
using System.Windows;
using TrialManager.Core.Model.Messages;

namespace TrialManager.Wpf
{
    public partial class MainWindow : MvxWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            IIntraMessenger messenger = Mvx.IoCProvider.Resolve<IIntraMessenger>();
            messenger.Subscribe(OnMessage, new Type[] { typeof(DialogMessage), typeof(FileDialogMessage) });
        }

        private void OnMessage(IMessage message)
        {
            if (message is DialogMessage dMessage)
            {
                MessageBoxButton buttons = MessageBoxButton.OK;
                if ((dMessage.Buttons & DialogMessage.DialogButton.Ok) != 0)
                {
                    if ((dMessage.Buttons & DialogMessage.DialogButton.Cancel) != 0)
                        buttons = MessageBoxButton.OKCancel;
                }
                else if ((dMessage.Buttons & DialogMessage.DialogButton.Yes) != 0)
                {
                    if ((dMessage.Buttons & DialogMessage.DialogButton.Cancel) != 0)
                        buttons = MessageBoxButton.YesNoCancel;
                    else
                        buttons = MessageBoxButton.YesNo;
                }

                MessageBoxResult result = Dispatcher.Invoke(() => MessageBox.Show(dMessage.Content, dMessage.Title, buttons));

                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.Cancel);
                        break;
                    case MessageBoxResult.No:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.No);
                        break;
                    case MessageBoxResult.OK:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.Ok);
                        break;
                    case MessageBoxResult.Yes:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.Yes);
                        break;
                }
            }
            else if (message is FileDialogMessage fMessage)
            {
                if (fMessage.Type == FileDialogMessage.DialogType.File)
                {
                    CommonOpenFileDialog cofd = new CommonOpenFileDialog
                    {
                        Title = fMessage.Title,
                        EnsurePathExists = true,
                        Multiselect = false
                    };
                    cofd.Filters.Add(new CommonFileDialogFilter("CSV Files", "*.csv"));
                    cofd.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));

                    CommonFileDialogResult result = cofd.ShowDialog();
                    cofd.Dispose();
                    if (result == CommonFileDialogResult.Ok)
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Succeeded, cofd.FileName);
                    else
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Failed, null);
                }
                else if (fMessage.Type == FileDialogMessage.DialogType.FileSave)
                {
                    CommonSaveFileDialog cosd = new CommonSaveFileDialog
                    {
                        Title = fMessage.Title,
                        EnsurePathExists = true,
                        DefaultExtension = ".csv",
                        DefaultFileName = "Draw"
                    };
                    cosd.Filters.Add(new CommonFileDialogFilter("CSV Files", "*.csv"));
                    cosd.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));

                    CommonFileDialogResult result = cosd.ShowDialog();
                    cosd.Dispose();
                    if (result == CommonFileDialogResult.Ok)
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Succeeded, cosd.FileName);
                    else
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Failed, null);
                }
            }
        }
    }
}
