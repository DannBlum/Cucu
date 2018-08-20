using CucuruchoCafe.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Mail;
using System.IO;

namespace CucuruchoCafe.Controllers
{
    public class SucursalesController : Controller
    {
        private string connectionString = "";
        public SQLCloud.SQLServerTools sql = new SQLCloud.SQLServerTools();

        public ActionResult SucPolanco(Models.SucursalesModels v)
        {
            return View(llenaTablaSucursales(v));
        }

        public ActionResult SucFuego(Models.SucursalesModels v)
        {
            return View(llenaTablaSucursales(v));
        }

        public ActionResult SucRoma(Models.SucursalesModels v)
        {
            return View(llenaTablaSucursales(v));
        }

        public ActionResult SucCondesa(Models.SucursalesModels v)
        {

            return View(llenaTablaSucursales(v));
        }

        public ActionResult SucCuauhtemoc(Models.SucursalesModels v)
        {
            string carrito = Request.Params["carrito"];
            if (carrito != null)
            {
                return View(llenaTablaCarrito(v));
            }
            else
            {
                return View(llenaTablaSucursales(v));
            }
        }

        public ActionResult Concentradora(Models.SucursalesModels v)
        {
            return View(llenaTablaConcentradora(v));
        }

        public Models.SucursalesModels llenaTablaCarrito(Models.SucursalesModels v)
        {
            connectionString = CadenaConexion.getStringConnection();
            sql.SetConnection(connectionString);
            v.contador = 0;
            string sucursal = v.sucursal.Trim();
            string skus = v.sku == null || v.sku.Trim().Length <= 0 ? "%%" : v.sku.Trim();
            int contador = v.contador == null || v.contador <= 0 ? 0 : v.contador;
            try
            {

                string s1 = "";
                string s2 = "";

                v.SucPolanco = new List<SucPolanco>();


                s1 = " select distinct  ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s1 += " isnull(cp.inventario,0)  as 'Inv', ";
                if (sucursal.Trim().ToUpper().Equals("CUAUHTEMOC"))
                {
                    s1 += " MaxC1 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CONDESA"))
                {
                    s1 += " MaxC2 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("ROMA"))
                {
                    s1 += " MaxC3 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("POLANCO"))
                {
                    s1 += " MaxC4 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CASADEFUEGO"))
                {
                    s1 += " MaxCF as 'MAX', ";
                }
                s1 += " isnull(cp.cantidad,0) as 'C4', ";
                s1 += " '' as 'happy' ";
                s1 += " from prods pr inner join carritoPedidos cp on ltrim(rtrim(pr.articulo))=ltrim(rtrim(cp.articulo)) and ltrim(rtrim(cp.sucursal))= '" + sucursal.Trim() + "'";
                s1 += " ";
                s1 += " where  ltrim(rtrim(pr.linea))<>'BAKE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%' ";
                s1 += " order by ltrim(rtrim(pr.LINEA)), ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.Descrip)) ";
                //s1 += " OFFSET iif(" + (contador - 1) + " < 0, 0, " + (contador - 1) + ")  ROWS FETCH NEXT 10 ROWS ONLY";

                // s1 = "with disp as (select distinct dispositivo,sucursal, count(distinct almacen) as cant from existenciadispositivolocal where ltrim(rtrim(dispositivo)) like '"+sucursal+"' group by dispositivo)select * from disp where cant>0 group by dispositivo order by dispositivo";
                // Response.Write("<h1>[query1]" + sucursal + " " + "</h1>");
                DataTable t1 = default(DataTable);
                sql.Reset();
                t1 = sql.SQLTable(s1, connectionString);
                v.SucPolanco.Clear();


                for (int ix = 0; ix < t1.Rows.Count; ix++)
                {
                    SucPolanco SucurPolanco = new SucPolanco();
                    SucurPolanco.linea = t1.Rows[ix]["linea"].ToString().Trim();
                    SucurPolanco.sku = t1.Rows[ix]["sku"].ToString().Trim();
                    SucurPolanco.descrip = t1.Rows[ix]["descrip"].ToString().Trim();
                    SucurPolanco.Inv = (t1.Rows[ix]["Inv"].ToString().Trim());
                    SucurPolanco.Max = (t1.Rows[ix]["MAX"].ToString().Trim());
                    SucurPolanco.C4 = (t1.Rows[ix]["C4"].ToString().Trim());
                    SucurPolanco.Happy = (t1.Rows[ix]["happy"].ToString().Trim());

                    contador++;
                    SucurPolanco.fila = "" + contador;
                    v.SucPolanco.Add(SucurPolanco);
                }
                s2 = " with total as ( ";
                s2 += " select distinct  ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s2 += " pr.EXISTENCIA as 'Inv', ";
                if (sucursal.Trim().ToUpper().Equals("CUAUHTEMOC"))
                {
                    s2 += " MaxC1 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CONDESA"))
                {
                    s2 += " MaxC2 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("ROMA"))
                {
                    s2 += " MaxC3 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("POLANCO"))
                {
                    s2 += " MaxC4 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CASADEFUEGO"))
                {
                    s2 += " MaxCF as 'MAX', ";
                }
                s2 += " '' as 'C4', ";
                s2 += " '' as 'happy' ";
                s2 += " from prods pr inner join carritoPedidos cp on ltrim(rtrim(pr.articulo))=ltrim(rtrim(cp.articulo)) and ltrim(rtrim(cp.sucursal))= '" + sucursal.Trim() + "'";
                s2 += "  ";
                s2 += " where  ltrim(rtrim(pr.linea))<>'BAKE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%'  ";
                s2 += " ) ";
                s2 += " select count(sku) as totalArticulos, ";
                s2 += " sum(Inv) as Talm11, sum(MAX) as Talm12 from total";

                // Response.Write("<h1>[query2]" + s2 + " " + "</h1>");
                DataTable t2 = default(DataTable);

                t2 = sql.SQLTable(s2, connectionString);

                if (t2.Rows.Count > 0)
                {
                    v.totalTickets = t2.Rows[0]["totalArticulos"].ToString().Trim();
                    v.TotInv = t2.Rows[0]["Talm11"].ToString().Trim();
                    v.TotMax = t2.Rows[0]["Talm12"].ToString().Trim();
                }
                sql.Reset();
                DataTable carritoContador = sql.SQLTable("SELECT * FROM carritoPedidos WHERE ltrim(rtrim(sucursal)) = '" + sucursal.Trim() + "' and ltrim(rtrim(usuario)) = '" + v.usuario.Trim() + "'", connectionString);
                v.tamCarrito = carritoContador == null ? 0 : carritoContador.Rows.Count;
                v.enCarrito = true;
                return (v);
            }
            catch
            {
                return (v);
            }
        }

