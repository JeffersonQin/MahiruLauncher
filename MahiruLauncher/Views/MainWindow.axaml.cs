using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MahiruLauncher.DataModel;
using MahiruLauncher.Manager;
using MahiruLauncher.Utils;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace MahiruLauncher.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var scriptsPath = Path.Join(DirectoryUtil.GetApplicationDirectory(), "scripts.xml");
                ScriptManager.GetInstance().Scripts = Serializer.Load<ScriptManager>(scriptsPath).Scripts;
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

            foreach (var script in ScriptManager.GetInstance().Scripts)
                if (script.StartWhenAppStarts)
                    ScriptTaskManager.AddAndStartScriptTask(new ScriptTask(script));
#if DEBUG
            this.AttachDevTools();
#endif
        }
        
        protected override void OnClosed(EventArgs e)
        {
            MahiruServer.StopServer();
            base.OnClosed(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            MahiruServer.StartServer();
        }

        private void NewScriptHandler(object? sender, RoutedEventArgs e)
        {
            var window = new NewScriptWindow();
            window.ShowDialog(this);
        }

        private void RunScript(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var script = button?.DataContext as DataModel.Script;
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
            var button = sender as Button;
            var script = button?.DataContext as DataModel.Script;
            var window = new ScriptEditorWindow()
            {
                DataContext = script
            };
            window.ShowDialog(this);
        }

        private void DeleteTask(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var task = button?.DataContext as ScriptTask;
                ScriptTaskManager.KillScriptTask(task);
                ScriptTaskManager.GetInstance().ScriptTasks.Remove(task);
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

        private void KillTask(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var task = button?.DataContext as ScriptTask;
                ScriptTaskManager.KillScriptTask(task);
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

        private void OpenOutputLog(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var task = button?.DataContext as ScriptTask;
                new Process
                {
                    StartInfo = new ProcessStartInfo(task.OutputFilePath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
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
        
        private void OpenErrorLog(object? sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var task = button?.DataContext as ScriptTask;
                new Process
                {
                    StartInfo = new ProcessStartInfo(task.ErrorFilePath)
                    {
                        UseShellExecute = true
                    }
                }.Start();
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

        private void SaveScripts(object? sender, RoutedEventArgs e)
        {
            try
            {
                var scriptsPath = Path.Join(DirectoryUtil.GetApplicationDirectory(), "scripts.xml");
                File.WriteAllText(scriptsPath, ScriptManager.GetInstance().Serialize());
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
