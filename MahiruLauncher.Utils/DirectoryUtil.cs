using System;
using System.IO;

namespace MahiruLauncher.Utils
{
    public static class DirectoryUtil
    {
        public static string GetApplicationDirectory()
        {
            return IOUtils.EnsureDirectoryExist(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MahiruLauncher"));
        }
        
        public static string GetLogDirectory()
        {
            return IOUtils.EnsureDirectoryExist(Path.Join(GetApplicationDirectory(), "./Log"));
        }

        public static string GetScriptDirectory()
        {
            return IOUtils.EnsureDirectoryExist(Path.Join(GetApplicationDirectory(), "./Scripts"));
        }
        
        public static string GetRealWorkingDirectory(string directory)
        {
            if (Path.IsPathRooted(directory)) return directory;
            var defaultPath = ""; // TODO: settings 里面要写
            if (string.IsNullOrEmpty(defaultPath)) defaultPath = GetScriptDirectory();
            return Path.Join(defaultPath, directory);
        }
    }
}
