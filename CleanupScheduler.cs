using Microsoft.Win32.TaskScheduler;
using TaskFolderExt;

static class CleanupScheduler
{
    static void Main(string[] args)
    {
        try
        {
            // Create a new TaskService object
            using (TaskService ts = new TaskService())
            {
                // Create a new task
                TaskDefinition td = ts.NewTask();

                // Set task properties
                td.RegistrationInfo.Description = "Cleanup Temporary Files and Flush DNS";

                // Create a trigger to run at system startup
                td.Triggers.Add(new BootTrigger());

                // Create a trigger to run at user logon
                td.Triggers.Add(new LogonTrigger());

                // Create actions (e.g., execute multiple commands)
                td.Actions.Add(new ExecAction("cmd.exe", "/c del /Q /S %temp%\\*.* 2>nul", null)); // Commands to cleanup temporary files
                td.Actions.Add(new ExecAction("cmd.exe", "/c del /Q /S %userprofile%\\AppData\\Local\\Microsoft\\Windows\\INetCache\\*.* 2>nul", null));
                td.Actions.Add(new ExecAction("cmd.exe", "/c del /Q /S %userprofile%\\AppData\\Local\\Microsoft\\Windows\\Temporary Internet Files\\*.* 2>nul", null));
                td.Actions.Add(new ExecAction("cmd.exe", "/c ipconfig /flushdns", null)); // Command to flush DNS cache

                // Get the root folder
                TaskFolder rootFolder = ts.RootFolder;

                // Folder path where you want to register the task
                const string folderPath = "\\MyTask"; // Change this to your desired folder path

                // Attempt to retrieve the folder
                TaskFolder? myTaskFolder = rootFolder.TryGetFolder(folderPath);

                // If the folder doesn't exist, create it
                if (myTaskFolder == null)
                {
                    myTaskFolder = rootFolder.CreateFolder(folderPath);
                }

                // Register the task in the specified folder
                myTaskFolder.RegisterTaskDefinition("CleanupTask", td);

                Console.WriteLine("Task scheduled successfully in the MyTask folder.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(); // Wait for any key press before exiting
    }
}

// Extension method to try getting a folder
namespace TaskFolderExt
{
    public static class TaskFolderExtensions
    {
        public static TaskFolder? TryGetFolder(this TaskFolder folder, string path)
        {
            try
            {
                return folder.SubFolders[path];
            }
            catch
            {
                return null;
            }
        }
    }
}