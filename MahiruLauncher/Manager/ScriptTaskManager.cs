using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Avalonia.Threading;
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
            try
            {
                using (var writer = File.AppendText(fileName))
                {
                    writer.AutoFlush = true;
                    for (; ; )
                    {
                        var line = stream.ReadLineWithEnding();
                        if (stream.EndOfStream) break;
                        writer.Write(line);
                        Debug.Write(line);
                    }
                }
            }
            catch (Exception e)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ExceptionHandler.ShowExceptionMessage(e);
                });
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

            if (script.RedirectStreams)
            {
                scriptTask.Process.StartInfo.RedirectStandardError = true;
                scriptTask.Process.StartInfo.RedirectStandardOutput = true;
            }

            var stdoutWriter = MakeThread(() =>
            {
                WriteStreamToFile(scriptTask.OutputFilePath, scriptTask.Process.StandardOutput);
            }, "stdout-" + script.Identifier + "-" + timestamp);
            var stderrWriter = MakeThread(() =>
            {
                WriteStreamToFile(scriptTask.ErrorFilePath, scriptTask.Process.StandardError);
            }, "stderr-" + script.Identifier + "-" + timestamp);
            scriptTask.Process.Start();

            if (script.RedirectStreams)
            {
                stdoutWriter.Start();
                stderrWriter.Start();
            }

            scriptTask.Status = ScriptStatus.Running;
            MakeThread(() =>
            {
                try
                {
                    scriptTask.Process.WaitForExit();
                    scriptTask.EndTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                    if (scriptTask.Status == ScriptStatus.Running)
                        scriptTask.Status = ScriptStatus.Error;
                    if (script.RedirectStreams)
                    {
                        stdoutWriter.Join();
                        stderrWriter.Join();
                    }

                    if (scriptTask.Status != ScriptStatus.Error) return;

                    var mySmtpClient = new SmtpClient(Properties.Settings.Default.SmtpHost);
                    mySmtpClient.Port = Properties.Settings.Default.SmtpPort;

                    // set smtp-client with basicAuthentication
                    mySmtpClient.EnableSsl = Properties.Settings.Default.SmtpEnableSsl;
                    mySmtpClient.UseDefaultCredentials = false;
                    System.Net.NetworkCredential basicAuthenticationInfo = new
                       System.Net.NetworkCredential(Properties.Settings.Default.SmtpUsername, Properties.Settings.Default.SmtpPassword);
                    mySmtpClient.Credentials = basicAuthenticationInfo;
                    mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // add from,to mailaddresses
                    var mailAddress = new MailAddress(Properties.Settings.Default.EmailAddress);
                    var myMail = new MailMessage();
                    myMail.From = mailAddress;
                    myMail.To.Add(Properties.Settings.Default.EmailAddress);
                    myMail.Priority = MailPriority.Normal;

                    // set subject and encoding
                    myMail.Subject = "[MahiruLauncher] Error Occurred during Task Execution";
                    myMail.SubjectEncoding = Encoding.UTF8;

                    // set body-message and encoding
                    var argumentString = "<ul>\n";
                    foreach (var arg in scriptTask.ScriptArguments)
                        argumentString += "<li>" + arg.Name + ": " + arg.Value + "</li>\n";
                    argumentString += "</ul>";

                    myMail.Body = string.Format(
                        "<ul>\n" +
                        "<li>Script Name:{0}</li>\n" +
                        "<li>Script Description:{1}</li>\n" +
                        "<li>Script Identifier:{2}</li>\n" +
                        "<li>Task Identifier:{3}</li>\n" +
                        "<li>Working Directory:{4}</li>\n" +
                        "<li>Process Name:{5}</li>\n" +
                        "<li>Start Time:{6}</li>\n" +
                        "<li>End Time:{7}</li>\n" +
                        "<li>Task Arguments:\n{8}</li>\n" +
                        "</ul>\n" + 
                        "<p>Output and error logs are attached if available.</p>", 
                        script.Name, 
                        script.Description, 
                        script.Identifier, 
                        scriptTask.TaskIdentifier, 
                        script.WorkingDirectory,
                        script.ProcessName,
                        DateTimeOffset.FromUnixTimeMilliseconds(scriptTask.StartTime).ToString("s"),
                        DateTimeOffset.FromUnixTimeMilliseconds(scriptTask.EndTime).ToString("s"),
                        argumentString
                    );
                    myMail.BodyEncoding = Encoding.UTF8;
                    // text or html
                    myMail.IsBodyHtml = true;

                    if (script.RedirectStreams)
                    {
                        myMail.Attachments.Add(new Attachment(scriptTask.OutputFilePath));
                        myMail.Attachments.Add(new Attachment(scriptTask.ErrorFilePath));
                    }

                    mySmtpClient.Send(myMail);
                }
                catch (Exception e)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ExceptionHandler.ShowExceptionMessage(e);
                    });
                }
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
