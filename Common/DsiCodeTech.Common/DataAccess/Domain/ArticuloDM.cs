using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class ArticuloDM
    {
        public string CodBarras { get; set; }

        public string CodAsociado { get; set; }

        public string CodInterno { get; set; }

        public string Descripcion { get; set; }

        public string DescripcionCorta { get; set; }

        public decimal CantidadUm { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal Utilidad { get; set; }

        public decimal PrecioVenta { get; set; }

        public string TipoArticulo { get; set; }

        public decimal Stock { get; set; }

        public decimal StockMin { get; set; }

        public decimal StockMax { get; set; }

        public bool IsDisponible { get; set; }

        public bool IsKit { get; set; }

        public bool IsVisible { get; set; }

        public short Puntos { get; set; }

        public string CveProducto { get; set; }

        public DateTime? KitFechaIni { get; set; }

        public DateTime? KitFechaFin { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime LastUpdateInventory { get; set; }

        /* Referencias a otras tablas */

        public long IdClasificacion { get; set; }

        public Guid IdUnidad { get; set; }

        public Guid IdProveedor { get; set; }

        public string IdObjetoImpuesto { get; set; }

        public CfdiObjetoImpuestoDM ObjetoImpuesto { get; set; }

        public CategoriaDM Categoria { get; set; }

        public UnidadMedidaDM Unidad { get; set; }

        public ProveedorDM Proveedor { get; set; }

        public List<ArticuloDM> Asociados { get; set; }

        public List<ArticuloDM> Anexos { get; set; }

        public ImpuestoDM Impuesto { get; set; }
    }
}
