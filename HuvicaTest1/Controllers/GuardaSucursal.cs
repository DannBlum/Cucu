using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;


namespace CucuruchoCafe.Controllers
{
    internal class GuardaSucursal
    {
        public string ConnectionString = "";
        public string estacion = "";
        public SQLCloud.SQLServerTools sql = new SQLCloud.SQLServerTools();

        public SQLCloud.SQLCloud SQLCloudNew = new SQLCloud.SQLCloud();
        List<Models.SucPolanco> ped;
        string sucursal;
        string usuario;
        bool todoBien;
        int miPedido;
        int tamCarrito;
        string observaciones;

        public GuardaSucursal(List<Models.SucPolanco> pedido,string suc,string usu,string coment, string cadena)
        {
            ConnectionString = cadena;
            sql.SetConnection(ConnectionString);
            sucursal = suc;
            usuario = usu;
            observaciones = coment;
            ped = new List<Models.SucPolanco>(pedido);
           // descargaPedido(pedido,suc,usu,observaciones);
        }

        public string obtenerTodo()
        {
            string s = "";
            //foreach (Models.SucPolanco suc in ped)
            //{
            //    s += " Fila:" + suc.fila + ", sku:" + suc.sku + ", descrip:" + suc.descrip + ", inv:" + suc.Inv + ", max:" + suc.Max + ", C:" + suc.C4 + ", happy:" + suc.Happy + ", <br>";
            //}
            List<string> skus = new List<string>();

            for (int i=0; i<ped.Count;i++)
            {
                if ( Convert.ToInt32(ped[i].C4) <= 0)
                    ped.RemoveAt(i);             
            }

            for (int i = 0; i < ped.Count; i++)
            {
                skus.Add(""+ped[i].C4);
            }

            return string.Join(",",skus);
        }

        public void agregaCarrito()
        {
            List<Models.SucPolanco> dtPedido = new List<Models.SucPolanco>(ped);
            string suc = sucursal;
            string usu = usuario;
            string observ = observaciones;

            sql.Reset();
            sql.SQLExec("BEGIN TRANSACTION transaction1");

            todoBien = false;

            for(int i = 0; i < dtPedido.Count; i++)
            {
                if (dtPedido[i].C4 == null || dtPedido[i].C4.Trim().Equals("0") || dtPedido[i].C4.Trim().Contains("-"))
                {
                    continue;
                }

                sql.Reset();
                DataTable carrito = sql.SQLTable("SELECT * FROM carritoPedidos WHERE ltrim(rtrim(usuario)) = '" + usu.Trim() + "' and  ltrim(rtrim(sucursal)) = '" + suc.Trim() + "' and ltrim(rtrim(articulo))='" + dtPedido[i].sku.Trim() +"'", this.ConnectionString);
                sql.Reset();

                if (carrito.Rows.Count > 0)
                {                    
                    sql.CreateUpdate("carritoPedidos", " ltrim(rtrim(usuario)) = '" + usu.Trim() + "' and  ltrim(rtrim(sucursal)) = '" + suc.Trim() + "' and ltrim(rtrim(articulo))='" + dtPedido[i].sku.Trim() + "'");
                }
                else
                {
                    sql.CreateInsert("carritoPedidos");
                }
                sql.AddField("sucursal", suc.Trim());
                sql.AddField("articulo", dtPedido[i].sku.Trim());
                sql.AddField("cantidad", dtPedido[i].C4.Trim());
                sql.AddField("inventario", dtPedido[i].Inv==null?"0":dtPedido[i].Inv.Trim());
                sql.AddField("usuario", usu.Trim());

                todoBien = sql.Exec() == "Ok." ? true : false;
                if (!todoBien)
                {
                    break;
                }
            }

            if (todoBien)
            {
                sql.Reset();
                sql.SQLExec("COMMIT TRANSACTION transaction1");                                               
            }
            else
            {
                sql.Reset();
                sql.SQLExec("ROLLBACK TRANSACTION transaction1");
            }
            DataTable carritoContador = sql.SQLTable("SELECT * FROM carritoPedidos WHERE ltrim(rtrim(sucursal)) = '" + suc.Trim() + "' and ltrim(rtrim(usuario)) = '" + usu.Trim() + "'", this.ConnectionString);
            tamCarrito = carritoContador == null ? 0 : carritoContador.Rows.Count;
        }

