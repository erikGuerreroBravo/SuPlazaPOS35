using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace DsiCodeTech.Common.Web.DataTransfer
{
    public class RequestWrapper<T>
    {
        [Required]
        public T Body { get; set; }
    }
}
