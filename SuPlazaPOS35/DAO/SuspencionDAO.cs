using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.model;

using SuPlazaPOS35.controller;

namespace SuPlazaPOS35.DAO
{
    public class SuspencionDAO : POSCaja
    {
		public enum SuspendedStatus
		{
			Sale,
			Cancel
		}

		public void changeSuspendedStatus(Guid SuspendedID, SuspendedStatus status)
		{
			string empty = string.Empty;
			if (!SuspendedID.Equals(default(Guid)))
			{
				switch (status)
				{
					case SuspendedStatus.Sale:
						empty = string.Format("UPDATE venta_cancelada SET [status]='{0}' WHERE id_venta_cancel='{1}'", "vendida", SuspendedID);
						ExecuteSQL(empty);
						break;
					case SuspendedStatus.Cancel:
						empty = string.Format("UPDATE venta_cancelada SET [status]='{0}' WHERE id_venta_cancel='{1}'", "suspecancel", SuspendedID);
						ExecuteSQL(empty);
						break;
				}
			}
		}

		/// <summary>
		/// Metodo modificado para la venta_cancelada se agrego pago_spei y pago_td
		/// </summary>
		/// <param name="v"></param>
		public void setSaleSuspended(venta_cancelada v)
		{
			//necesitamos eliminar el pago_cheque
			string sql = $"DELETE FROM venta_cancelada_articulo WHERE id_venta_cancel='{v.id_venta_cancel}'";
			ExecuteSQL(sql);
			
			sql = ((!POS.SaleRecovery) ? string.Format("INSERT INTO venta_cancelada(id_pos,id_venta_cancel,vendedor,fecha,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,status,supervisor,pago_td,pago_spei) VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10}, {11},{12})", v.id_pos, v.id_venta_cancel, v.vendedor, v.fecha.ToString("yyyy/MM/dd HH:mm:ss"), v.total_vendido.ToString("F3"), v.pago_efectivo.ToString("F3"), v.pago_cheque.ToString("F3"), v.pago_vales.ToString("F3"), v.pago_tc.ToString("F3"), v.status, (v.supervisor != null) ? $"'{v.supervisor}'" : "NULL", v.pago_td.ToString("F3"), v.pago_spei.ToString("F3")) : string.Format("UPDATE venta_cancelada SET vendedor='{2}',fecha='{3}',total_vendido={4},pago_efectivo={5},pago_cheque={6},pago_vales={7},pago_tc={8},status='{9}',supervisor={10},pago_td={11}, pago_spei={12},upload=0 WHERE id_venta_cancel='{1}'", v.id_pos, v.id_venta_cancel, v.vendedor, v.fecha.ToString("yyyy/MM/dd HH:mm:ss"), v.total_vendido.ToString("F3"), v.pago_efectivo.ToString("F3"), v.pago_cheque.ToString("F3"), v.pago_vales.ToString("F3"), v.pago_tc.ToString("F3"), v.status, (v.supervisor != null) ? $"'{v.supervisor}'" : "NULL",v.pago_td.ToString("F3"),v.pago_spei.ToString("F3")));
			ExecuteSQL(sql);
			foreach (venta_cancelada_articulo item in v.venta_cancelada_articulo)
			{
				//se agregan las columnas de IVA y IEPS
				sql = string.Format("INSERT INTO venta_cancelada_articulo(id_pos,id_venta_cancel,no_articulo,cod_barras,cantidad,articulo_ofertado,precio_regular,cambio_precio,precio_vta,porcent_desc,[user_name],ieps,iva) VALUES({0},'{1}',{2},'{3}','{4}',{5},'{6}',{7},'{8}','{9}',{10},{11},{12})", item.id_pos, item.id_venta_cancel, item.no_articulo, item.cod_barras, item.cantidad, item.articulo_ofertado ? "1" : "0", item.precio_regular.ToString("F3"), item.cambio_precio ? "1" : "0", item.precio_vta.ToString("F3"), item.porcent_desc.ToString("F3"), (item.user_name != null) ? $"'{item.user_name}'" : "NULL",item.ieps.ToString("F3"),item.iva.ToString("F3"));
				ExecuteSQL(sql);
			}
		}

		public void deleteVentaSuspendidaTemporal(Guid SuspendeID)
		{
			string sql = $"DELETE FROM venta_cancelada_articulo WHERE id_venta_cancel='{SuspendeID}'";
			ExecuteSQL(sql);
			sql = $"DELETE FROM venta_cancelada WHERE id_venta_cancel='{SuspendeID}'";
			ExecuteSQL(sql);
		}

