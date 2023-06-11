
using DsiCodeTech.Repository.PosCaja;
using System.Data.Entity;
using DsiCodeTech.Common.Util;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;

namespace DsiCodeTech.Repository.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly pos_caja_Entities _dbContext;

        public UnitOfWork()
        {
            _dbContext = new pos_caja_Entities(new SqlInjectConnection()
                .WithNameDatabase("pos_caja")
                .WithMetadata("res://*/PosCaja.PosCaja.csdl|res://*/PosCaja.PosCaja.ssdl|res://*/PosCaja.PosCaja.msl")
                .Build());
            _dbContext.Database.CommandTimeout = 30000;
        }

        public DbContext Db
        {
            get { return _dbContext; }
        }

        public void Dispose()
        {
        }
    }
}
