using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class empleado
    {
        public string user_name { get; set; }

        public string nombre { get; set; }

        public string a_paterno { get; set; }

        public string a_materno { get; set; }

        public string shortName()
        {
            return nombre + " " + a_paterno;
        }

        public string fullName()
        {
            return nombre + " " + a_paterno + " " + a_materno;
        }
    }
}