        public int cuantoTieneCarrito()
        {
            return tamCarrito;
        }

        public void descargaPedido()
        {
            List<Models.SucPolanco> dtPedido = new List<Models.SucPolanco>(ped);
            string suc = sucursal;
            string usu = usuario;
            string observ = observaciones;

          //  DataTable dtPedido = default(DataTable);
            DataTable no_ped = default(DataTable);

            
            sql.Reset();
            sql.SQLExec("BEGIN TRANSACTION transaction1");

            sql.Reset();
            no_ped = sql.SQLTable("SELECT dato,consec FROM consec WHERE dato = 'no_ped'", this.ConnectionString);
            int nPedido = 0;

            if (no_ped.Rows.Count > 0)
            {
                nPedido = Convert.ToInt32(no_ped.Rows[0]["consec"].ToString().Trim());
                nPedido++;
                sql.Reset();
                sql.SQLExec("UPDATE consec SET consec=" +nPedido+ " where dato='no_ped'");
            }
            else
            {
                nPedido = 1;
                sql.Reset();
                sql.SQLExec("INSERT INTO  consec  ( consec, dato) VALUES (" + nPedido + ", 'no_ped')");
            }
            miPedido = nPedido;
               

            todoBien = false; 

            if (guardaPedido(nPedido, suc, usu, observ))
            {              
                foreach (Models.SucPolanco partidas in dtPedido)
                {
                    todoBien = guardaPedPart(partidas, nPedido, suc, usu);

                    if (!todoBien)
                    {                        
                        break;
                    }
                }                   
            }
            else
            {
                todoBien = false;
            }

            if (todoBien)
            {
                sql.Reset();
                sql.SQLExec("COMMIT TRANSACTION transaction1");
            }
            else
            {
                sql.Reset();
                sql.SQLExec("ROLLBACK TRANSACTION transaction1");
            }            
        }


