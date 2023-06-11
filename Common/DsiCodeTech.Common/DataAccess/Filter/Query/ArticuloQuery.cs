using DsiCodeTech.Common.DataAccess.Filter.Page;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Filter.Query
{
    public class ArticuloQuery
    {
        [JsonProperty(propertyName: "cod_barras")]
        public string CodBarras { get; set; }

        [JsonProperty(propertyName: "descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty(propertyName: "cve_producto")]
        public string CveProducto { get; set; }

        [JsonProperty(propertyName: "cod_interno")]
        public string CodInterno { get; set; }

        [JsonProperty(propertyName: "id_proveedor")]
        public Nullable<Guid> IdProveedor { get; set; } = null;
        public PageRequest Page { get; set; }
    }
}
