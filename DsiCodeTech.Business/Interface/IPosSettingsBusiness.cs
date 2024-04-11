using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business.Interface
{
    public interface IPosSettingsBusiness
    {
        DsiCodeTech.Repository.PosCaja.pos_settings GetPosSettings();
    }
}
