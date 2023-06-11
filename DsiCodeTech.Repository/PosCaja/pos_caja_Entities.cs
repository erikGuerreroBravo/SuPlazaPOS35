using System.Data.Entity;

namespace DsiCodeTech.Repository.PosCaja
{
    public partial class pos_caja_Entities :DbContext
    {
        public pos_caja_Entities(string StringConnection) : base(StringConnection) { }
    }
}
