using DsiCodeTech.Common.DataAccess.Filter.Page;

namespace DsiCodeTech.Common.DataAccess.Filter.Query
{
    public class FacturaQuery
    {
        public string IdComprobante { get; set; }

        public string IdMetodoPago { get; set; }

        public string IdUsoCfdi { get; set; }

        public string IdFormaPago { get; set; }

        public string RazonSocial { get; set; }

        public PageRequest Page { get; set; }
    }
}
