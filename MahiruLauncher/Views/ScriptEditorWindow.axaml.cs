using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MahiruLauncher.DataModel;
using MahiruLauncher.Utils;

namespace MahiruLauncher.Views
{
    public class ScriptEditorWindow : Window
    {
        public ScriptEditorWindow()
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

        private void AddArgument(object? sender, RoutedEventArgs e)
        {
            var vm = DataContext as Script;
            vm?.DefaultArguments.Add(new ScriptArgument("ARG_NAME", "ARG_VALUE"));
            vm?.RaisePropertyChanged(nameof(vm.DefaultArguments));
        }


        private void DeleteAllArguments(object? sender, RoutedEventArgs e)
        {
            var vm = DataContext as Script;
            vm?.DefaultArguments.Clear();
            vm?.RaisePropertyChanged(nameof(vm.DefaultArguments));
        }

        private void DeleteArgument(object? sender, RoutedEventArgs e)
        {
            var vm = DataContext as Script;
            if ((sender as Button)?.DataContext is not ScriptArgument arg) return;
            vm?.DefaultArguments.Remove(arg);
            vm?.RaisePropertyChanged(nameof(vm.DefaultArguments));
        }

        private void RedirectStreamChecked(object? sender, RoutedEventArgs e)
        {
            (DataContext as Script)!.UseShellExecute = false;
        }

        private async void ExportScript(object? sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog();
                dialog.DefaultExtension = ".xml";
                dialog.Filters.Add(new FileDialogFilter() { Extensions = new List<string> { "xml" }, Name = "XML" });
                dialog.Title = "Export Script";
                dialog.InitialFileName = "script.xml";
                var fileName = await dialog.ShowAsync(this);
                if (fileName != null)
                    try
                    {
                        File.WriteAllText(fileName, (DataContext as Script).Serialize());
                    }
                    catch (Exception eex)
                    {
                        ExceptionHandler.ShowExceptionMessage(eex);
                    }
            }
            catch (Exception ex)
            {
                ExceptionHandler.ShowExceptionMessage(ex);
            }
        }
    }
}