        public Models.SucursalesModels llenaTablaSucursales(Models.SucursalesModels v)
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

                v.SucPolanco = new List<SucPolanco>();
                

                s1 = " select distinct  ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s1 += " isnull(cp.inventario,0)  as 'Inv', ";
                if (sucursal.Trim().ToUpper().Equals("CUAUHTEMOC"))
                {
                    s1 += " MaxC1 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CONDESA"))
                {
                    s1 += " MaxC2 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("ROMA"))
                {
                    s1 += " MaxC3 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("POLANCO"))
                {
                    s1 += " MaxC4 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CASADEFUEGO"))
                {
                    s1 += " MaxCF as 'MAX', ";
                }
                s1 += " isnull(cp.cantidad,0) as 'C4', ";
                s1 += " '' as 'happy' ";
                s1 += " from prods pr  left join carritoPedidos cp on ltrim(rtrim(pr.articulo))=ltrim(rtrim(cp.articulo)) and ltrim(rtrim(cp.sucursal))= '" + sucursal.Trim() + "'";
                s1 += " ";
                s1 += " where  ltrim(rtrim(pr.linea))<>'BAKE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%' ";
                s1 += " order by ltrim(rtrim(pr.LINEA)), ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.Descrip)) ";
                s1 += " OFFSET iif(" + (contador - 1) + " < 0, 0, " + (contador - 1) + ")  ROWS FETCH NEXT 10 ROWS ONLY";

                // s1 = "with disp as (select distinct dispositivo,sucursal, count(distinct almacen) as cant from existenciadispositivolocal where ltrim(rtrim(dispositivo)) like '"+sucursal+"' group by dispositivo)select * from disp where cant>0 group by dispositivo order by dispositivo";
                // Response.Write("<h1>[query1]" + s1 + " " + "</h1>");
                DataTable t1 = default(DataTable);
                sql.Reset();
                t1 = sql.SQLTable(s1, connectionString);
                v.SucPolanco.Clear();
             

                for (int ix = 0; ix < t1.Rows.Count; ix++)
                {
                    SucPolanco SucurPolanco = new SucPolanco();
                    SucurPolanco.linea = t1.Rows[ix]["linea"].ToString().Trim();
                    SucurPolanco.sku = t1.Rows[ix]["sku"].ToString().Trim();
                    SucurPolanco.descrip = t1.Rows[ix]["descrip"].ToString().Trim();
                    SucurPolanco.Inv =(t1.Rows[ix]["Inv"].ToString().Trim());
                    SucurPolanco.Max =(t1.Rows[ix]["MAX"].ToString().Trim());
                    SucurPolanco.C4 =(t1.Rows[ix]["C4"].ToString().Trim());
                    SucurPolanco.Happy = (t1.Rows[ix]["happy"].ToString().Trim());
                    
                    contador++;
                    SucurPolanco.fila = ""+contador;
                    v.SucPolanco.Add(SucurPolanco);
                }
                s2 = " with total as ( ";
                s2 += " select distinct  ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s2 += " pr.EXISTENCIA as 'Inv', ";
                if (sucursal.Trim().ToUpper().Equals("CUAUHTEMOC"))
                {
                    s2 += " MaxC1 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CONDESA"))
                {
                    s2 += " MaxC2 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("ROMA"))
                {
                    s2 += " MaxC3 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("POLANCO"))
                {
                    s2 += " MaxC4 as 'MAX', ";
                }
                else if (sucursal.Trim().ToUpper().Equals("CASADEFUEGO"))
                {
                    s2 += " MaxCF as 'MAX', ";
                }
                s2 += " isnull(cp.cantidad,0) as 'C4', ";
                s2 += " '' as 'happy' ";
                s2 += " from prods pr  left join carritoPedidos cp on ltrim(rtrim(pr.articulo))=ltrim(rtrim(cp.articulo)) and ltrim(rtrim(cp.sucursal))= '" + sucursal.Trim() + "'";
                s2 += "  ";
                s2 += " where  ltrim(rtrim(pr.linea))<>'BAKE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%'  ";
                s2 += " ) ";
                s2 += " select count(sku) as totalArticulos, ";
                s2 += " sum(Inv) as Talm11, sum(MAX) as Talm12 from total";

             //   Response.Write("<h1>[query2]" + s2 + " " + "</h1>");
                DataTable t2 = default(DataTable);

                t2 = sql.SQLTable(s2, connectionString);

                if (t2.Rows.Count > 0)
                {
                    v.totalTickets = t2.Rows[0]["totalArticulos"].ToString().Trim();
                    v.TotInv = t2.Rows[0]["Talm11"].ToString().Trim();
                    v.TotMax = t2.Rows[0]["Talm12"].ToString().Trim();
                }

                sql.Reset();
                DataTable carritoContador = sql.SQLTable("SELECT * FROM carritoPedidos WHERE ltrim(rtrim(sucursal)) = '" + sucursal.Trim() + "' and ltrim(rtrim(usuario)) = '" + v.usuario.Trim() + "'", connectionString);
                v.tamCarrito = carritoContador == null ? 0 : carritoContador.Rows.Count;
                v.enCarrito = false;
                return (v);
            }
            catch
            {
                return (v);
            }
        }

