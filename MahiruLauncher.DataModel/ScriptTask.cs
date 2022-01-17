using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MahiruLauncher.Mvvm;

namespace MahiruLauncher.DataModel
{
    public class ScriptTask : NotifyObject
    {
        public string TaskIdentifier { get; } = Guid.NewGuid().ToString();

        private ScriptStatus _status = ScriptStatus.Waiting;

        public ScriptStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private long _startTime;

        public long StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }
        
        private int _endTime;
        
        public int EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private string _scriptIdentifier;
        
        public string ScriptIdentifier
        {
            get => _scriptIdentifier;
            set => SetProperty(ref _scriptIdentifier, value);
        }

        private string _outputFilePath;
        
        public string OutputFilePath
        {
            get => _outputFilePath;
            set => SetProperty(ref _outputFilePath, value);
        }
        
        private string _errorFilePath;
        
        public string ErrorFilePath
        {
            get => _errorFilePath;
            set => SetProperty(ref _errorFilePath, value);
        }

        private ObservableCollection<ScriptArgument> _scriptArguments = new ObservableCollection<ScriptArgument>();
        
        public ObservableCollection<ScriptArgument> ScriptArguments
        {
            get => _scriptArguments;
            set => SetProperty(ref _scriptArguments, value);
        }

        public readonly Process Process = new Process();

        public ScriptTask(Script script, IList<ScriptArgument> scriptArguments = null)
        {
            ScriptIdentifier = script.Identifier;
            if (scriptArguments != null)
                foreach (var argument in scriptArguments)
                    ScriptArguments.Add(argument);
            else ScriptArguments = script.DefaultArguments;
        }
    }
}