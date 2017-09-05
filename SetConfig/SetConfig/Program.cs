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
            //CREA LA ESTRUCTURA DEL DIRECTORIO
            createDirectory();
            //COPIA EL SCRIPT DE INICIALIZACION DE DB
            getScriptExecuter();
            int op;
            do
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("---------|Bach SIS-Atlantida|--------");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("[ 0 ] Servidor/Server");
                Console.WriteLine("[ 1 ] Maquina/Machine");
                Console.WriteLine("[ 2 ] Guardar/Save");
                Console.WriteLine("[ 3 ] Iniciar/Initialize");
                Console.WriteLine("[ 4 ] Salir/Exit");
                Console.WriteLine("-------------------------------------");
                Console.Write("Seleccione opcion/Select Option: ");
                op = Int32.Parse(Console.ReadLine());
                switch (op)
                {
                    case 0:
                       setConfigServer();
                        break;
                    case 1:
                        setConfigMachine();
                        break;
                    case 2:
                        if (!string.IsNullOrEmpty(servidor) && !string.IsNullOrEmpty(servidor))
                        {
                            saveFiles(servidor, machine);
                        }
                        else
                        {
                            Console.WriteLine("Falta configurar nombre de servidorSQL y nombre de Maquina.");
                        };
                        break;
                    case 3:
                        if (!string.IsNullOrEmpty(servidor) && !string.IsNullOrEmpty(servidor))
                        {
                            ExecuteScriptDB();
                            moveAppFileConfig();
                        }
                        else {
                            Console.WriteLine("Falta configurar nombre de servidorSQL y nombre de Maquina.");
                        };
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
            while (op != 4);
        }

    
        private static void setConfigServer() {
            Console.WriteLine("Servidor/Server :");
            servidor = Console.ReadLine();
        }

        private static void setConfigMachine(){
            Console.WriteLine("Maquina/Machine :");
            machine = Console.ReadLine();
        }

        private static void saveFiles(String Servidor,String machine)
        {
            //SAVE DB FILE
            // Compose a string that consists of three lines.
            string sqlcommand = "SQLCMD -S "+ machine + "\\" + servidor + " -E -i script_generacion_Atlantida_db.sql";

            // Write the string to a file.
            System.IO.StreamWriter dbFile = new System.IO.StreamWriter("c:\\SIS_AtlantidaConfig\\InstallerDB.bat");
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
            System.IO.StreamWriter appFile = new System.IO.StreamWriter("c:\\SIS_AtlantidaConfig\\data\\App.config");
            appFile.WriteLine(appConfig1);
            appFile.WriteLine(appConfig2);
            appFile.WriteLine(appConfig3);

            appFile.Close();
        }

        static void moveAppFileConfig()
        {
        }

        static void ExecuteScriptDB()
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
            string path1 = @"c:\SIS_AtlantidaConfig";
            string path2 = @"c:\SIS_AtlantidaConfig\data";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path1) || Directory.Exists(path2))
                {
                    Console.WriteLine("La direccion ya existe o esta creada.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di1 = Directory.CreateDirectory(path1);
                DirectoryInfo di2 = Directory.CreateDirectory(path2);
                Console.WriteLine("El directorio se creó correctamente en {0}.", Directory.GetCreationTime(path1));
                Console.WriteLine("El directorio se creó correctamente en {0}.", Directory.GetCreationTime(path2));

            }
            catch (Exception e)
            {
                Console.WriteLine("El proceso fallo: {0}", e.ToString());
            }
            finally { }
        }

        static void getScriptExecuter()
        {
            string fileName = "script_generacion_Atlantida_db.sql";
            string sourcePath = @"C:\lugardeinstalacion\a\b";
            string targetPath = @"C:\SIS_AtlantidaConfig";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("La direccion mensionada no existe!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("presione cualquier tecla para salir.");
            Console.ReadKey();
        }
    }
}