        public bool guardaPedido(int nPedido, string suc, string usu, string observ)
        {

            List<string> skus = new List<string>();

            sql.Reset();
            



            //Insertar
            sql.CreateInsert("pedidos");
            sql.AddField("pedido", nPedido);
            
            //sql.AddField("PEDID2", drPedido["PEDID"].ToString().Trim());
            sql.AddField("F_EMISION", DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd"));
            sql.AddField("CLIENTE", suc+"");
            sql.AddField("VEND", "SYS");
            sql.AddField("IMPORTE", "0");
            //sql.AddField("DESCUENTO", "0");
            sql.AddField("IMPUESTO", "0");
            sql.AddField("PRECIO","1");
            sql.AddField("COSTO", "0");
            sql.AddField("ALMACEN", "1");
            sql.AddField("ESTADO", "PE");
            sql.AddField("OBSERV", "PEDIDO " + nPedido + ": "+observ);
            sql.AddField("TIPO_CAM", "1");
            sql.AddField("MONEDA", "MXN");
            sql.AddField("DESC1", "0");
            sql.AddField("DESC2", "0");
            sql.AddField("DESC3", "0");
            sql.AddField("DESC4", "0");
            sql.AddField("DESC5", "0");
            sql.AddField("DATOS", usu+" DESDE "+suc);
            sql.AddField("DESGLOSE", "0");
           // sql.AddField("COBRANZA", /*drPedido["COBRANZA"].ToString().Trim()+*/"");
            sql.AddField("USUARIO", usu+"");
            sql.AddField("UsuFecha", DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd"));
            sql.AddField("UsuHora", DateTime.Parse(DateTime.Now.ToString()).ToString("hh:mm:ss"));
            sql.AddField("RELACION", "(Al mismo)");
            sql.AddField("PEDCLI", nPedido+"");
            sql.AddField("AplicarDes", "1");
            //sql.AddField("Autorizado", "1");
            sql.AddField("Entrega", DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd"));
            sql.AddField("Tipo", "PE");
            // sql.AddField("no_cotiz", 0);
            //sql.AddField("no_ped", nPedido+"");
            sql.AddField("donativo", "0");
            // sql.AddField("sucremota", suc+"");
            //sql.AddField("pedidosremota", /*drPedido["pedidosremota"].ToString().Trim()+*/"");

            //  string sResult = sql.Exec();
            string sResult = sql.Execute("SET IDENTITY_INSERT dbo.pedidos ON; " + sql.getSQL() + " SET IDENTITY_INSERT dbo.pedidos OFF;");

            //logErrorFile("Inserta pedido: " + sResult);
            if (sResult.Equals("Ok."))
            {
                //logErrorFile("TRUE: " + sql.getSQL());
                return true;
            }
            else
            {
                //logErrorFile("FALSE: " + sql.getSQL());
                //logErrorFile("Error al salvar registro en tabla pedidos: " + sResult);
                return false;
            }
        }
        public bool guardaPedPart(Models.SucPolanco drPedPar, int nPedido, string suc, string usu)
        {

            if ( drPedPar.C4==null || drPedPar.C4.Trim().Equals("0") || drPedPar.C4.Trim().Contains("-"))
            {

                return true;
            }
            else
            {
                sql.Reset();
                DataTable prods = default(DataTable);
                DataTable pedidDT = default(DataTable);
                String PEDID;
                prods = sql.SQLTable("select prods.articulo, prods.precio1, impuestos.valor from prods inner join impuestos on prods.impuesto=impuestos.impuesto where prods.articulo='" + drPedPar.sku.Trim()+"'", this.ConnectionString);

                double precio = 0.00;
                double impuesto = 0.00;

                if (prods.Rows.Count > 0)
                {
                    precio = Convert.ToDouble(prods.Rows[0]["precio1"].ToString());
                    impuesto = Convert.ToDouble(prods.Rows[0]["valor"].ToString());
                }
                else
                {
                    precio = 1;
                    impuesto = 0;
                }



                pedidDT = sql.SQLTable("SELECT PEDID FROM pedidos WHERE pedido = " + nPedido + " ", this.ConnectionString);
                PEDID = pedidDT.Rows[0]["PEDID"].ToString().Trim();

                sql.Reset();
                //Insertar
                sql.CreateInsert("pedpar");
                sql.AddField("pedido", nPedido);
                sql.AddField("pedid", PEDID);
                //sql.AddField("PEDPARID2", drPedPar["PEDPARID"].ToString().Trim());
                sql.AddField("ARTICULO", drPedPar.sku+"");
                sql.AddField("CANTIDAD",drPedPar.C4+"");
                sql.AddField("SURTIDO", "0");
                sql.AddField("POR_SURT",drPedPar.C4+"");
                sql.AddField("PRECIO",precio);
                sql.AddField("DESCUENTO","0");
                sql.AddField("IMPUESTO", impuesto);
                sql.AddField("OBSERV", drPedPar.descrip+" - "+drPedPar.Inv+" - "+drPedPar.Max+" - "+drPedPar.C4);
              //  sql.AddField("PARTIDA", drPedPar["PARTIDA"].ToString().Trim());
                sql.AddField("Usuario", usu+"");
                sql.AddField("UsuFecha", DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd"));
                sql.AddField("UsuHora", DateTime.Parse(DateTime.Now.ToString()).ToString("hh:mm:ss"));
                sql.AddField("Almacen", "1");
                sql.AddField("Lista", "1");
                sql.AddField("Clave", "");
                sql.AddField("PRCANTIDAD", 0);
                sql.AddField("PRDESCRIP", "");
                sql.AddField("donativo", 0);

                string sResult = sql.Exec();

                return sResult.Equals("Ok.") ? true:false;
                
            }
        }

        public bool estaTodoBien()
        {
            return todoBien;
        }
        public int cualPedidoEs()
        {
            return miPedido;
        }
    }
}