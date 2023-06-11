using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.Properties;

namespace SuPlazaPOS35.DAO
{
    public class POSCaja
    {
		private static SqlConnection connectionLocal;

		private static SqlConnection connectionRemote;

		private static SqlDataReader dr;

		public static SqlConnection getConnectionLocal()
		{
			if (connectionLocal == null)
			{
				connectionLocal = new SqlConnection(Settings.Default.pos_cajaConnectionString + ";MultipleActiveResultSets=true;");
				connectionLocal.Open();
			}
			return connectionLocal;
		}

		public static SqlConnection getConnectionRemote()
		{
			if (connectionRemote == null)
			{
				connectionRemote = new SqlConnection(Settings.Default.pos_adminConnectionString + ";MultipleActiveResultSets=true;");
				connectionRemote.Open();
			}
			return connectionRemote;
		}

		public static void closeRemoteConnection()
		{
			if (connectionRemote != null)
			{
				connectionRemote.Close();
				connectionRemote.Dispose();
			}
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
