﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MahiruLauncher.DataModel;
using MahiruLauncher.Mvvm;
using MahiruLauncher.Utils;

namespace MahiruLauncher.Manager
{
    public class ScriptTaskManager : NotifyObject
    {
        private static readonly object Locker = new object();
        
        private static ScriptTaskManager _instance;
        
        public static ScriptTaskManager GetInstance()
        {
            lock (Locker)
            {
                _instance ??= new ScriptTaskManager();
            }
            return _instance;
        }
        
        private ObservableCollection<ScriptTask> _scriptTasks = new ObservableCollection<ScriptTask>();
        
        public ObservableCollection<ScriptTask> ScriptTasks
        {
            get => _scriptTasks;
            set => SetProperty(ref _scriptTasks, value);
        }

        #region private methods
        
        private static Thread MakeThread(ThreadStart startInfo, string name)
        {
            var t = new Thread(startInfo)
            {
                IsBackground = true,
                Name = name
            };
            return t;
        }

        private static void WriteStreamToFile(string fileName, StreamReader stream)
        {
            using (var writer = File.AppendText(fileName))
            {
                writer.AutoFlush = true;
                for (;;)
                {
                    var line = stream.ReadLineWithEnding();
                    if (stream.EndOfStream) break;
                    writer.Write(line);
                    Debug.Write(line);
                    // Debug.WriteLine(line);
                }
            }
        }
        
        #endregion
        
        public static void AddAndStartScriptTask(ScriptTask scriptTask)
        {
            GetInstance().ScriptTasks.Insert(0, scriptTask);
            var script = ScriptManager.GetScript(scriptTask.ScriptIdentifier);
            if (script == null)
                throw new Exception("Script not found with identifier: " + scriptTask.ScriptIdentifier);
            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            scriptTask.Process.StartInfo.FileName = script.ProcessName;
            scriptTask.Process.StartInfo.UseShellExecute = script.UseShellExecute;
            scriptTask.Process.StartInfo.CreateNoWindow = script.CreateNoWindow;
            scriptTask.Process.StartInfo.WorkingDirectory = DirectoryUtil.GetRealWorkingDirectory(script.WorkingDirectory);
            foreach (var argument in scriptTask.ScriptArguments)
                if (argument.Name == "$[SCHEDULE_UUID]")
                    scriptTask.Process.StartInfo.ArgumentList.Add(scriptTask.TaskIdentifier);
                else
                    scriptTask.Process.StartInfo.ArgumentList.Add(argument.Value);
            
            scriptTask.StartTime = timestamp;
            scriptTask.OutputFilePath = Path.Join(DirectoryUtil.GetLogDirectory(),
                script.Identifier + "-" + timestamp + "-output.log");
            scriptTask.ErrorFilePath = Path.Join(DirectoryUtil.GetLogDirectory(),
                script.Identifier + "-" + timestamp + "-error.log");

            scriptTask.Process.StartInfo.RedirectStandardError = true;
            scriptTask.Process.StartInfo.RedirectStandardOutput = true;

            var stdoutWriter = MakeThread(() =>
            {
                WriteStreamToFile(scriptTask.OutputFilePath, scriptTask.Process.StandardOutput);
            }, "stdout-" + script.Identifier + "-" + timestamp);
            var stderrWriter = MakeThread(() =>
            {
                WriteStreamToFile(scriptTask.ErrorFilePath, scriptTask.Process.StandardError);
            }, "stderr-" + script.Identifier + "-" + timestamp);
            scriptTask.Process.Start();
            stdoutWriter.Start();
            stderrWriter.Start();
            scriptTask.Status = ScriptStatus.Running;
            MakeThread(() =>
            {
                scriptTask.Process.WaitForExit();
                if (scriptTask.Status == ScriptStatus.Running)
                    scriptTask.Status = ScriptStatus.Error;
                stdoutWriter.Join();
                stderrWriter.Join();
                // TODO: Email
            }, "monitor-" + script.Identifier + "-" + timestamp).Start();
        }

        public static void KillScriptTask(ScriptTask scriptTask)
        {
            if (scriptTask.Status == ScriptStatus.Waiting) return;
            if (scriptTask.Process == null) return;
            if (scriptTask.Process.HasExited) return;
            scriptTask.Process.Kill();
            scriptTask.Status = ScriptStatus.Killed;
        }
    }
}