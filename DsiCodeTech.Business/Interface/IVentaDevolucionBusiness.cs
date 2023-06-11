using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaDevolucionBusiness
    {
        List<venta_devolucion> GetDevolucionesByDates(DateTime start, DateTime end);
    }
}