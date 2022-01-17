using MahiruLauncher.Mvvm;

namespace MahiruLauncher.DataModel
{
    public class ScriptArgument : NotifyObject
    {
        private string _name = "";

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        private string _value = "";
        
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        
        public ScriptArgument() {}
        
        public ScriptArgument(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}