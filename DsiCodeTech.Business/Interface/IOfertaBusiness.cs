using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business.Interface
{
    public interface IOfertaBusiness
    {
        /// <summary>
        /// Busca una oferta por el código de barras de un producto tomando que la oferta este activa 
        /// y no este caducada
        /// </summary>
        /// <param name="cod_barras"></param>
        /// <returns></returns>
        oferta_articulo GetActiveOfferByCodBar(string cod_barras);
    }
}
