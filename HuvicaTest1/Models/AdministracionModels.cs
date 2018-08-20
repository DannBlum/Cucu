using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CucuruchoCafe.Models
{
    public class AdministracionModels 
    {
        public List<Maximos> maximos { get; set; }    
        
        public string sku { get; set; }

        public bool estaTodobien { get; set; }

        public string sucursal { get; set; } = "";
        public string usuario { get; set; }
        public int contador { get; set; } = 0;

        public int cuantos { get; set; }

    }


    public class Maximos
    {
        public int fila { get; set; }
        public string sku { get; set; }
        public string descrip { get; set; }
        public int MaxC1 { get; set; }
        public int MaxC2 { get; set; }
        public int MaxC3 { get; set; }
        public int MaxC4 { get; set; }
        public int MaxCF { get; set; }
    }
}