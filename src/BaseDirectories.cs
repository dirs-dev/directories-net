using System;
using System.IO;

namespace Directories.Net
{
    public class BaseDirectories
    {
        public string HomeDir { get; set; }
        public string CacheDir { get; set; }
        public string ConfigDir { get; set; }
        public string DataDir { get; set; }
        public string DataLocalDir { get; set; }
        public string ExecutableDir { get; set; }
        public string PreferenceDir { get; set; }
        public string RuntimeDir { get; set; }

        public BaseDirectories()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    DataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    DataLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    ConfigDir = DataDir;
                    CacheDir = DataLocalDir;
                    ExecutableDir = null;
                    PreferenceDir = ConfigDir;
                    RuntimeDir = null;
                    break;
                case PlatformID.Unix:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    CacheDir = Environment.GetEnvironmentVariable("XDG_CACHE_HOME") ?? Path.Join(HomeDir, ".cache");
                    ConfigDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ?? Path.Join(HomeDir, ".config");
                    DataDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME") ?? Path.Join(HomeDir, ".local/share");
                    DataLocalDir = DataDir;
                    var xdg_bin = Environment.GetEnvironmentVariable("XDG_BIN_HOME");
                    if (!string.IsNullOrEmpty(xdg_bin))
                    {
                        ExecutableDir = xdg_bin;
                    }
                    else if (!string.IsNullOrEmpty(DataDir))
                    {
                        ExecutableDir = Path.Join(DataDir, "../bin");
                    }
                    else if (!string.IsNullOrEmpty(HomeDir))
                    {
                        ExecutableDir = Path.Join(HomeDir, ".local/bin");
                    }
                    PreferenceDir = ConfigDir;
                    var xdg_run = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
                    if (!string.IsNullOrEmpty(xdg_run))
                    {
                        RuntimeDir = xdg_run;
                    }
                    else if (!string.IsNullOrEmpty(DataDir))
                    {
                        RuntimeDir = Path.Join(DataDir, "../bin");
                    }
                    else if (!string.IsNullOrEmpty(HomeDir))
                    {
                        RuntimeDir = Path.Join(HomeDir, ".local/bin");
                    }
                    break;
                case PlatformID.MacOSX:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    CacheDir = Path.Join(HomeDir, "Library/Caches");
                    ConfigDir = Path.Join(HomeDir, "Library/Application Support");
                    DataDir = ConfigDir;
                    DataLocalDir = ConfigDir;
                    ExecutableDir = null;
                    PreferenceDir = Path.Join(HomeDir, "Library/Preferences");
                    RuntimeDir = null;
                    break;
                default:
                    throw new UnsupportedOperatingSystemException("Base directories are not supported on this platform");
            }
        }
    }

    public class UnsupportedOperatingSystemException : Exception
    {
        public UnsupportedOperatingSystemException(string message)
            : base(message)
        { }
    }
}