        public Models.SucursalesModels llenaTablaConcentradora(Models.SucursalesModels v)
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


                v.articulosConcentrador = new List<Concentradora>();

                s1 = " select ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ltrim(rtrim(pr.Descrip)) as Descrip, ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CUAUHTEMOC' then isnull(cantidad, 0) end), 0) as 'C1', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CONDESA' then isnull(cantidad, 0) end), 0) as 'C2', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'ROMA CRUZ' then isnull(cantidad, 0) end), 0) as 'C3', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'POLANCO' then isnull(cantidad, 0) end), 0) as 'C4', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CASADEFUEGO' then isnull(cantidad, 0) end), 0) as 'CF' ";
                s1 += " from prods pr full join (pedidos p inner join pedpar pp on p.pedido = pp.pedido) on pp.articulo = pr.articulo ";
                s1 += " where P.ESTADO='PE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%' and  ltrim(rtrim(pr.linea))<>'BAKE' ";
                s1 += " group by ltrim(rtrim(pr.LINEA)),  ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.Descrip)) ";
                s1 += " order by ltrim(rtrim(pr.LINEA)), ltrim(rtrim(pr.ARTICULO)), ltrim(rtrim(pr.Descrip)) ";
               // s1 += " OFFSET iif(" + (contador - 1) + " < 0, 0, " + (contador - 1) + ")  ROWS FETCH NEXT 10 ROWS ONLY";

