// dotnet run key1=value1 --key2=value2 /key3=value3 --key4 value4 /key5 value5
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CommandLineSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Enter the path of the root of all your repositories: ");
            string reposRootDir = Console.ReadLine();
            string[] subdirectoryEntries = Directory.GetDirectories(reposRootDir);
            string gitFetch = @"git fetch";
            string gitPull = @"git pull";
            string gitMergeMaster = @"git merge origin/master";

            foreach (string subdirectory in subdirectoryEntries)
            {
                try
                {
                    Console.WriteLine($"Changing Directory to: {subdirectory}");
                    //Set the current directory.
                    Directory.SetCurrentDirectory(subdirectory);
                    string checkGitRepo = "git rev-parse --is-inside-work-tree";
                    string resultCheckGitRepo = ExecuteCommandWithOutput(checkGitRepo).Replace("\n", "").Replace("\r", ""); ;
                    bool isGitRepo = resultCheckGitRepo == "true";
                    if (isGitRepo)
                    {
                        Console.WriteLine(subdirectory);
                        Console.WriteLine(subdirectory + " "+ gitFetch);
                        Console.WriteLine(ExecuteCommandWithOutput(gitFetch));
                        Console.WriteLine(subdirectory + " " + gitPull);
                        Console.WriteLine(ExecuteCommandWithOutput(gitPull));
                        Console.WriteLine(subdirectory + " " + gitMergeMaster);
                        Console.WriteLine(ExecuteCommandWithOutput(gitMergeMaster));
                    }
                    
                    
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"The specified directory does not exist. {e}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception occured while syncing code using git. {e}");
                }
            }
            Console.ReadKey();
        }

        public static string ExecuteCommandWithOutput(string command,
                                       string workingDirectory = null)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardError = procStartInfo.RedirectStandardInput = procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                if (null != workingDirectory)
                {
                    procStartInfo.WorkingDirectory = workingDirectory;
                }

                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                StringBuilder sb = new StringBuilder();
                proc.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    sb.AppendLine(e.Data);
                };
                proc.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    sb.AppendLine(e.Data);
                };

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                return sb.ToString();
            }
            catch (Exception objException)
            {
                return $"Error in command: {command}, {objException.Message}";
            }
        }
    }
}