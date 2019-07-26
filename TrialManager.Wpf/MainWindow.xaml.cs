using AmRoMessageDialog;
using IntraMessaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using MvvmCross;
using MvvmCross.Platforms.Wpf.Views;
using System;
using TrialManager.Core.Model.Messages;

namespace TrialManager.Wpf
{
    public partial class MainWindow : MvxWindow
    {
        private readonly AmRoMessageBox _messageBox;

        public MainWindow()
        {
            InitializeComponent();

            _messageBox = new AmRoMessageBox()
            {
                ShowMessageWithEffect = true,
                EffectArea = this,
                ParentWindow = this
            };

            IIntraMessenger messenger = Mvx.IoCProvider.Resolve<IIntraMessenger>();
            messenger.Subscribe(OnMessage, new Type[] { typeof(DialogMessage) });
        }

        private void OnMessage(IMessage message)
        {
            if (message is DialogMessage dMessage)
            {
                AmRoMessageBoxButton buttons = AmRoMessageBoxButton.Ok;
                if ((dMessage.Buttons & DialogMessage.DialogButton.Ok) != 0)
                {
                    if ((dMessage.Buttons & DialogMessage.DialogButton.Cancel) != 0)
                        buttons = AmRoMessageBoxButton.OkCancel;
                    else
                        buttons = AmRoMessageBoxButton.Ok;
                }
                else if ((dMessage.Buttons & DialogMessage.DialogButton.Yes) != 0)
                {
                    if ((dMessage.Buttons & DialogMessage.DialogButton.Cancel) != 0)
                        buttons = AmRoMessageBoxButton.YesNoCancel;
                    else
                        buttons = AmRoMessageBoxButton.YesNo;
                }

                AmRoMessageBoxResult result = Dispatcher.Invoke(() => _messageBox.Show(dMessage.Content, dMessage.Title, buttons));

                switch (result)
                {
                    case AmRoMessageBoxResult.Cancel:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.Cancel);
                        break;
                    case AmRoMessageBoxResult.No:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.No);
                        break;
                    case AmRoMessageBoxResult.Ok:
                        dMessage.Callback?.Invoke(DialogMessage.DialogButton.Ok);
                        break;
                    case AmRoMessageBoxResult.Yes:
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
                    cofd.Filters.Add(new CommonFileDialogFilter("CSV Files", ".csv"));

                    CommonFileDialogResult result = cofd.ShowDialog();
                    if (result == CommonFileDialogResult.Ok)
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Succeeded, cofd.FileName);
                    else
                        fMessage.Callback?.Invoke(FileDialogMessage.DialogResult.Failed, null);
                }
            }
        }
    }
}
