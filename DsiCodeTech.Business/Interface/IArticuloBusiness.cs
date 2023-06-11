using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business.Interface
{
    public interface IArticuloBusiness
    {
        articulo GetArticleByBarCode(string barcode);
    }
}
