using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class ArchivoDM
    {
        public FileInfo FileInfo { get; set; }

        public string ContentType { get; set; }

        public byte[] File { get; set; }
    }
}
