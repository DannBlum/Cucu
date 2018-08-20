using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CucuruchoCafe.Models
{
    public class CadenaConexion
    {

        public static string getStringConnection()
        {
            int count = 0;
            string line;
            string stringConnection = "";
            string estacion = "";

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\connectionString.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (count == 0)
                {
                    string[] arregloconexion = line.Split(';');

                    if (line.IndexOf("Integrated ") > 0)
                        stringConnection = arregloconexion[4] + ";" + arregloconexion[3] + ";" + arregloconexion[1];
                    else
                        stringConnection = arregloconexion[5] + ";" + arregloconexion[4] + ";" + arregloconexion[3] + ";" + arregloconexion[1];

                }
                else
                {
                    estacion = line;
                }

                count++;
            }

            file.Close();

            // Suspend the screen.

            return stringConnection;
        }
    }
}
