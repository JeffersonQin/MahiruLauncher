using System;
using System.Collections.ObjectModel;
using System.Linq;
using MahiruLauncher.DataModel;
using MahiruLauncher.Mvvm;

namespace MahiruLauncher.Manager
{
    public class ScriptManager : NotifyObject
    {
        private static readonly object Locker = new object();
        
        private static ScriptManager _instance;

        public static ScriptManager GetInstance()
        {
            lock (Locker)
            {
                _instance ??= new ScriptManager();
            }
            return _instance;
        }
        
        private ObservableCollection<Script> _scripts = new ObservableCollection<Script>();
        
        public ObservableCollection<Script> Scripts
        {
            get => _scripts;
            set => SetProperty(ref _scripts, value);
        }

        public static Script GetScript(string identifier)
        {
            var ret = GetInstance().Scripts.FirstOrDefault(script => script.Identifier == identifier);
            if (ret == null) throw new Exception("Script not found with identifier: " + identifier);
            return ret;
        }

        public static void AddScript(Script script)
        {
            if (GetInstance().Scripts.Any(s => s.Identifier == script.Identifier))
                throw new Exception("Script identifier already exist! Identifier: " + script.Identifier);
            GetInstance().Scripts.Add(script);
        }
    }
}