		public int existsSaleSuspended()
		{
			string sql = "SELECT id_venta_cancel FROM venta_cancelada WHERE status='suspendida'";
			DataSet dataSet = GetDataSet(sql);
			new List<venta_articulo>();
			return dataSet.Tables[0].Rows.Count;
		}

		/// <summary>
		/// eliminar  el campo pago_cheque aun no se ha eliminado
		/// </summary>
		/// <param name="id_venta_cancelada"></param>
		/// <returns></returns>
		public venta_cancelada getVentaSuspendida(Guid id_venta_cancelada)
		{
			string sql = $"SELECT id_pos,id_venta_cancel,vendedor,fecha,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,[status],supervisor FROM venta_cancelada WHERE [status]='suspendida' AND id_venta_cancel='{id_venta_cancelada}'";
			SqlDataReader dataReader = GetDataReader(sql);
			if (dataReader.Read())
			{
				venta_cancelada venta_cancelada = new venta_cancelada();
				venta_cancelada.id_pos = int.Parse(dataReader["id_pos"].ToString());
				venta_cancelada.id_venta_cancel = new Guid(dataReader["id_venta_cancel"].ToString());
				venta_cancelada.vendedor = dataReader["vendedor"].ToString();
				venta_cancelada.fecha = DateTime.Parse(dataReader["fecha"].ToString());
				venta_cancelada.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
				venta_cancelada.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
				venta_cancelada.pago_cheque = decimal.Parse(dataReader["pago_cheque"].ToString());
				venta_cancelada.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
				venta_cancelada.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
				venta_cancelada.status = dataReader["status"].ToString();
				venta_cancelada.supervisor = dataReader["supervisor"].ToString();
				venta_cancelada.venta_cancelada_articulo = getArticulosVentaSuspendida(new Guid(dataReader["id_venta_cancel"].ToString()));
				return venta_cancelada;
			}
			return null;
		}

		#region Metodo  GetVentaSuspendida Modificado y correcto
		/*
		public venta_cancelada getVentaSuspendida(Guid id_venta_cancelada)
		{
			string sql = $"SELECT id_pos,id_venta_cancel,vendedor,fecha,total_vendido,pago_efectivo,pago_vales,pago_tc,[status],supervisor,pago_td,pago_spei  FROM venta_cancelada WHERE [status]='suspendida' AND id_venta_cancel='{id_venta_cancelada}'";
			SqlDataReader dataReader = GetDataReader(sql);
			if (dataReader.Read())
			{
				venta_cancelada venta_cancelada = new venta_cancelada();
				venta_cancelada.id_pos = int.Parse(dataReader["id_pos"].ToString());
				venta_cancelada.id_venta_cancel = new Guid(dataReader["id_venta_cancel"].ToString());
				venta_cancelada.vendedor = dataReader["vendedor"].ToString();
				venta_cancelada.fecha = DateTime.Parse(dataReader["fecha"].ToString());
				venta_cancelada.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
				venta_cancelada.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
				//venta_cancelada.pago_cheque = decimal.Parse(dataReader["pago_cheque"].ToString());
				venta_cancelada.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
				venta_cancelada.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
				venta_cancelada.status = dataReader["status"].ToString();
				venta_cancelada.supervisor = dataReader["supervisor"].ToString();
				venta_cancelada.pago_td = decimal.Parse(dataReader["pago_td"].ToString());
				venta_cancelada.pago_spei = decimal.Parse(dataReader["pago_spei"].ToString());
				venta_cancelada.venta_cancelada_articulo = getArticulosVentaSuspendida(new Guid(dataReader["id_venta_cancel"].ToString()));
				return venta_cancelada;
			}
			return null;
		}*/

		#endregion






