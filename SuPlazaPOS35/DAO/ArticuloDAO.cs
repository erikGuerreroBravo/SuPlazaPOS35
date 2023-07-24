using SuPlazaPOS35.domain;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SuPlazaPOS35.DAO
{
    public class ArticuloDAO: POSCaja
    {
		public decimal getOfferPrice(string barCode)
		{
			string cmdText = $"SELECT TOP(1) oa.precio_oferta FROM oferta_articulo oa JOIN oferta o ON oa.id_oferta=o.id_oferta JOIN (SELECT ISNULL(cod_asociado,cod_barras) cod_barras FROM articulo WHERE tipo_articulo IN ('principal','asociado') AND cod_barras='{barCode}') art ON oa.cod_barras=art.cod_barras\r\nWHERE oa.status_oferta='disponible' AND CONVERT(date, GETDATE()) BETWEEN o.fecha_ini AND o.fecha_fin ORDER BY oa.fecha_registro DESC";
			SqlCommand sqlCommand = new SqlCommand(cmdText, POSCaja.getConnectionLocal());
			sqlCommand.CommandType = CommandType.Text;
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (!sqlDataReader.Read())
			{
				return 0.0m;
			}
			return decimal.Parse(sqlDataReader["precio_oferta"].ToString());
		}

		/// <summary>
		/// Este metodo se encarga de consultar un articulo por codigo de barras
		/// </summary>
		/// <param name="codigoBarras">el codigo de barras</param>
		/// <returns>una entidad articulo</returns>
		public domain.articulo getArticuloByCodigoBarras(string codigoBarras)
		{ 
			DataClassesPOSDataContext dataClasses = new DataClassesPOSDataContext();
            var articulo =  dataClasses.articulo.FirstOrDefault(p => p.cod_barras.Equals(codigoBarras));
			return articulo;
		}
	}
}
