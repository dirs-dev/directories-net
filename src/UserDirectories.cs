using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DWG.Directories
{
    public class UserDirectories
    {
        public string HomeDir;
        public string AudioDir;
        public string DesktopDir;
        public string DocumentDir;
        public string DownloadDir;
        public string FontDir;
        public string PictureDir;
        public string PublicDir;
        public string TemplateDir;
        public string VideoDir;

        public UserDirectories()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    AudioDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    FontDir = null;
                    DesktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    DownloadDir = GetPath("{374DE290-123F-4565-9164-39C4925E467B}");
                    PictureDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    PublicDir = GetPath("{DFDF76A2-C82A-4D63-906A-5644AC457385}");
                    TemplateDir = GetPath("{A63293E8-664E-48DB-A079-DF759E0509F7}");
                    VideoDir = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    break;
                case PlatformID.Unix:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    AudioDir = GetXDGUserDir("MUSIC");
                    DesktopDir = GetXDGUserDir("DESKTOP");
                    DocumentDir = GetXDGUserDir("DOCUMENTS");
                    DownloadDir = GetXDGUserDir("DOWNLOAD");
                    var data_home = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                    if (!string.IsNullOrEmpty(data_home))
                    {
                        FontDir = Path.Join(data_home, "fonts");
                    }
                    else if (!string.IsNullOrEmpty(HomeDir))
                    {
                        FontDir = Path.Join(HomeDir, ".local/share/fonts");
                    }
                    else
                    {
                        FontDir = null;
                    }
                    PictureDir = GetXDGUserDir("PICTURES");
                    PublicDir = GetXDGUserDir("PUBLICSHARE");
                    TemplateDir = GetXDGUserDir("TEMPLATES");
                    VideoDir = GetXDGUserDir("VIDEOS");
                    break;
                case PlatformID.MacOSX:
                    HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    AudioDir = Path.Join(HomeDir, "Music");
                    DesktopDir = Path.Join(HomeDir, "Desktop");
                    DocumentDir = Path.Join(HomeDir, "Documents");
                    DownloadDir = Path.Join(HomeDir, "Downloads");
                    FontDir = Path.Join(HomeDir, "Library/Fonts");
                    PictureDir = Path.Join(HomeDir, "Pictures");
                    TemplateDir = null;
                    VideoDir = Path.Join(HomeDir, "Movies");
                    break;
                default:
                    throw new UnsupportedOperatingSystemException("User directories are not supported on this platform");
            }
        }

        private static string GetXDGUserDir(string dirname)
        {
            var process = new Process();
            process.StartInfo.FileName = "xdg-user-dir";
            process.StartInfo.ArgumentList.Add(dirname);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode > 0)
            {
                throw new ExternalException("Failed to get user dir");
            }
            return output.Trim();
        }

        private static string GetPath(string guid)
        {
            int result = SHGetKnownFolderPath(new Guid(guid),
                (uint)0, new IntPtr(0), out IntPtr outPath);
            if (result >= 0)
            {
                string path = Marshal.PtrToStringUni(outPath);
                Marshal.FreeCoTaskMem(outPath);
                return path;
            }
            else
            {
                throw new ExternalException("Unable to retrieve the known folder path. It may not "
                    + "be available on this system.", result);
            }
        }

        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken,
            out IntPtr ppszPath);
    }
}
