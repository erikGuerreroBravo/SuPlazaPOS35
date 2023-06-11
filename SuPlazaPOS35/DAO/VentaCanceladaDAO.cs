using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class VentaCanceladaDAO : POSCaja
    {
        public venta_devolucion getSaleOutDevolution(Guid id_devolucion)
        {
            string sql = $"SELECT id_devolucion,folio,id_pos,id_venta,fecha_dev,cant_dev,vendedor,supervisor FROM venta_devolucion WHERE id_devolucion='{id_devolucion}'";
            SqlDataReader dataReader = GetDataReader(sql);
            if (dataReader.Read())
            {
                venta_devolucion venta_devolucion = new venta_devolucion();
                venta_devolucion.id_devolucion = new Guid(dataReader["id_devolucion"].ToString());
                venta_devolucion.folio = long.Parse(dataReader["folio"].ToString());
                venta_devolucion.id_pos = int.Parse(dataReader["id_pos"].ToString());
                venta_devolucion.id_venta = new Guid(dataReader["id_venta"].ToString());
                venta_devolucion.fecha_dev = DateTime.Parse(dataReader["fecha_dev"].ToString());
                venta_devolucion.cant_dev = decimal.Parse(dataReader["cant_dev"].ToString());
                venta_devolucion.vendedor = dataReader["vendedor"].ToString();
                venta_devolucion.supervisor = dataReader["supervisor"].ToString();
                venta_devolucion result = venta_devolucion;
                dataReader.Dispose();
                return result;
            }
            return null;
        }

        public List<venta_devolucion> getListSaleOutDevolution(DateTime fecha_ini, DateTime fecha_fin)
        {
            string sql = string.Format("SELECT id_devolucion,folio,id_pos,id_venta,fecha_dev,cant_dev,vendedor,supervisor FROM venta_devolucion WHERE fecha_dev BETWEEN '{0}' AND '{1}' ORDER BY folio", fecha_ini.ToString("dd/MM/yyyy HH:mm:ss"), fecha_fin.ToString("dd/MM/yyyy HH:mm:ss"));
            DataSet dataSet = GetDataSet(sql);
            List<venta_devolucion> list = new List<venta_devolucion>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                list.Add(new venta_devolucion
                {
                    id_devolucion = new Guid(row["id_devolucion"].ToString()),
                    folio = long.Parse(row["folio"].ToString()),
                    id_pos = int.Parse(row["id_pos"].ToString()),
                    id_venta = new Guid(row["id_venta"].ToString()),
                    fecha_dev = DateTime.Parse(row["fecha_dev"].ToString()),
                    cant_dev = decimal.Parse(row["cant_dev"].ToString()),
                    vendedor = row["vendedor"].ToString(),
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
    }
}
