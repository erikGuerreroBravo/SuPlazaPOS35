using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DsiCodeTech.Common.Util;
using SuPlazaPOS35.domain;

namespace SuPlazaPOS35.model
{

public partial class venta_articulo
{

    public int id_pos { get; set; }

    public Guid id_venta { get; set; }

    public int no_articulo { get; set; }

    public string cod_barras { get; set; }

    public string descripcion { get; set; }

    public string descripcion_corta { get; set; }

    public decimal cantidad { get; set; }

    public string medida { get; set; }

    public bool articulo_ofertado { get; set; }

    public decimal iva { get; set; }

    public decimal precio_vta { get; set; }

    public decimal porcent_desc { get; set; }

    public decimal cant_devuelta { get; set; }

    public decimal precio_regular { get; set; }

    public bool cambio_precio { get; set; }

    public string user_name { get; set; }

    public string id_devolucion { get; set; }


    
 }
}
