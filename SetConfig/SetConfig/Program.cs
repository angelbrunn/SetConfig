using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SetConfig
{
    class Program
    {
        public static String servidor;
        public static String machine;

        static void Main(string[] args)
        {
            createDirectory();
            int op;
            do
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("[ 0 ] Servidor/Server");
                Console.WriteLine("[ 1 ] Maquina/Machine");
                Console.WriteLine("[ 2 ] Guardar/Save");
                Console.WriteLine("[ 3 ] Salir/Exit");
                Console.WriteLine("-------------------------------------");
                Console.Write("Seleccione opcion/Select Option: ");
                op = Int32.Parse(Console.ReadLine());
                switch (op)
                {
                    case 0:
                        CreateFilesConfigServer();
                        break;
                    case 1:
                        CreateFilesConfigMachine();
                        break;
                    case 2:
                        saveFiles(servidor, machine);
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
            while (op != 3);
        }

    
        private static void CreateFilesConfigServer() {
            Console.WriteLine("Servidor/Server :");
            servidor = Console.ReadLine();
        }

        private static void CreateFilesConfigMachine(){
            Console.WriteLine("Maquina/Machine :");
            machine = Console.ReadLine();
        }

        private static void saveFiles(String Servidor,String machine)
        {
            //SAVE DB FILE
            // Compose a string that consists of three lines.
            string sqlcommand = "SQLCMD -S "+ machine + "\\" + servidor + " -E -i script_generacion_Atlantida_db.sql";

            // Write the string to a file.
            System.IO.StreamWriter dbFile = new System.IO.StreamWriter("c:\\Atlantida\\InstallerDB.bat");
            dbFile.WriteLine(sqlcommand);

            dbFile.Close();

            //SAVE APP.CONFIG FILE
            // Compose a string that consists of three lines.
            String providerName = "providerName =\"System.Data.SqlClient\" ";
            String name = "name = \"AtlantidaDev\"/>";

            string appConfig1 = "<connectionStrings>";
            string appConfig2 = "<add connectionString=Data Source = " + machine + "\\"+ Servidor + "; Initial Catalog = AtlantidaDev; Integrated Security = True " + providerName + " " + name;
            string appConfig3 = "</connectionStrings>";

            // Write the string to a file.
            System.IO.StreamWriter appFile = new System.IO.StreamWriter("c:\\Atlantida\\data\\App.config");
            appFile.WriteLine(appConfig1);
            appFile.WriteLine(appConfig2);
            appFile.WriteLine(appConfig3);

            appFile.Close();
        }

        private void moveFilesConfig()
        {
        }

        static void ExecuteCommand()
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }

        static void createDirectory(){
            // Specify the directory you want to manipulate.
            string path1 = @"c:\Atlantida";
            string path2 = @"c:\Atlantida\data";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path1) || Directory.Exists(path2))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di1 = Directory.CreateDirectory(path1);
                DirectoryInfo di2 = Directory.CreateDirectory(path2);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path1));
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path2));

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }
        }
    }
}
