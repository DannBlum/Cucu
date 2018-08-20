using CucuruchoCafe.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CucuruchoCafe.Controllers
{
    public class AdministracionController : Controller
    {
        private string connectionString = "";
        public SQLCloud.SQLServerTools sql = new SQLCloud.SQLServerTools();

        // GET: Administracion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Maximos(Models.AdministracionModels v)
        {
            return View(llenaTablaMaximos(v));
        }


        [HttpPost]
        public ActionResult GuardaMaximos(Models.AdministracionModels v)
        {
            connectionString = CadenaConexion.getStringConnection();
            sql.SetConnection(connectionString);
            string sResult="";
            v.estaTodobien = false;

            string maximoXarticulo = Request.Params["maximoXarticulo"];
            string maximoXSucursal = Request.Params["maximoXSucursal"];
            string maximoTotal = Request.Params["maximoTotal"];

            string navegar = Request.Params["navegar"];
            string buscar = Request.Params["buscar"];


            if (navegar != null || buscar != null)
            { 
                return RedirectToAction("Maximos", "Administracion", v);
            }
            
            //string str = "";//( maximoXarticulo.Trim().Length <= 0 || maximoXarticulo == null ) ? ( ( maximoXSucursal.Trim().Length <= 0 || maximoXSucursal == null ) ? (  maximoTotal.Trim().Length <= 0 || (maximoTotal == null  ) ? "Error" : maximoTotal.Trim() ): maximoXSucursal.Trim() ) : maximoXarticulo.Trim();                             
            string str = ( maximoXarticulo == null || maximoXarticulo.Trim().Length <= 0 ) ? ( (maximoXSucursal == null || maximoXSucursal.Trim().Length <= 0 )? ( (maximoTotal == null || maximoTotal.Trim().Length <= 0 ) ? "Error" : maximoTotal.Trim() ): maximoXSucursal.Trim() ) : maximoXarticulo.Trim();

            GuardarMaximos gm = new GuardarMaximos(v.maximos,connectionString);

            if ( maximoXarticulo == null ||maximoXarticulo.Trim().Length <= 0 )
            {
                if (maximoXSucursal == null || maximoXSucursal.Trim().Length <= 0 )
                {
                    if(maximoTotal == null || maximoTotal.Trim().Length <= 0  )
                    {
                        str = "Error";
                    }
                    else
                    {
                        sResult = gm.guardarTodo();

                    }
                }
                else
                {
                    sResult = gm.guardarSucursal(maximoXSucursal.Trim());
                }
            }
            else
            {
                sResult = gm.guardarArticulo(maximoXarticulo.Trim());
            }


            connectionString = CadenaConexion.getStringConnection();
            sql.SetConnection(connectionString);


            //string s = "" + v.sucursal + "<br>" + v.usuario + "<br>";
            //GuardaSucursal gs = new GuardaSucursal(v.SucPolanco, v.sucursal, v.usuario, connectionString);


            //v.estaTodobien = gs.estaTodoBien();
            //v.nPedido = gs.cualPedidoEs();
          //  Response.Write("<h1>[query2]<br>" + sResult + " " + "</h1>");
            //return View(v);
            return RedirectToAction("Maximos", "Administracion", v);            
        }

        

        public AdministracionModels llenaTablaMaximos(Models.AdministracionModels v)
        {
            connectionString = CadenaConexion.getStringConnection();
            sql.SetConnection(connectionString);

            string sucursal = v.sucursal.Trim();
            string skus = v.sku == null || v.sku.Trim().Length <= 0 ? "%%" : v.sku.Trim();
            int contador = v.contador == null || v.contador <= 0 ? 0 : v.contador;
            try
            {

                string s1 = "";
                string s2 = "";


                v.maximos = new List<Maximos>();

                s1 = " select ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s1 += " isnull(pr.MaxC1, 0) as 'C1', ";
                s1 += " isnull(pr.MaxC2, 0) as 'C2', ";
                s1 += " isnull(pr.MaxC3, 0) as 'C3', ";
                s1 += " isnull(pr.MaxC4, 0) as 'C4', ";
                s1 += " isnull(pr.MaxCF, 0) as 'CF' ";
                s1 += " from prods pr ";
                s1 += " where ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%'  and  ltrim(rtrim(pr.linea))<>'BAKE' ";
                s1 += " order by ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.LINEA)),  ltrim(rtrim(pr.Descrip)) ";
                 s1 += " OFFSET iif(" + (contador - 1) + " < 0, 0, " + (contador - 1) + ")  ROWS FETCH NEXT 10 ROWS ONLY";

                // s1 = "with disp as (select distinct dispositivo,sucursal, count(distinct almacen) as cant from existenciadispositivolocal where ltrim(rtrim(dispositivo)) like '"+sucursal+"' group by dispositivo)select * from disp where cant>0 group by dispositivo order by dispositivo";
                // Response.Write("<h1>[query1]" + s1 + " " + "</h1>");
                DataTable t1 = default(DataTable);

                sql.Reset();
                t1 = sql.SQLTable(s1, connectionString);
                v.maximos.Clear();

                int cont = contador;
                for (int ix = 0; ix < t1.Rows.Count; ix++)
                {
                    Maximos articuloConcentradora = new Models.Maximos();
                    //articuloConcentradora.fila = Convert.ToInt32(t1.Rows[ix]["linea"].ToString().Trim());
                    articuloConcentradora.sku = t1.Rows[ix]["sku"].ToString().Trim();
                    articuloConcentradora.descrip = t1.Rows[ix]["descrip"].ToString().Trim();
                    articuloConcentradora.MaxC1 = Convert.ToInt32(t1.Rows[ix]["C1"].ToString().Trim());
                    articuloConcentradora.MaxC2 = Convert.ToInt32(t1.Rows[ix]["C2"].ToString().Trim());
                    articuloConcentradora.MaxC3 = Convert.ToInt32(t1.Rows[ix]["C3"].ToString().Trim());
                    articuloConcentradora.MaxC4 = Convert.ToInt32(t1.Rows[ix]["C4"].ToString().Trim());
                    articuloConcentradora.MaxCF = Convert.ToInt32(t1.Rows[ix]["CF"].ToString().Trim());

                    cont++;
                    articuloConcentradora.fila = cont;
                    v.maximos.Add(articuloConcentradora);
                }

                s2 = " select count(*) as cuantos ";
                s2 += " from prods pr ";
                s2 += " where ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%'  and  ltrim(rtrim(pr.linea))<>'BAKE' ";
                //s2 += " order by ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.LINEA)),  ltrim(rtrim(pr.Descrip)) ";
                //s2 += " OFFSET iif(" + (contador - 1) + " < 0, 0, " + (contador - 1) + ")  ROWS FETCH NEXT 10 ROWS ONLY";

                DataTable t2 = default(DataTable);

                sql.Reset();
                t2 = sql.SQLTable(s2, connectionString);

                if(t2.Rows.Count>0)
                v.cuantos= Convert.ToInt32(t2.Rows[0]["cuantos"].ToString().Trim());

                return (v);
            }
            catch
            {
                return (v);
            }
        }
    }
}