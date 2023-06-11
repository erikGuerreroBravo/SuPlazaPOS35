﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class VentaCanceladaArticulo
    {
        public int IdPos { get; set; }

        public Guid IdVentaCancel { get; set; }

        public long NumArticulo { get; set; }

        public string CodBarras { get; set; }

        public decimal Cantidad { get; set; }

        public bool ArticuloOfertado { get; set; }

        public decimal PrecioRegular { get; set; }

        public bool CambioPrecio { get; set; }

        public decimal Iva { get; set; }

        public decimal Ieps { get; set; }

        public decimal PrecioVenta { get; set; }

        public decimal PorcDescuento { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal Utilidad { get; set; }

        public string UserName { get; set; }
    }
}
