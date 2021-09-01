using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Directories.Net
{
    public class ProjectDirectories
    {
        public string ProjectPath { get; set; }
        public string CacheDir { get; set; }
        public string ConfigDir { get; set; }
        public string DataDir { get; set; }
        public string DataLocalDir { get; set; }
        public string PreferenceDir { get; set; }
        public string RuntimeDir { get; set; }

        private ProjectDirectories() { }

        public static ProjectDirectories FromPath(string path)
        {
            var dirs = new ProjectDirectories();
            dirs.ProjectPath = path;
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    dirs.DataDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path, "data");
                    dirs.DataLocalDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path, "data");
                    dirs.ConfigDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path, "config");
                    dirs.CacheDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path, "cache");
                    dirs.PreferenceDir = dirs.ConfigDir;
                    break;
                case PlatformID.Unix:
                    var cache_home = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
                    if (!string.IsNullOrEmpty(cache_home))
                    {
                        dirs.CacheDir = Path.Join(cache_home, path);
                    }
                    else
                    {
                        dirs.CacheDir = Path.Join(homeDir, ".cache", path);
                    }
                    var config_home = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                    if (!string.IsNullOrEmpty(config_home))
                    {
                        dirs.ConfigDir = Path.Join(config_home, path);
                    }
                    else
                    {
                        dirs.ConfigDir = Path.Join(homeDir, ".config", path);
                    }
                    var data_home = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                    if (!string.IsNullOrEmpty(data_home))
                    {
                        dirs.DataDir = Path.Join(data_home, path);
                    }
                    else
                    {
                        dirs.DataDir = Path.Join(homeDir, ".local/share", path);
                    }
                    dirs.DataLocalDir = dirs.DataDir;
                    dirs.PreferenceDir = dirs.ConfigDir;
                    var run_dir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
                    if (!string.IsNullOrEmpty(run_dir))
                    {
                        dirs.RuntimeDir = Path.Join(run_dir, path);
                    }
                    else
                    {
                        dirs.RuntimeDir = null;
                    }
                    break;
                case PlatformID.MacOSX:
                    dirs.CacheDir = Path.Join(homeDir, "Library/Caches", path);
                    dirs.ConfigDir = Path.Join(homeDir, "Library/Application Support", path);
                    dirs.DataDir = Path.Join(homeDir, "Library/Application Support", path);
                    dirs.DataLocalDir = dirs.DataDir;
                    dirs.PreferenceDir = Path.Join(homeDir, "Library/Preferences", path);
                    break;
                default:
                    throw new UnsupportedOperatingSystemException("Project directories are not supported on this platform");
            }
            return dirs;
        }

        public static ProjectDirectories From(string qualifier, string organization, string application)
        {
            string path = null;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    if (!string.IsNullOrEmpty(organization))
                    {
                        path = organization;
                        if (!string.IsNullOrEmpty(application))
                        {
                            path = Path.Join(path, application);
                            break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(application))
                    {
                        path = application;
                    }
                    break;
                case PlatformID.Unix:
                    path = Slugify(application);
                    break;
                case PlatformID.MacOSX:
                    path = "";
                    if (!string.IsNullOrEmpty(qualifier))
                    {
                        path += Slugify(qualifier);
                        if (!string.IsNullOrEmpty(organization) || !string.IsNullOrEmpty(application))
                        {
                            path += ".";
                        }
                    }
                    if (!string.IsNullOrEmpty(organization))
                    {
                        path += Slugify(organization);
                        if (!string.IsNullOrEmpty(application))
                        {
                            path += ".";
                        }
                    }
                    if (!string.IsNullOrEmpty(application))
                    {
                        path += Slugify(application);
                    }
                    break;
                default:
                    throw new UnsupportedOperatingSystemException("Project directories are not supported on this platform");
            }
            return FromPath(path);
        }

        private static string Slugify(string str)
        {
            return new Regex("\\s+").Replace(str, "-").ToLowerInvariant();
        }
    }
}
