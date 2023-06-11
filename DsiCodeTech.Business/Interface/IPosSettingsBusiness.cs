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
        pos_settings GetPosSettings();
    }
}
