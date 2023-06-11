using System.Data.SqlClient;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class EmpleadoDAO : POSCaja //Plaza15$_                        

    {
		public empleado getEmployeeByUserName(string user_name)
		
		{
			string sql = $"SELECT nombre,a_paterno,a_materno,[user_name] FROM empleado WHERE [user_name]='{user_name}'";
			SqlDataReader dataReader = GetDataReader(sql);
			if (dataReader.Read())
			{
				empleado empleado = new empleado();
				empleado.user_name = dataReader["user_name"].ToString();
				empleado.nombre = dataReader["nombre"].ToString();
				empleado.a_paterno = dataReader["a_paterno"].ToString();
				empleado.a_materno = dataReader["a_materno"].ToString();
				empleado result = empleado;
				dataReader.Dispose();
				return result;
			}
			return null;
		}
	}
}
