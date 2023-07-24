using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.Properties;

using DsiCodeTech.Common.Util;

namespace SuPlazaPOS35.DAO
{
    public class POSCaja
    {
		private static SqlConnection connectionLocal;

		private static SqlDataReader dr;

		public static SqlConnection getConnectionLocal()
		{
			if (connectionLocal == null)
			{
				connectionLocal = new SqlConnection(new SqlInjectConnectionToLinq().WithNameDatabase(Settings.Default.dbName).Build());
				connectionLocal.Open();
			}
			return connectionLocal;
		}

		public SqlDataReader GetDataReader(string sql)
		{
			SqlCommand sqlCommand = new SqlCommand(sql, getConnectionLocal());
			dr = sqlCommand.ExecuteReader();
			sqlCommand.Dispose();
			return dr;
		}

		public DataSet GetDataSet(string sql)
		{
			using SqlCommand sqlCommand = new SqlCommand(sql, getConnectionLocal());
			sqlCommand.CommandType = CommandType.Text;
			using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
			DataSet dataSet = new DataSet();
			sqlDataAdapter.Fill(dataSet);
			return dataSet;
		}

		public void ExecuteSQL(string sql)
		{
			new SqlCommand(sql, getConnectionLocal()).ExecuteNonQuery();
		}
	}
}
