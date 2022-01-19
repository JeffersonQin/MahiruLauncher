using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MahiruLauncher.DataModel;
using MahiruLauncher.Manager;
using MahiruLauncher.Utils;
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
                ExceptionHandler.ShowExceptionMessage(ex);
            }
        }
    }
}