                // s1 = "with disp as (select distinct dispositivo,sucursal, count(distinct almacen) as cant from existenciadispositivolocal where ltrim(rtrim(dispositivo)) like '"+sucursal+"' group by dispositivo)select * from disp where cant>0 group by dispositivo order by dispositivo";
                //Response.Write("<h1>[query1]" + s1 + " " + "</h1>");
                DataTable t1 = default(DataTable);
                sql.Reset();
                t1 = sql.SQLTable(s1, connectionString);
                v.articulosConcentrador.Clear();
                int cont = contador;
                for (int ix = 0; ix < t1.Rows.Count; ix++)
                {
                    Concentradora articuloConcentradora = new Concentradora();
                    articuloConcentradora.linea= t1.Rows[ix]["linea"].ToString().Trim();
                    articuloConcentradora.sku = t1.Rows[ix]["sku"].ToString().Trim();
                    articuloConcentradora.descrip = t1.Rows[ix]["descrip"].ToString().Trim();
                    articuloConcentradora.C1 = t1.Rows[ix]["C1"].ToString().Trim();
                    articuloConcentradora.C2 = t1.Rows[ix]["C2"].ToString().Trim();
                    articuloConcentradora.C3 = t1.Rows[ix]["C3"].ToString().Trim();
                    articuloConcentradora.C4 = t1.Rows[ix]["C4"].ToString().Trim();
                    articuloConcentradora.CF = t1.Rows[ix]["CF"].ToString().Trim();
                 
                    contador++;
                    articuloConcentradora.fila = contador;
                    v.articulosConcentrador.Add(articuloConcentradora);
                }

                s2 = " with total as ( ";
                s2 += " select ltrim(rtrim(pr.LINEA)) as Linea, ltrim(rtrim(pr.ARTICULO)) as sku, ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CUAUHTEMOC' then isnull(cantidad, 0) end), 0) as 'C1', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CONDESA' then isnull(cantidad, 0) end), 0) as 'C2', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'ROMA CRUZ' then isnull(cantidad, 0) end), 0) as 'C3', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'POLANCO' then isnull(cantidad, 0) end), 0) as 'C4', ";
                s1 += " isnull(sum(case when ltrim(rtrim(p.cliente)) = 'CASADEFUEGO' then isnull(cantidad, 0) end), 0) as 'CF' ";
                s2 += " from prods pr full join (pedidos p inner join pedpar pp on p.pedido = pp.pedido) on pp.articulo = pr.articulo ";
                s1 += " where  P.ESTADO='PE' and ltrim(rtrim(pr.descrip)) like '%" + skus.Trim() + "%' and  ltrim(rtrim(pr.linea))<>'BAKE' ";
                s2 += " group by ltrim(rtrim(pr.LINEA)),  ltrim(rtrim(pr.ARTICULO)) ";
                s2 += " ) ";
                s2 += " select count(sku) as totalArticulos, ";
                s2 += " sum(C1) as Talm11, sum(C2) as Talm12, sum(C3) as Talm13, sum(C4) as Talm14, sum(CF) as TTotalAzc from total";


                //Response.Write("<h1>[query2]" + s2 + " " + "</h1>");
                DataTable t2 = default(DataTable);

                t2 = sql.SQLTable(s2, connectionString);

