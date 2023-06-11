using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.Properties;

namespace SuPlazaPOS35.DAO
{
    public class DownloadDataTables
    {
        private string[] tablesDownload = new string[10] { "articulo", "usuario", "permiso", "usuario_permiso", "empleado", "unidad_medida", "oferta", "oferta_articulo", "factura_venta", "empresa" };

        public void LoadFromServer()
        {
            using SqlConnection sqlConnection = new SqlConnection(Settings.Default.pos_adminConnectionString);
            using SqlConnection sqlConnection2 = new SqlConnection(Settings.Default.pos_cajaConnectionString);
            sqlConnection.Open();
            sqlConnection2.Open();
            string[] array = tablesDownload;
            foreach (string text in array)
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter($"SELECT * FROM {text}", sqlConnection);
                sqlDataAdapter.Fill(dataTable);
                SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection2);
                sqlBulkCopy.DestinationTableName = text;
                sqlBulkCopy.WriteToServer(dataTable);
            }
            sqlConnection2.Close();
            sqlConnection.Close();
        }
    }
}
