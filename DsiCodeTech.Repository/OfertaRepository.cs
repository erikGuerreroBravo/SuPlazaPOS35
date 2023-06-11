using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class OfertaRepository : BaseRepository<oferta>
    {
        public OfertaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
