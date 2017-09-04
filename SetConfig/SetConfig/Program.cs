using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetConfig
{
    class Program
    {
        public static String servidor;
        public static String machine;

        static void Main(string[] args)
        {
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
    }
}
