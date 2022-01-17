using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MahiruLauncher.DataModel;
using MahiruLauncher.Manager;
using MahiruLauncher.ViewModels;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace MahiruLauncher.Views
{
    public class NewScriptWindow : Window
    {
        public NewScriptWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CancelHandler(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DoneHandler(object? sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as NewScriptViewModel;
                ScriptManager.AddScript(new Script(viewModel?.Name, viewModel?.Identifier, viewModel?.Description));
                Close();
            }
            catch (Exception ex)
            {
                var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow(new MessageBoxStandardParams{
                        ButtonDefinitions = ButtonEnum.Ok,
                        ContentTitle = "Error",
                        FontFamily = "Microsoft YaHei,Simsun,苹方-简,宋体-简",
                        ContentMessage = ex.Message + "\n" + ex.StackTrace
                    });
                msBoxStandardWindow.Show();
            }
        }
    }
}