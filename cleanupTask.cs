using Microsoft.Win32.TaskScheduler;

class Program
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

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("CleanupTask", td);

                Console.WriteLine("Task scheduled successfully.");
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
