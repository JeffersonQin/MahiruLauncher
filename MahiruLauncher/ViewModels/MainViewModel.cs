using MahiruLauncher.Manager;
using MahiruLauncher.Mvvm;
using ReactiveUI;

namespace MahiruLauncher.ViewModels
{
    public class MainViewModel : NotifyObject
    {
        public ScriptManager ScriptManager => ScriptManager.GetInstance();
        
        public ScriptTaskManager ScriptTaskManager => ScriptTaskManager.GetInstance();
    }
}