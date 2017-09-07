using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace SetConfig
{
    class Program
    {
        public static String servidor;
        public static String machine;
        public static Boolean isSave = false;
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
                Console.WriteLine("[ 1 ] ServidorSQL/ServerSQL");
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
                            isSave = true;
                            Console.WriteLine("Se configuro nombre de servidorSQL y nombre de Maquina. Inicie!");
                        }
                        else
                        {
                            Console.WriteLine("Falta configurar nombre de servidorSQL y nombre de Maquina.");
                        };
                        break;
                    case 3:
                        if (!string.IsNullOrEmpty(servidor) && !string.IsNullOrEmpty(servidor))
                        {
                            if (isSave == true)
                            {
                                ExecuteScriptDB();
                                moveAppFileConfig();
                                deleteDirectory();
                            }
                            else {
                                Console.WriteLine("Falta guardar las configuraciones.");
                            }
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
            string sqlcommand = "SQLCMD -S " + machine + " -E -i script_generacion_Atlantida_db.sql";

            // Write the string to a file.
            System.IO.StreamWriter dbFile = new System.IO.StreamWriter("c:\\SIS_AtlantidaConfig\\InstallerDB.bat");
            dbFile.WriteLine(sqlcommand);

            dbFile.Close();

            //SAVE APP.CONFIG FILE
            // Compose a string that consists of three lines.
            String providerName = "providerName =\"System.Data.SqlClient\" ";
            String name = "name =\"AtlantidaDev\"/>";

            string appConfigh0 = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            string appConfigh1 = "<configuration>";
            string appConfig1 = "<connectionStrings>";
            string appConfig2 = "<add connectionString=\"Data Source=" + machine + ";Initial Catalog=AtlantidaDev;Integrated Security=True \"" + providerName + " " + name;
            string appConfig3 = "</connectionStrings>";
            string appConfig4 = "</configuration>";

            // Write the string to a file.
            System.IO.StreamWriter appFile = new System.IO.StreamWriter("c:\\SIS_AtlantidaConfig\\data\\Atlantida.UI.exe.config");
            appFile.WriteLine(appConfigh0);
            appFile.WriteLine(appConfigh1);
            appFile.WriteLine(appConfig1);
            appFile.WriteLine(appConfig2);
            appFile.WriteLine(appConfig3);
            appFile.WriteLine(appConfig4);

            appFile.Close();
        }

        static void ExecuteScriptDB()
        {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "c:\\SIS_AtlantidaConfig\\InstallerDB.bat";
            process.StartInfo.WorkingDirectory = "C:\\SIS_AtlantidaConfig";
            process.Start();

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

        static void deleteDirectory() {
            Directory.Delete(@"C:\SIS_AtlantidaConfig", true);
        }

        static void getScriptExecuter()
        {
            string fileName = "script_generacion_Atlantida_db.sql";
            string sourcePath = @"C:\Program Files (x86)\SISAtlantida\Atlantida\script";
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
                Console.WriteLine("La direccion mencionada no existe!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("presione cualquier tecla para continuar con la configuracion.");
            Console.ReadKey();
        }

        static void moveAppFileConfig()
        {
            string fileName = "Atlantida.UI.exe.config";
            string sourcePath = @"C:\SIS_AtlantidaConfig\data";
            string targetPath = @"C:\Program Files (x86)\SISAtlantida\Atlantida";

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

            //FileInfo file1 = new FileInfo(@"C:\Program Files (x86)\SISAtlantida\Atlantida\script\Atlantida.UI.exe.config");
            // FileInfo file2 = new FileInfo(@"C:\SIS_AtlantidaConfig\data\script\Atlantida.UI.exe.config");
            // FileSecurity ac1 = file1.GetAccessControl();
            //ac1.SetAccessRuleProtection(true, true);
            //file2.SetAccessControl(ac1);
            string fileA = targetPath + "\\" + fileName;
            FileSecurity securityA = File.GetAccessControl(fileA);

            string fileB = sourcePath + "\\" + fileName;
            File.SetAccessControl(fileB, securityA);

            System.IO.File.Copy(fileB, fileA, true);

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
                Console.WriteLine("La direccion mencionada no existe!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("presione cualquier tecla para continuar con la configuracion.");
            Console.ReadKey();
        }
    }
}