                if (t2.Rows.Count > 0)
                {

                    v.totalTickets = t2.Rows[0]["totalArticulos"].ToString().Trim();

                    v.TotC1 = t2.Rows[0]["Talm11"].ToString().Trim();
                    v.TotC2 = t2.Rows[0]["Talm12"].ToString().Trim();
                    v.TotC3 = t2.Rows[0]["Talm13"].ToString().Trim();
                    v.TotC4 = t2.Rows[0]["Talm14"].ToString().Trim();
                    v.TotCF = t2.Rows[0]["TTotalAzc"].ToString().Trim();

                }
                return (v);
            }
            catch
            {
                return (v);
            }
        }

        [HttpPost]
        public ActionResult GuardaSucursal(Models.SucursalesModels v)
        {
            string navegar = Request.Params["navegar"];
            string buscar = Request.Params["buscar"];
            string carrito = Request.Params["carrito"];
            string Guardar = Request.Params["Guardar"];

            if (carrito != null || Guardar != null)
            {
                v.sku = "";
            }

            if (navegar != null || buscar != null || carrito != null)
            {
                v.enCarrito = false;
                return RedirectToAction("SucCuauhtemoc", "Sucursales", v);
            }
            

            if (v.SucPolanco.Count > 0)
            {
                if (Guardar.ToUpper().Trim().Equals("Add".ToUpper().Trim()))
                {
                    
                    connectionString = CadenaConexion.getStringConnection();
                    GuardaSucursal gs = new GuardaSucursal(v.SucPolanco, v.sucursal, v.usuario,"", connectionString);
                    gs.agregaCarrito();
                    v.tamCarrito = gs.cuantoTieneCarrito();
                   // Response.Write(" < script language = javascript > alert('Agregado a carrito');</ script > ");
                    return RedirectToAction("SucCuauhtemoc", "Sucursales", v);
                }
                else
                {
                    v.estaTodobien = false;


                    connectionString = CadenaConexion.getStringConnection();
                    sql.SetConnection(connectionString);
                    string s = "" + v.sucursal + "<br>" + v.usuario + "<br>";
                    GuardaSucursal gs = new GuardaSucursal(v.SucPolanco, v.sucursal, v.usuario, v.comentariosPedido, connectionString);
                    gs.descargaPedido();

                    v.estaTodobien = gs.estaTodoBien();
                    v.nPedido = gs.cualPedidoEs();

                    sql.Reset();
                    if (v.estaTodobien)
                        sql.SQLExec("delete from carritoPedidos where ltrim(rtrim(sucursal))='"+v.sucursal.Trim()+ "' and ltrim(rtrim(usuario)) = '" + v.usuario.Trim() + "'");


                    DataTable carritoContador = sql.SQLTable("SELECT * FROM carritoPedidos WHERE ltrim(rtrim(sucursal)) = '" + v.sucursal.Trim() + "' and ltrim(rtrim(usuario)) = '" + v.usuario.Trim() + "'", connectionString);
                    v.tamCarrito = carritoContador == null ? 0 : carritoContador.Rows.Count;
                    //Response.Write("<h1>[query2]<br>" +s+gs.obtenerTodo()+ " " + "</h1>");

                    return RedirectToAction("GuardaSucursalSuccess", "Sucursales", v);
                }
            }
            else
            {
                return new EmptyResult();
            }

        }
        [HttpGet]
        public ActionResult GuardaSucursalSuccess(Models.SucursalesModels v)
        {
            connectionString = CadenaConexion.getStringConnection();
            sql.SetConnection(connectionString);
            sql.Reset();           
            v.tamCarrito = 0;
            SQLCloud.ReportesCLOUD r = new SQLCloud.ReportesCLOUD();
            string rutaMRT = Server.MapPath("~\\mrt\\Pedidos.mrt");
            string rutaReportes = Server.MapPath("~\\Pedidos");
            r.LoadReport(rutaMRT);
            string sVenta = v.nPedido+"";
            DataTable dtVenta = sql.SQLTable("SELECT * FROM pedidos WHERE pedido = " + sVenta, connectionString);
            r.limpiaDatos();
            r.ReportQuery("encabezado", "SELECT * FROM pedidos WHERE pedido = " + dtVenta.Rows[0]["pedido"].ToString().Trim(), connectionString);
            r.ReportQuery("partidas", "SELECT pedpar.*, pedpar.precio * (1 - (pedpar.descuento / 100)) * pedpar.cantidad * (pedpar.impuesto / 100) as MiImpuesto FROM pedpar inner join prods on pedpar.articulo=prods.articulo where pedpar.pedido = " + dtVenta.Rows[0]["pedido"].ToString().Trim(), connectionString);
            r.ReportQuery("cliente", "SELECT * FROM clients WHERE cliente = '" + dtVenta.Rows[0]["cliente"].ToString().Trim() + "'", connectionString);

            r.SincronizaDatos();
            r.ShowReport();
            string nombreArchivo = rutaReportes + "\\Pedido_" + dtVenta.Rows[0]["pedido"].ToString().Trim() + "_Referencia_" + dtVenta.Rows[0]["no_ped"].ToString().Trim() + "_Cliente_" + dtVenta.Rows[0]["cliente"].ToString().Trim() + "_" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + "_.pdf";

            try
            {
                r.ExportToPDF(nombreArchivo);
                enviaReporte(nombreArchivo, sVenta, v.sucursal, v);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            
            return View(v);

        }

        public void enviaReporte(string pdf,string pedido, string suc, Models.SucursalesModels v)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("desarrollogeneralblum@gmail.com");
            mail.To.Add("desarrollo.blum.danielm@gmail.com");
            //mail.To.Add("desarrollo3@blumitech.com.mx");
            //mail.To.Add("desarrollo2@blumitech.com.mx");
            //mail.To.Add("fmorrill@blumitech.com.mx");
            //mail.To.Add("lzafra@blumitech.com.mx");
            mail.Subject = "Comprobante de pedido "+pedido;
            mail.Body = "Hola, se realizó el pedido #"+pedido+" de la sucursal "+suc;
            mail.Attachments.Add(new Attachment(pdf));
            SmtpServer.Port = 587;                                     //usuario de correo      password de correo 
            SmtpServer.Credentials = new System.Net.NetworkCredential("desarrollogeneralblum", "$BlumiTeam@"); //si no se establece, enviará el correo de 
            SmtpServer.EnableSsl = true;                                                                        // forma anónima
            SmtpServer.Send(mail);
            v.error=("Correo enviado");
        }


        public ActionResult _Concentradora(Models.SucursalesModels reporte)
        {
            //   Response.Write("<h1>[query]" + reporte.sucursales.ToString() + "</h1>");
            //if (reporte.btnExcel.Equals("NO") && reporte.btnPDF.Equals("NO"))
            //{
            //    return PartialView("_Concentradora", llenaTablaConcentradora(reporte));

            //}
            //else if (reporte.btnExcel.Equals("SI"))
            //{
            //    if (reporte.sku == null || reporte.sku == null || reporte.sku.Trim().Equals("") || reporte.sku.Trim().Equals("") || reporte.sucursales.Trim().Equals(""))
            //    {
            //        Response.Write("<h1>Informacion ingresada incorrectamente</h1>");
            //    }
            //    else
            //    {
            //        //-- generarExcel(reporte);
            //        Response.Write("<h1>Excel generado</h1>");
            //        Response.Flush();
            //    }
            //    return PartialView("_Concentradora", llenaTablaConcentradora(reporte));
            //}
            //else if (reporte.btnPDF.Equals("SI"))
            //{
            //    if (reporte.sku == null || reporte.sku == null || reporte.sku.Trim().Equals("") || reporte.sku.Trim().Equals("") || reporte.sucursales.Trim().Equals(""))
            //    {
            //        Response.Write("<h1>Informacion ingresada incorrectamente</h1>");
            //    }
            //    else
            //    {
            //        //  Response.Write("<h1>Generando PDF</h1>");
            //        //--   generarPDF(reporte);
            //        Response.Write("<h1>PDF Generado </h1>");
            //    }

            //    return PartialView("_Concentradora", llenaTablaConcentradora(reporte));
            //}
            return PartialView("_Concentradora", llenaTablaConcentradora(reporte));

        }
    }
}
