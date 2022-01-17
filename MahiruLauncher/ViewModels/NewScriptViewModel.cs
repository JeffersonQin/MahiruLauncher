using MahiruLauncher.Mvvm;

namespace MahiruLauncher.ViewModels
{
    public class NewScriptViewModel : NotifyObject
    {
        private string _name = "";

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        private string _description = "";
        
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _identifier = "";
        
        public string Identifier
        {
            get => _identifier;
            set => SetProperty(ref _identifier, value);
        }
    }
}