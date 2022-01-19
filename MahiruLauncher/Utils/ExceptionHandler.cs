using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahiruLauncher.Utils
{
    public static class ExceptionHandler
    {
        public static void ShowExceptionMessage(Exception e)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = "Error",
                FontFamily = "Microsoft YaHei,Simsun,苹方-简,宋体-简",
                ContentMessage = e.Message + "\n" + e.StackTrace
            });
            msBoxStandardWindow.Show();
        }
    }
}