		/// <summary>
		/// eliminar la columna pago_cheque, agregar pago_td, pago_spei aun no se han agregado
		/// </summary>
		/// <returns></returns>
		public List<venta_cancelada> getVentasSuspendidas()
		{
			List<venta_cancelada> list = new List<venta_cancelada>();
			string sql = "SELECT id_pos,id_venta_cancel,vendedor,fecha,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,[status],supervisor FROM venta_cancelada WHERE [status]='suspendida' ORDER BY fecha DESC";
			DataSet dataSet = GetDataSet(sql);
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				list.Add(new venta_cancelada
				{
					id_pos = int.Parse(row["id_pos"].ToString()),
					id_venta_cancel = new Guid(row["id_venta_cancel"].ToString()),
					vendedor = row["vendedor"].ToString(),
					fecha = DateTime.Parse(row["fecha"].ToString()),
					total_vendido = decimal.Parse(row["total_vendido"].ToString()),
					pago_efectivo = decimal.Parse(row["pago_efectivo"].ToString()),
					pago_cheque = decimal.Parse(row["pago_cheque"].ToString()),
					pago_vales = decimal.Parse(row["pago_vales"].ToString()),
					pago_tc = decimal.Parse(row["pago_tc"].ToString()),
					status = row["status"].ToString(),
					supervisor = row["supervisor"].ToString()
				});
			}
			dataSet.Dispose();
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}

		#region Metodo GetVentasSuependidas Corregido con columnas
		//public List<venta_cancelada> getVentasSuspendidas()
		//{
		//	List<venta_cancelada> list = new List<venta_cancelada>();
		//	string sql = "SELECT id_pos,id_venta_cancel,vendedor,fecha,total_vendido,pago_efectivo,pago_vales,pago_tc,[status],supervisor,pago_td,pago_spei FROM venta_cancelada WHERE [status]='suspendida' ORDER BY fecha DESC";
		//	DataSet dataSet = GetDataSet(sql);
		//	foreach (DataRow row in dataSet.Tables[0].Rows)
		//	{
		//		list.Add(new venta_cancelada
		//		{
		//			id_pos = int.Parse(row["id_pos"].ToString()),
		//			id_venta_cancel = new Guid(row["id_venta_cancel"].ToString()),
		//			vendedor = row["vendedor"].ToString(),
		//			fecha = DateTime.Parse(row["fecha"].ToString()),
		//			total_vendido = decimal.Parse(row["total_vendido"].ToString()),
		//			pago_efectivo = decimal.Parse(row["pago_efectivo"].ToString()),
		//			//pago_cheque = decimal.Parse(row["pago_cheque"].ToString()),
		//			pago_vales = decimal.Parse(row["pago_vales"].ToString()),
		//			pago_tc = decimal.Parse(row["pago_tc"].ToString()),
		//			status = row["status"].ToString(),
		//			supervisor = row["supervisor"].ToString(),
		//			pago_td = decimal.Parse(row["pago_td"].ToString()),
		//			pago_spei = decimal.Parse(row["pago_spei"].ToString())

		//		});
		//	}
		//	dataSet.Dispose();
		//	if (list.Count <= 0)
		//	{
		//		return null;
		//	}
		//	return list;
		//}
		#endregion



		public List<venta_cancelada_articulo> getArticulosVentaSuspendida(Guid id_venta_cancel)
		{
			List<venta_cancelada_articulo> list = new List<venta_cancelada_articulo>();
			string sql = $"SELECT id_pos,id_venta_cancel,no_articulo,vca.cod_barras,a.descripcion,um.descripcion [unidad_medida],cantidad,articulo_ofertado,precio_regular,cambio_precio,precio_vta,porcent_desc,[user_name] FROM venta_cancelada_articulo vca  INNER JOIN articulo a ON vca.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE id_venta_cancel='{id_venta_cancel}'";
			DataSet dataSet = GetDataSet(sql);
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				list.Add(new venta_cancelada_articulo
				{
					id_pos = int.Parse(row["id_pos"].ToString()),
					id_venta_cancel = new Guid(row["id_venta_cancel"].ToString()),
					no_articulo = int.Parse(row["no_articulo"].ToString()),
					cod_barras = row["cod_barras"].ToString(),
					descripcion = row["descripcion"].ToString(),
					unidad_medida = row["unidad_medida"].ToString(),
					cantidad = decimal.Parse(row["cantidad"].ToString()),
					articulo_ofertado = bool.Parse(row["articulo_ofertado"].ToString()),
					precio_regular = decimal.Parse(row["precio_regular"].ToString()),
					cambio_precio = bool.Parse(row["cambio_precio"].ToString()),
					precio_vta = decimal.Parse(row["precio_vta"].ToString()),
					porcent_desc = decimal.Parse(row["porcent_desc"].ToString()),
					user_name = row["user_name"].ToString()
				});
			}
			dataSet.Dispose();
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}
	}
}
