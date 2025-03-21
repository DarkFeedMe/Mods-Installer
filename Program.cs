using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VBtoCSharpConverter
{
    internal static class ModInstaller
    {
        private static readonly string InstallerFilePath = Process.GetCurrentProcess().MainModule.FileName;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (Path.GetFileName(InstallerFilePath).Equals("UninstallMods.exe", StringComparison.OrdinalIgnoreCase) ||
                    (args.Length > 0 && args[0] == "uninstall"))
                {
                    UninstallMods();
                }
                else
                {
                    InstallMods();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("A critical error occurred: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static void InstallMods()
        {
            Console.WriteLine("Starting mod installation...");
            string GameFolder, ModFolder = Path.GetDirectoryName(InstallerFilePath);
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the game folder";
                folderDialog.SelectedPath = ModFolder;
                folderDialog.ShowNewFolderButton = false;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    GameFolder = folderDialog.SelectedPath;
                    Console.WriteLine("Game folder selected: " + GameFolder);
                }
                else
                {
                    Console.WriteLine("Game folder not selected. Exiting.");
                    return;
                }
            }

            string BackupFolder = Path.Combine(GameFolder, "ModBackup");
            string FileDataPath = Path.Combine(BackupFolder, "FileData.txt");
            string UninstallExePath = Path.Combine(BackupFolder, "UninstallMods.exe");
            string UninstallShortcutPath = Path.Combine(GameFolder, "Uninstall Mods.lnk");

            try
            {
                if (!Directory.Exists(BackupFolder))
                {
                    Directory.CreateDirectory(BackupFolder);
                    Console.WriteLine("Backup folder created: " + BackupFolder);
                }
                else
                {
                    Console.WriteLine("Backup folder already exists: " + BackupFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating backup folder: " + ex.Message);
                return;
            }

            try
            {
                File.Copy(InstallerFilePath, UninstallExePath, true);
                Console.WriteLine("Uninstall executable copied to backup folder.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error copying uninstall executable: " + ex.Message);
                return;
            }

            try
            {
                CreateShortcut(UninstallShortcutPath, UninstallExePath, "uninstall");
                Console.WriteLine("Uninstall shortcut created: " + UninstallShortcutPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating uninstall shortcut: " + ex.Message);
                return;
            }

            var filesToDelete = new List<string>();
            try
            {
                foreach (var modFile in Directory.GetFiles(ModFolder, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (modFile.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Skipping executable file: " + modFile);
                            continue;
                        }

                        var relativePath = modFile.Substring(ModFolder.Length + 1);
                        var gameFilePath = Path.Combine(GameFolder, relativePath);
                        var backupFilePath = Path.Combine(BackupFolder, relativePath);

                        var gameFileDir = Path.GetDirectoryName(gameFilePath);
                        var uncreated = GetUnCreatedTop(gameFileDir);
                        if (!string.IsNullOrEmpty(uncreated))
                        {
                            filesToDelete.Add(uncreated.Substring(GameFolder.Length + 1));
                            Directory.CreateDirectory(gameFileDir);
                        }

                        if (File.Exists(gameFilePath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath));
                            File.Copy(gameFilePath, backupFilePath, true);
                            Console.WriteLine("Backed up: " + gameFilePath + " to " + backupFilePath);
                        }
                        else
                        {
                            filesToDelete.Add(relativePath);
                            Console.WriteLine("File marked for deletion during uninstall: " + gameFilePath);
                        }

                        File.Copy(modFile, gameFilePath, true);
                        Console.WriteLine("Copied mod file: " + modFile + " to " + gameFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing file: " + modFile + " - " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading mod files: " + ex.Message);
                return;
            }

            try
            {
                File.WriteAllLines(FileDataPath, filesToDelete);
                Console.WriteLine("File data saved: " + FileDataPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving file data: " + ex.Message);
                return;
            }

            Console.WriteLine("Mod installation completed successfully!");
        }

        static string GetUnCreatedTop(string dir)
        {
            string top = null;
            while (!Directory.Exists(dir))
            {
                top = dir;
                dir = Path.GetDirectoryName(dir);
            }
            return top;
        }

        static void UninstallMods()
        {
            Console.WriteLine("Starting mod uninstallation...");
            string BackupFolder = Path.GetDirectoryName(InstallerFilePath);
            string FileDataPath = Path.Combine(BackupFolder, "FileData.txt");
            string UninstallExePath = Path.Combine(BackupFolder, "UninstallMods.exe");
            string GameFolder = Path.GetDirectoryName(BackupFolder);
            string UninstallShortcutPath = Path.Combine(GameFolder, "Uninstall Mods.lnk");

            string[] filesToDelete;
            try
            {
                filesToDelete = File.ReadAllLines(FileDataPath);
                Console.WriteLine("File data loaded: " + FileDataPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file data: " + ex.Message);
                return;
            }

            try
            {
                foreach (var backupFile in Directory.GetFiles(BackupFolder, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (backupFile.Equals(UninstallExePath, StringComparison.OrdinalIgnoreCase) ||
                            backupFile.Equals(FileDataPath, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Skipping uninstall executable: " + backupFile);
                            continue;
                        }

                        var relativePath = backupFile.Substring(BackupFolder.Length + 1);
                        var gameFilePath = Path.Combine(GameFolder, relativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(gameFilePath));
                        File.Copy(backupFile, gameFilePath, true);
                        Console.WriteLine("Restored: " + backupFile + " to " + gameFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error restoring file: " + backupFile + " - " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading backup files: " + ex.Message);
                return;
            }

            try
            {
                foreach (var fileToDelete in filesToDelete)
                {
                    try
                    {
                        var gameFilePath = Path.Combine(GameFolder, fileToDelete);
                        if (File.Exists(gameFilePath))
                        {
                            File.Delete(gameFilePath);
                            Console.WriteLine("File Deleted: " + gameFilePath);
                        }
                        else if (Directory.Exists(gameFilePath))
                        {
                            Directory.Delete(gameFilePath, true);
                            Console.WriteLine("Directory Deleted: " + gameFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting file: " + fileToDelete + " - " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing files to delete: " + ex.Message);
                return;
            }

            try
            {
                if (File.Exists(UninstallShortcutPath))
                {
                    File.Delete(UninstallShortcutPath);
                    Console.WriteLine("Uninstall shortcut deleted: " + UninstallShortcutPath);
                }
                else
                {
                    Console.WriteLine("Uninstall shortcut not found: " + UninstallShortcutPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting uninstall shortcut: " + ex.Message);
            }

            Console.WriteLine("Mod uninstallation completed successfully!");
            try
            {
                DeleteFolderAfterDelay(BackupFolder, 5);
                Console.WriteLine("Backup folder will be deleted after 5 seconds: " + BackupFolder);
                Console.WriteLine("Closing...");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting backup folder: " + ex.Message);
            }
        }

        static void DeleteFolderAfterDelay(string folderPath, int delayInSeconds)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C timeout /t {delayInSeconds} /nobreak && rmdir /s /q \"{folderPath}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }

        static void CreateShortcut(string shortcutPath, string targetPath, string arguments)
        {
            var shell = new IWshRuntimeLibrary.WshShell();
            var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = arguments;
            shortcut.Save();
        }
    }
}