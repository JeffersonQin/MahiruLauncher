using System;
using System.Collections.Generic;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using MahiruLauncher.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MahiruLauncher.Manager
{
    public static class MahiruServer
    {
        private static readonly WebServer Server = new WebServer(46357); // TODO: port写进settings

        private static bool _startFlag = false;
        
        public static void StartServer()
        {
            if (_startFlag) return;
            _startFlag = true;
            Server.WithWebApi("/", m => m.WithController<ScriptController>());
            Server.RunAsync();
        }

        public static void StopServer()
        {
            Server.Dispose();
        }
    }

    internal class Response 
    {
        public string Status { get; set; }
        
        public string Message { get; set; }
        
        public Response(string status, string message)
        {
            Status = status;
            Message = message;
        }
    }
    
    internal class ScriptController : WebApiController
    {
        [Route(HttpVerbs.Get, "/finish/{id}")]
        public Response ReportFinish(string id)
        {
            try
            {
                foreach (var task in ScriptTaskManager.GetInstance().ScriptTasks)
                {
                    if (task.TaskIdentifier != id) continue;
                    task.Status = ScriptStatus.Success;
                    return new Response("success", "");
                }
                return new Response("error", "task not found");
            }
            catch (Exception e)
            {
                return new Response("error", e.Message + "\n" + e.StackTrace);
            }
        }
        
        [Route(HttpVerbs.Post, "/start")]
        public Response StartTask([FormField("id")] string identifier, [FormField("arguments")] string arguments)
        {
            try
            {
                var script = ScriptManager.GetScript(identifier);
                var args = JObject.Parse(arguments);
                var customArgs = new List<ScriptArgument>();
                foreach (var (key, value) in args)
                    customArgs.Add(new ScriptArgument(key, value.ToString()));
                ScriptTaskManager.AddAndStartScriptTask(new ScriptTask(script, customArgs));
                return new Response("success", "");
            }
            catch (Exception e)
            {
                return new Response("error", e.Message + "\n" + e.StackTrace);
            }
        }
    }
}