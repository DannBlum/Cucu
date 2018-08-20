using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CucuruchoCafe.Controllers
{
    internal class GuardarMaximos 
    {

        public string ConnectionString = "";
        public string estacion = "";
        public SQLCloud.SQLServerTools sql = new SQLCloud.SQLServerTools();

        public SQLCloud.SQLCloud SQLCloudNew = new SQLCloud.SQLCloud();
        List<Models.Maximos> maximos;

        bool todoBien;


        // GET: GuardarMaximos
        public GuardarMaximos(List<Models.Maximos> max, string cadena)
        {
            ConnectionString = cadena;
            sql.SetConnection(ConnectionString);
            maximos = new List<Models.Maximos>(max);

        }

        public string guardarTodo()
        {
            string sResult = "Ok.";
            
            for (int i = 0; i < maximos.Count && sResult.Equals("Ok."); i++)
            {
                sql.Reset();
                sql.CreateUpdate("prods", " ltrim(rtrim(articulo))='" + maximos[i].sku.Trim() + "'");
                sql.AddField("MaxC1", maximos[i].MaxC1);
                sql.AddField("MaxC2", maximos[i].MaxC2);
                sql.AddField("MaxC3", maximos[i].MaxC3);
                sql.AddField("MaxC4", maximos[i].MaxC4);
                sql.AddField("MaxCF", maximos[i].MaxCF);

                sResult = sql.Exec();
            }
            return sResult;
        }

        public string guardarSucursal(string sucursal)
        {
            string sResult="Ok.";
            for (int i = 0; i < maximos.Count && sResult.Equals("Ok."); i++)
            {
                sql.Reset();
                sql.CreateUpdate("prods", " ltrim(rtrim(articulo))='" + maximos[i].sku.Trim() + "'");
                if(sucursal.Trim().ToUpper().Equals("Cuauhtemoc".ToUpper()))
                    sql.AddField("MaxC1", maximos[i].MaxC1);
                else if (sucursal.Trim().ToUpper().Equals("Condesa".ToUpper()))
                    sql.AddField("MaxC2", maximos[i].MaxC2);
                else if (sucursal.Trim().ToUpper().Equals("Roma".ToUpper()))
                    sql.AddField("MaxC3", maximos[i].MaxC3);
                else if (sucursal.Trim().ToUpper().Equals("Polanco".ToUpper()))
                    sql.AddField("MaxC4", maximos[i].MaxC4);
                else if (sucursal.Trim().ToUpper().Equals("CasaDeFuego".ToUpper()))
                    sql.AddField("MaxCF", maximos[i].MaxCF);

                sResult= sql.Exec();
                
            }
            return sResult;
        }

        public string guardarArticulo(string articulo)
        {            
            for(int i = 0; i < maximos.Count; i++)
            {
                if (maximos[i].sku.Trim().Equals(articulo.Trim()))
                {
                    sql.Reset();                    
                    sql.CreateUpdate("prods", " ltrim(rtrim(articulo))='" + maximos[i].sku.Trim() + "'");
                    sql.AddField("MaxC1", maximos[i].MaxC1);
                    sql.AddField("MaxC2", maximos[i].MaxC2);
                    sql.AddField("MaxC3", maximos[i].MaxC3);
                    sql.AddField("MaxC4", maximos[i].MaxC4);
                    sql.AddField("MaxCF", maximos[i].MaxCF);

                    return sql.Exec();
                }
            }
            return "OknoOK.";
        }

    }
}