using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CucuruchoCafe.Models
{
 public class SucursalesModels
    {
        [BrowsableAttribute(false)]
        public bool IsPostBack { get; }

        public bool estaTodobien { get; set; }
        public int nPedido { get; set; }

        public List<Concentradora> articulosConcentrador { get; set; }
        public List<SucPolanco> SucPolanco { get; set; }
        public List<SucPolanco> carrito { get; set; } 

        public int tamCarrito { get; set; }
        public bool enCarrito { get; set; }

        public string sucursal { get; set; } = "";
        public string usuario { get; set; }
        public string comentariosPedido { get; set; }
        
        public string sku { get; set; }

        //public string btnExcel { get; set; } = "NO";
        //public string btnPDF { get; set; } = "NO";

        public int contador { get; set; } = 0;


        public string totalVentas { get; set; } = "0";

        public string totalTickets { get; set; } = "0";


        public string TotC1 { get; set; }
        public string TotC2 { get; set; }
        public string TotC3 { get; set; }
        public string TotC4 { get; set; }
        public string TotCF { get; set; }

        public string TotMax { get; set; }
        public string TotInv { get; set; }        
        public string TotCCuatro { get; set; }
        public string TotHappy { get; set; }

        public string error { get; set; }
    }

    public class Concentradora
    {
        public int fila { get; set; }
        public string linea { get; set; }
        public string sku { get; set; }
        public string descrip { get; set; }

        public string C1 { get; set; }
        public string C2 { get; set; }
        public string C3 { get; set; }
        public string C4 { get; set; }
        public string CF { get; set; }
        

    }

    public class SucPolanco
    {
        public string fila { get; set; }
        public string linea { get; set; }
        public string sku { get; set; }
        public string descrip { get; set; }

        
        public string Inv { get; set; }

        public string Max { get; set; }
        
        public string C4 { get; set; }
        public string Happy { get; set; }        
    }

}