using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DsiCodeTech.Common.Util;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class CorteDAO
    {
		public corte getCorte(DateTime fechaFin)
		{
			DateTime dateTime = DateTime.Now;
			SqlCommand sqlCommand = new SqlCommand("SELECT CONVERT(nvarchar,pos.last_corte_z,121) ultimo_corte FROM pos_settings pos", POSCaja.getConnectionLocal());
			sqlCommand.CommandType = CommandType.Text;
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				dateTime = DateTime.Parse(sqlDataReader["ultimo_corte"].ToString());
			}
			sqlCommand.Dispose();
			sqlCommand = null;
			sqlDataReader.Close();
			sqlDataReader.Dispose();
			sqlDataReader = null;
			GC.Collect();
	
     		string corteText = "SELECT\tpos.id_pos,\r\n\t\t@fechaIni\t\t\t[fecha_ini],\r\n\t\t@fechaFin\t\t\t[fecha_fin],\r\n\t\tMIN(vd.folio)\t\t[folio_ini],\r\n\t\tMAX(vd.folio)\t\t[folio_fin],\r\n\t\tSUM(vd.efectivo)\t[efectivo],\r\n\t\tSUM(vd.cheques)\t\t[cheques],\r\n\t\tSUM(vd.vales)\t\t[vales],\r\n\t\tSUM(vd.tc)\t\t\t[tc],\r\n\t\tSUM(vd.spei)\t    [spei],\r\n\t\tCOUNT(vd.id_pos)\t[no_transacciones],\r\n\t\tSUM(vd.total_vendido) [total_vendido],\r\n\t\tSUM(vd.total_desglosado) [total_desglosado],\r\n\t\tSUM(vd.iva)\t\t\t[iva],\r\n\t\tSUM(vd.ieps)\t    [ieps],\r\n\t\tSUM(vd.total_exentos) [total_exentos],\r\n\t\tAVG(vde.cant_dev)\t[total_devuelto]\r\nFROM pos_settings pos \r\n LEFT JOIN (SELECT vp.id_pos,\r\n\t\t\t\t   vp.id_venta,\r\n\t\t\t\t   vp.folio,\r\n\t\t\t\t   SUM(vd.total_venta)-(vp.pago_td+vp.pago_vales+vp.pago_tc+vp.pago_spei) [efectivo],\r\n\t\t\t\t   (vp.pago_td) [cheques],\r\n\t\t\t\t   (vp.pago_vales) [vales],\r\n\t\t\t\t   (vp.pago_tc) [tc],\r\n\t\t\t\t   (vp.pago_spei) [spei],\r\n\t\t\t\t   SUM(vd.total_iva) [iva],\r\n\t\t\t\t   SUM(vd.total_ieps) [ieps],\r\n\t\t\t\t   SUM(vd.total_venta) [total_vendido],\r\n\t\t\t\t   SUM(vd.total_desglosado) [total_desglosado],\r\n\t\t\t\t   SUM(vd.total_exentos) [total_exentos]\r\n\t\t\t\tFROM venta vp\r\n\t\t\t INNER JOIN (SELECT v.id_venta,\r\n       v.folio,\r\n\t   (va.precio_vta - (va.precio_vta * va.porcent_desc)) * (va.cantidad - va.cant_devuelta)\r\n\t   [total_venta],\r\n       CASE\r\n         WHEN va.iva > 0.000 THEN\r\n\t\t (va.precio_compra + (va.precio_compra * va.utilidad / 100) - ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) * va.porcent_desc )) *\r\n\t\t (va.cantidad - va.cant_devuelta)\r\n\r\n         ELSE 0.000\r\n       END [total_desglosado],\r\n\t   \r\n\t   ((va.precio_compra + (va.precio_compra * va.utilidad / 100) - ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) * va.porcent_desc )) *\r\n\t   (va.cantidad - va.cant_devuelta) +\r\n\t   ((va.precio_compra + (va.precio_compra * va.utilidad / 100) - ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) * va.porcent_desc )) *\r\n\t   (va.cantidad - va.cant_devuelta) * va.ieps)) * va.iva\r\n\r\n\t   [total_iva],\r\n\r\n       CASE\r\n         WHEN va.iva > 0.000 THEN 0.000\r\n         ELSE \r\n\t\t va.precio_vta * (va.cantidad - va.cant_devuelta) - \r\n\t\t (va.precio_vta * (va.cantidad - va.cant_devuelta)) * va.porcent_desc\r\n\r\n       END [total_exentos],\r\n\t   CASE \r\n\t\t\tWHEN va.ieps > 0.000 THEN \r\n\t\t\t((va.precio_compra + (va.precio_compra * va.utilidad / 100) - ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) * va.porcent_desc )) *\r\n\t\t\t(va.cantidad - va.cant_devuelta) ) * va.ieps\r\n\t\t\tELSE 0.000 END [total_ieps]\r\nFROM   venta v\r\n       INNER JOIN venta_articulo va\r\n               ON v.id_venta = va.id_venta\r\nWHERE  v.fecha_venta BETWEEN @fechaIni AND @fechaFin) vd ON vp.id_venta=vd.id_venta\r\n\t\t\tWHERE vp.fecha_venta BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY vp.id_pos,vp.id_venta,vp.folio, vp.fecha_venta, vp.pago_td, vp.pago_vales,vp.pago_tc, vp.pago_spei) vd ON vd.id_pos = pos.id_pos\r\n LEFT JOIN (SELECT dev.id_pos,SUM(dev.cant_dev) [cant_dev]\r\n\t\t\tFROM venta_devolucion dev\r\n\t\t\tWHERE dev.fecha_dev BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY dev.id_pos) vde ON vde.id_pos = pos.id_pos \r\nGROUP BY pos.id_pos;";
			#region Codigo anterior que no funciono
            //string corteText = "SELECT\tpos.id_pos,\r\n\t\t@fechaIni\t\t\t[fecha_ini],\r\n\t\t@fechaFin\t\t\t[fecha_fin],\r\n\t\tMIN(vd.folio)\t\t[folio_ini],\r\n\t\tMAX(vd.folio)\t\t[folio_fin],\r\n\t\tSUM(vd.efectivo)\t[efectivo],\r\n\t\tSUM(vd.cheques)\t\t[cheques],\r\n\t\tSUM(vd.vales)\t\t[vales],\r\n\t\tSUM(vd.tc)\t\t\t[tc],\r\n\t\tCOUNT(vd.id_pos)\t[no_transacciones],\r\n\t\tSUM(vd.total_vendido) [total_vendido],\r\n\t\tSUM(vd.total_desglosado) [total_desglosado],\r\n\t\tSUM(vd.iva)\t\t\t[iva],\r\n\t\tSUM(vd.ieps)\t    [ieps],\r\n\t\tSUM(vd.total_exentos) [total_exentos],\r\n\t\tAVG(vde.cant_dev)\t[total_devuelto]\r\nFROM pos_settings pos \r\n LEFT JOIN (SELECT vp.id_pos,\r\n\t\t\t\t   vp.id_venta,\r\n\t\t\t\t   vp.folio,\r\n\t\t\t\t   SUM(vd.total_venta)-(vp.pago_td+vp.pago_vales+vp.pago_tc) [efectivo],\r\n\t\t\t\t   (vp.pago_td) [cheques],\r\n\t\t\t\t   (vp.pago_vales) [vales],\r\n\t\t\t\t   (vp.pago_tc) [tc],\r\n\t\t\t\t   SUM(vd.total_iva) [iva],\r\n\t\t\t\t   SUM(vd.total_iva) [ieps],\r\n\t\t\t\t   SUM(vd.total_venta) [total_vendido],\r\n\t\t\t\t   SUM(vd.total_desglosado) [total_desglosado],\r\n\t\t\t\t   SUM(vd.total_exentos) [total_exentos]\r\n\t\t\t\tFROM venta vp\r\n\t\t\t INNER JOIN (SELECT v.id_venta,\r\n       v.folio,\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) +\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) +\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) * va.iva \r\n\t   -\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) +\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) +\r\n\t   ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) * va.iva) * va.porcent_desc)\r\n\t   *\r\n\t   (va.cantidad - va.cant_devuelta)\r\n\t   [total_venta],\r\n       CASE\r\n         WHEN va.iva > 0.000 THEN \r\n\t\t ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) -\r\n\t\t ((va.precio_compra + (va.precio_compra * va.utilidad / 100)) * va.porcent_desc )) * (va.cantidad - va.cant_devuelta)\r\n         ELSE 0.000\r\n       END [total_desglosado],\r\n\t   (( ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) - \r\n\t\t\t(((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) * va.porcent_desc) ) * (va.cantidad - va.cant_devuelta)) * va.iva\r\n\t   [total_iva],\r\n\r\n       CASE\r\n         WHEN va.iva > 0.000 THEN 0.000\r\n         ELSE ( va.precio_vta - va.precio_vta * va.porcent_desc ) * (\r\n              va.cantidad - va.cant_devuelta )\r\n       END                                [total_exentos],\r\n\t   CASE \r\n\t\t\tWHEN va.ieps > 0.000 THEN \r\n\t\t\t( ((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) - \r\n\t\t\t(((va.precio_compra + (va.precio_compra * (va.utilidad / 100))) * va.ieps) * va.porcent_desc) ) * (va.cantidad - va.cant_devuelta)\r\n\t\t\tELSE 0.000 END [total_ieps]\r\nFROM   venta v\r\n       INNER JOIN venta_articulo va\r\n               ON v.id_venta = va.id_venta\r\nWHERE  v.fecha_venta BETWEEN @fechaIni AND @fechaFin) vd ON vp.id_venta=vd.id_venta\r\n\t\t\tWHERE vp.fecha_venta BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY vp.id_pos,vp.id_venta,vp.folio, vp.fecha_venta, vp.pago_td, vp.pago_vales,vp.pago_tc) vd ON vd.id_pos = pos.id_pos\r\n LEFT JOIN (SELECT dev.id_pos,SUM(dev.cant_dev) [cant_dev]\r\n\t\t\tFROM venta_devolucion dev\r\n\t\t\tWHERE dev.fecha_dev BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY dev.id_pos) vde ON vde.id_pos = pos.id_pos \r\nGROUP BY pos.id_pos;";
            //string cmdText = "SELECT\tpos.id_pos,\r\n\t\t@fechaIni\t\t\t[fecha_ini],\r\n\t\t@fechaFin\t\t\t[fecha_fin],\r\n\t\tMIN(vd.folio)\t\t[folio_ini],\r\n\t\tMAX(vd.folio)\t\t[folio_fin],\r\n\t\tSUM(vd.efectivo)\t[efectivo],\r\n\t\tSUM(vd.cheques)\t\t[cheques],\r\n\t\tSUM(vd.vales)\t\t[vales],\r\n\t\tSUM(vd.tc)\t\t\t[tc],\r\n\t\tCOUNT(vd.id_pos)\t[no_transacciones],\r\n\t\tSUM(vd.total_vendido) [total_vendido],\r\n\t\tSUM(vd.total_desglosado) [total_desglosado],\r\n\t\tSUM(vd.iva)\t\t\t[iva],\r\n\t\tSUM(vd.total_exentos) [total_exentos],\r\n\t\tAVG(vde.cant_dev)\t[total_devuelto]\r\nFROM pos_settings pos \r\n LEFT JOIN (SELECT vp.id_pos,\r\n\t\t\t\t   vp.id_venta,\r\n\t\t\t\t   vp.folio,\r\n\t\t\t\t   SUM(vd.total_venta)-(vp.pago_td+vp.pago_vales+vp.pago_tc) [efectivo],\r\n\t\t\t\t   (vp.pago_td) [cheques],\r\n\t\t\t\t   (vp.pago_vales) [vales],\r\n\t\t\t\t   (vp.pago_tc) [tc],\r\n\t\t\t\t   SUM(vd.total_iva) [iva],\r\n\t\t\t\t   SUM(vd.total_venta) [total_vendido],\r\n\t\t\t\t   SUM(vd.total_desglosado) [total_desglosado],\r\n\t\t\t\t   SUM(vd.total_exentos) [total_exentos]\r\n\t\t\tFROM venta vp\r\n\t\t\t INNER JOIN (SELECT v.id_venta,\r\n\t\t\t\t\t\t\t\tv.folio,\r\n\t\t\t\t\t\t\t\t(((va.precio_vta / (1.000 + va.iva)) -  (va.precio_vta / (1.000 + va.iva) *  va.porcent_desc)) + ((va.precio_vta / (1.000 + va.iva)) -  (va.precio_vta / (1.000 + va.iva) *  va.porcent_desc)) * va.iva) * va.cantidad [total_venta],\r\n\t\t\t\t\t\t\t\tCASE WHEN va.iva > 0.000 THEN (va.precio_vta / (1.000+va.iva) - (va.precio_vta / (1.000 + va.iva) *  va.porcent_desc)) * (va.cantidad-va.cant_devuelta) ELSE 0.000 END [total_desglosado],\r\n\t\t\t\t\t\t\t\t((va.precio_vta / (1.000 + va.iva)) -  (va.precio_vta / (1.000 + va.iva) *  va.porcent_desc)) * va.iva * (va.cantidad-va.cant_devuelta) [total_iva],\r\n\t\t\t\t\t\t\t\tCASE WHEN va.iva > 0.000 THEN 0.000 ELSE (va.precio_vta - va.precio_vta * va.porcent_desc) * (va.cantidad-va.cant_devuelta) END [total_exentos]\r\n\t\t\t\t\t\t FROM venta v\r\n\t\t\t\t\t\t  INNER JOIN venta_articulo va ON v.id_venta=va.id_venta\r\n\t\t\t\t\t\t WHERE v.fecha_venta BETWEEN @fechaIni AND @fechaFin) vd ON vp.id_venta=vd.id_venta\r\n\t\t\tWHERE vp.fecha_venta BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY vp.id_pos,vp.id_venta,vp.folio, vp.fecha_venta, vp.pago_td, vp.pago_vales,vp.pago_tc) vd ON vd.id_pos = pos.id_pos\r\n LEFT JOIN (SELECT dev.id_pos,SUM(dev.cant_dev) [cant_dev]\r\n\t\t\tFROM venta_devolucion dev\r\n\t\t\tWHERE dev.fecha_dev BETWEEN @fechaIni AND @fechaFin\r\n\t\t\tGROUP BY dev.id_pos) vde ON vde.id_pos = pos.id_pos \r\nGROUP BY pos.id_pos";
            #endregion

            sqlCommand = new SqlCommand(corteText, POSCaja.getConnectionLocal());
			sqlCommand.Parameters.Add("@fechaIni", SqlDbType.DateTime).Value = dateTime;
			sqlCommand.Parameters.Add("@fechaFin", SqlDbType.DateTime).Value = fechaFin;
			sqlDataReader = sqlCommand.ExecuteReader();
			if (sqlDataReader.Read())
			{
				corte corte = new corte();
				if (sqlDataReader["folio_ini"].ToString().Equals(""))
				{
					return null;
				}
				corte.id_pos = int.Parse(sqlDataReader["id_pos"].ToString());
				corte.fecha_ini = DateTime.Parse(sqlDataReader["fecha_ini"].ToString());
				corte.fecha_fin = DateTime.Parse(sqlDataReader["fecha_fin"].ToString());
				corte.folio_ini = long.Parse(sqlDataReader["folio_ini"].ToString());
				corte.folio_fin = long.Parse(sqlDataReader["folio_fin"].ToString());
				corte.efectivo = decimal.Parse(sqlDataReader["efectivo"].ToString());
                corte.cheques = decimal.Parse(sqlDataReader["cheques"].ToString());
                corte.vales = decimal.Parse(sqlDataReader["vales"].ToString());
				corte.tc = decimal.Parse(sqlDataReader["tc"].ToString());
				corte.no_transacciones = int.Parse(sqlDataReader["no_transacciones"].ToString());
				corte.total_vendido = decimal.Parse(sqlDataReader["total_vendido"].ToString());
				//corte.total_desglosado = decimal.Parse(sqlDataReader["total_desglosado"].ToString());
				corte.iva = decimal.Parse(sqlDataReader["iva"].ToString());
				corte.ieps = decimal.Parse(sqlDataReader["ieps"].ToString());
				corte.total_exentos = decimal.Parse(sqlDataReader["total_exentos"].ToString());
				corte.total_devuelto = ((sqlDataReader["total_devuelto"].ToString().Length > 0) ? decimal.Parse(sqlDataReader["total_devuelto"].ToString()) : 0.0m);
				sqlDataReader.Close();
				sqlDataReader.Dispose();
				sqlDataReader = null;
				sqlCommand.Dispose();
				sqlCommand = null;
				GC.Collect();
				return corte;
			}
			sqlDataReader.Close();
			sqlDataReader.Dispose();
			sqlDataReader = null;
			sqlCommand.Dispose();
			sqlCommand = null;
			GC.Collect();
			return null;
		}

		public void setLastCut(DateTime fechaFin)
		{
			SqlCommand sqlCommand = new SqlCommand("UPDATE pos_settings SET last_corte_z=@fechaFin", POSCaja.getConnectionLocal());
			sqlCommand.Parameters.Add("@fechaFin", SqlDbType.DateTime).Value = fechaFin;
			sqlCommand.CommandType = CommandType.Text;
			sqlCommand.ExecuteNonQuery();
		}
		        
    }
}
