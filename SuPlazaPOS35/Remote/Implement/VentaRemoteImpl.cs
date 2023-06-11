using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using SuPlazaPOS35.Remote;
using SuPlazaPOS35.domain;
using SuPlazaPOS35.DAO;

namespace SuPlazaPOS35.Remote.Implement
{
    public class VentaRemoteImpl : IVentaRemote
    {
        private VentaDAO ventaDAO;

        public VentaRemoteImpl()
        {
            this.ventaDAO = new VentaDAO();
        }

        public void SyncSaleRemote(venta venta)
        {

        }
    }
}
