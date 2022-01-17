using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MahiruLauncher.Mvvm;

namespace MahiruLauncher.DataModel
{
    public class Script : NotifyObject
    {
        private string _identifier;

        /// <summary>
        /// Identifier of the Script.
        /// Example: "com.example.scriptTask"
        /// </summary>
        public string Identifier
        {
            get => _identifier;
            set => SetProperty(ref _identifier, value);
        }
        
        private string _name;
        
        /// <summary>
        /// Name of the Script.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        private string _description;
        
        /// <summary>
        /// Description of the Script.
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _processName;
        
        /// <summary>
        /// Process name for the Script.
        /// </summary>
        public string ProcessName
        {
            get => _processName;
            set => SetProperty(ref _processName, value);
        }
        
        private ObservableCollection<ScriptArgument> _defaultArguments = new ObservableCollection<ScriptArgument>();
        
        /// <summary>
        /// Default arguments of the Script.
        /// </summary>
        public ObservableCollection<ScriptArgument> DefaultArguments
        {
            get => _defaultArguments;
            set => SetProperty(ref _defaultArguments, value);
        }

        private bool _redirectStreams;
        
        /// <summary>
        /// Whether to redirect streams of the Script.
        /// </summary>
        public bool RedirectStreams
        {
            get => _redirectStreams;
            set => SetProperty(ref _redirectStreams, value);
        }
        
        private bool _useShellExecute;
        
        /// <summary>
        /// Whether to use shell execute the script.
        /// </summary>
        public bool UseShellExecute
        {
            get => _useShellExecute;
            set => SetProperty(ref _useShellExecute, value);
        }

        private bool _createNoWindow;

        /// <summary>
        /// Whether to create no window during execution.
        /// </summary>
        public bool CreateNoWindow
        {
            get => _createNoWindow;
            set => SetProperty(ref _createNoWindow, value);
        }

        private bool _startWhenAppStarts;
        
        /// <summary>
        /// Whether to execute the script when app starts.
        /// </summary>
        public bool StartWhenAppStarts
        {
            get => _startWhenAppStarts;
            set => SetProperty(ref _startWhenAppStarts, value);
        }

        private string _workingDirectory;

        /// <summary>
        /// Working directory of the script.
        /// </summary>
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetProperty(ref _workingDirectory, value);
        }

        public Script(string identifier)
        {
            Identifier = identifier;
        }
        
        public Script(string name, string identifier, string description)
        {
            Name = name;
            Identifier = identifier;
            Description = description;
        }

        public Script() {}
    }
}
