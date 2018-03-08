using System;

using System.Runtime.CompilerServices;  // for MethodImplOptions
using System.Diagnostics; // for StackFrame
using System.IO; // for Path
using System.IO.Compression; // for ZipFile

namespace zip
{
    class Program
    {
        enum LogType { info, warning, error };

        static int _log_verbosity = 5;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Log(LogType type, int level, string message, string detail)
        {
            if (_log_verbosity < level)
                return;

            string caller_name = new StackFrame(1, true).GetMethod().Name;
            DateTime time = DateTime.Now;
            Console.WriteLine($"{time.ToString()}.{time.Millisecond.ToString()},{caller_name},{type.ToString()},{level.ToString()},{message},{detail}");
        }

        static void Main(string[] args)
        {
            string command = args.Length > 0 ? args[0] : "help";
            try
            {

                if (command.Equals("zip", StringComparison.InvariantCultureIgnoreCase))
                    Zip(args[1], args[2], true);
                else if (command.Equals("unzip", StringComparison.InvariantCultureIgnoreCase))
                    Unzip(args[1], args[2], true);

                if (!command.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                    return;
            }
            catch (System.Exception ex)
            {
                Log(LogType.error, 1, $"Could not execute command {command}", $"Inner error: {ex.Message}");
            }

            //help
            Console.WriteLine("Commands:");
            Console.WriteLine("- zip <source> <destination>\t\tE.g.: \"zip\" \"./README.md\" \"./README.zip\"");
            Console.WriteLine("- unzip <source> <destination>\t\tE.g.: \"unzip\" \"./README.md\" \"./README.zip\"");
        }

        static private bool CanonizePath(string input, out string output)
        {
            try
            {
                output = Path.GetFullPath(input);
            }
            catch (System.Exception)
            {
                output = "";
                return false;
            }

            return true;
        }

        static private bool RemoveDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" - Error: {ex.Message}");
                return false;
            }
        }
        
         static private bool RemoveFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" - Error: {ex.Message}");
                return false;
            }
        }

        static bool Zip(string source, string destination, bool replace)
        {
            try
            {
                Console.WriteLine($"Zipping {source}:");

                string source_path, destination_path;
                if (!CanonizePath(source, out source_path) || !CanonizePath(destination, out destination_path))
                {
                    Console.WriteLine($" - Invalid parameters.");
                    return false;
                }

                if (File.Exists(destination_path))
                {
                    if (!replace)
                    {
                        Console.WriteLine($" - Destination already exist.");
                        return false;
                    }

                    if (!RemoveFile(destination_path))
                    {
                        Console.WriteLine($" - Could not remove file.");
                        return false;
                    }
                }

                ZipFile.CreateFromDirectory(source_path, destination_path);
                Console.WriteLine($" - {destination} created with success!");
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" - Error: {ex.Message}");
                return false;
            }
        }

        static private bool Unzip(string source, string destination, bool replace)
        {
            try
            {
                Console.WriteLine($"Unzipping {source}:");

                string source_path, destination_path;
                if (!CanonizePath(source, out source_path) || !CanonizePath(destination, out destination_path))
                {
                    Console.WriteLine($" - Invalid parameters.");
                    return false;
                }

                if (Directory.Exists(destination_path))
                {
                    if (!replace)
                    {
                        Console.WriteLine($" - Destination already exist.");
                        return false;
                    }

                    if (!RemoveDirectory(destination_path))
                    {
                        Console.WriteLine($" - Could not remove directory.");
                        return false;
                    }
                }

                ZipFile.ExtractToDirectory(source_path, destination_path);
                Console.WriteLine($" - {destination} created with success!");
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" - Error: {ex.Message}");
                return false;
            }
        }
    }
}
