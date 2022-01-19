using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MahiruLauncher.DataModel;
using MahiruLauncher.Manager;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace MahiruLauncher.Views
{
    public class ScriptControl : UserControl
    {
        public ScriptControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            AddHandler(DragDrop.DropEvent, Drop);
        }
        
        private void Drop(object? sender, DragEventArgs e)
        {
            if (DataContext is not Script script) return;
            if (string.IsNullOrEmpty(script.DragAndDropField)) return;
            var data = "";
            if (e.Data.Contains(DataFormats.Text))
            {
                data = e.Data.GetText();
            } else if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = e.Data.GetFileNames();
                if (files == null) return;
                var enumerable = files as string[] ?? files.ToArray();
                if (!enumerable.Any()) return;
                data = string.Join(" ", enumerable);
            }
            else return;
            if (data == "") return;
            ScriptTaskManager.AddAndStartScriptTask(new ScriptTask(script, new List<ScriptArgument>()
            {
                new(script.DragAndDropField, data)
            }));
        }

        private void RunScript(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var script = button?.DataContext as Script;
                ScriptTaskManager.AddAndStartScriptTask(new ScriptTask(script));
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

        private void EditScript(object? sender, RoutedEventArgs e)
        {
            var script = DataContext as Script;
            var window = new ScriptEditorWindow()
            {
                DataContext = script
            };
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                window.ShowDialog(desktop.MainWindow);
            }
        }
        
        private void DeleteScript(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var script = button?.DataContext as Script;
                ScriptManager.GetInstance().Scripts.Remove(script);
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