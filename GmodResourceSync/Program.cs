using System;
using System.IO;
using System.Diagnostics;

namespace GmodResourceSync
{
    class Program
    {
        static bool SyncFile(string fullPath, string relativePath, string fastdlpath, string sevenzippath, bool force)
        {
            var newpath = fastdlpath + Path.DirectorySeparatorChar + relativePath + ".bz2";
            if (!File.Exists(newpath) || force)
            {
                Process process = new Process();

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.FileName = sevenzippath;
                process.StartInfo.Arguments = "u -tbzip2 \"" + newpath + "\" \"" + fullPath + "\"";

                process.Start();

                return true;
            }

            return false;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Garry's Mod Resource Synchronizer - Version 1.0 - by ibot3\n");

            if (args.Length >= 3)
            {
                var gmodpath = args[0];
                var fastdlpath = args[1];
                var sevenzippath = args[2];

                bool force = (args.Length == 4);

                Console.WriteLine("Gmod Path: {0}", gmodpath);
                Console.WriteLine("FastDL Path: {0}", fastdlpath);
                Console.WriteLine("7Zip Path: {0}", sevenzippath);

                Console.WriteLine("");

                string[] gmodAddonFiles = System.IO.Directory.GetFiles(gmodpath + Path.DirectorySeparatorChar + "addons", "*.*", System.IO.SearchOption.AllDirectories);
                string[] gmodMapFiles = System.IO.Directory.GetFiles(gmodpath + Path.DirectorySeparatorChar + "maps", "*.*", System.IO.SearchOption.AllDirectories);

                Console.WriteLine("Scanning garrysmod/addons directory...");

                foreach (string file in gmodAddonFiles)
                {
                    var basepathlength = gmodpath.Length + 7 + 1;
                    var shortpath = file.Substring(basepathlength, file.Length - basepathlength);
                    var split = shortpath.Split(Path.DirectorySeparatorChar);

                    if(split.Length > 1)
                    {
                        var addonNameLength = split[0].Length + 1;
                        shortpath = shortpath.Substring(addonNameLength, shortpath.Length - addonNameLength);

                        if (shortpath.StartsWith("materials") || shortpath.StartsWith("models") || shortpath.StartsWith("resource") || shortpath.StartsWith("sound") || shortpath.StartsWith("particles") || shortpath.StartsWith("maps"))
                        {
                            if (SyncFile(file, shortpath, fastdlpath, sevenzippath, force))
                            {
                                Console.WriteLine("Resource Synced: " + shortpath);
                            }
                        }
                    }
                   
                }

                Console.WriteLine("");
                Console.WriteLine("Scanning garrysmod/maps directory...");

                foreach (string file in gmodMapFiles)
                {
                    var basepathlength = gmodpath.Length + 1;
                    var shortpath = file.Substring(basepathlength, file.Length - basepathlength);
                    if (shortpath.EndsWith(".bsp") || shortpath.EndsWith(".ain"))
                    {
                        if (SyncFile(file, shortpath, fastdlpath, sevenzippath, force))
                        {
                            Console.WriteLine("Map Synced: " + shortpath);
                        }
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("Finished.");
            }
            else
            {
                Console.WriteLine("Wrong amount of arguments. Required: gmodpath fastdlpath 7zpath");
            }
        }
    }
}