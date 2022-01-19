using System;
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
        private bool _exit = false;

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
            
            App.TrayIcon.IsVisible = true;
            App.TrayIcon.ToolTipText = "MahiruLauncher";
            App.TrayIcon.Menu = new NativeMenu();
            var openingMenu = new NativeMenuItem("Open");
            openingMenu.Click += (sender, args) =>
            {
                Show();
                Activate();
            };
            var exitMenu = new NativeMenuItem("Exit");
            exitMenu.Click += (sender, args) =>
            {
                MahiruServer.StopServer();
                _exit = true;
                foreach (var task in ScriptTaskManager.GetInstance().ScriptTasks)
                    ScriptTaskManager.KillScriptTask(task);
                Environment.Exit(0);
            };
            App.TrayIcon.Menu.Items.Add(openingMenu);
            App.TrayIcon.Menu.Items.Add(new NativeMenuItemSeparator());
            App.TrayIcon.Menu.Items.Add(exitMenu);

#if DEBUG
            this.AttachDevTools();
#endif
        }
        
        protected override bool HandleClosing()
        {
            if (_exit) return false;
            Hide();
            return true;
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
                ExceptionHandler.ShowExceptionMessage(ex);
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
                ExceptionHandler.ShowExceptionMessage(ex);
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
                ExceptionHandler.ShowExceptionMessage(ex);
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
                ExceptionHandler.ShowExceptionMessage(ex);
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
    }
}
