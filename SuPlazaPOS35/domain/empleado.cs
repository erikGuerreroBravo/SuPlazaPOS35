using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.domain
{
	/// <summary>
	/// Company: DSI Tech
	/// Date: 18-05-2022
	/// Information: Está clase agrega funcionalidad fuera de la implementación de Linq.
	/// </summary>
	public partial class empleado
    {
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
