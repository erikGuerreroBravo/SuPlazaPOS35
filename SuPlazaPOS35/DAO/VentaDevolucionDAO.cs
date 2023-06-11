using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class VentaDevolucionDAO : POSCaja
    {
        public venta_devolucion getSaleOutDevolution(Guid id_devolucion)
        {
            string sql = $"SELECT id_devolucion,folio,id_pos,id_venta,fecha_dev,cant_dev,vendedor,supervisor,impuestos,descuento FROM venta_devolucion WHERE id_devolucion='{id_devolucion}'";
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
                venta_devolucion.descuento = decimal.Parse(dataReader["descuento"].ToString());
                venta_devolucion.impuestos = decimal.Parse(dataReader["impuestos"].ToString());
                venta_devolucion result = venta_devolucion;
                dataReader.Dispose();
                return result;
            }

            return null;
        }

        public List<domain.venta_articulo> getSaleOutDevolutionDetail(Guid DevolutionID)
        {
            try
            {
                string devolucionSql = $"SELECT \r\nva.no_articulo, \r\nva.cod_barras, \r\na.descripcion, \r\na.descripcion_corta, \r\nva.cantidad, \r\num.descripcion medida, \r\nva.precio_vta,\r\nva.porcent_desc,\r\nva.iva, \r\nva.ieps,\r\nvda.cantidad cant_devuelta \r\nFROM venta_devolucion_articulo vda \r\nINNER JOIN venta_devolucion vd\r\nON vda.id_devolucion=vd.id_devolucion \r\nINNER JOIN articulo a ON vda.cod_barras=a.cod_barras \r\nINNER JOIN venta_articulo va \r\nON vda.no_articulo=va.no_articulo \r\nAND vd.id_venta=va.id_venta \r\nINNER JOIN unidad_medida um \r\nON a.id_unidad=um.id_unidad WHERE vda.id_devolucion='{DevolutionID}'";
                DataSet dataSet = GetDataSet(devolucionSql);
                List<domain.venta_articulo> list = new List<domain.venta_articulo>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    list.Add(new domain.venta_articulo
                    {
                        cod_barras = row["cod_barras"].ToString(),
                        no_articulo = int.Parse(row["no_articulo"].ToString()),
                        articulo = new domain.articulo
                        {
                            cod_barras = row["cod_barras"].ToString(),
                            descripcion = row["descripcion"].ToString(),
                            descripcion_corta = row["descripcion_corta"].ToString(),
                            precio_venta = decimal.Parse(row["precio_vta"].ToString()),
                            unidad_medida = new domain.unidad_medida
                            {
                                descripcion = row["medida"].ToString(),
                            },
                            impuestos = new() {
                               new domain.impuestos{
                                iva = decimal.Parse(row["iva"].ToString()) * 100,
                                ieps= decimal.Parse(row["ieps"].ToString()) * 100
                               }
                            }

                        },
                        cantidad_a_devolver = decimal.Parse(row["cant_devuelta"].ToString()),
                        precio_vta = decimal.Parse(row["precio_vta"].ToString()),
                        porcent_desc = decimal.Parse(row["porcent_desc"].ToString()),

                    });
                    dataSet.Dispose();
                    if (list.Count <= 0)
                    {
                        return null;
                    }

                }
                return list;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }

        }
        public List<venta_devolucion> getListSaleOutDevolution(DateTime fecha_ini, DateTime fecha_fin)
        {
            string sql = string.Format("SELECT id_devolucion,folio,id_pos,id_venta,fecha_dev,cant_dev,vendedor,supervisor FROM venta_devolucion WHERE fecha_dev BETWEEN '{0}' AND '{1}' ORDER BY folio", fecha_ini.ToString("yyyy-MM-dd HH:mm:ss"), fecha_fin.ToString("yyyy/MM/dd HH:mm:ss")); //dd/MM/yyyy
            SqlDataReader dataReader = GetDataReader(sql);
            List<venta_devolucion> list = new List<venta_devolucion>();
            while (dataReader.Read())
            {
                list.Add(new venta_devolucion
                {
                    id_devolucion = new Guid(dataReader["id_devolucion"].ToString()),
                    folio = long.Parse(dataReader["folio"].ToString()),
                    id_pos = int.Parse(dataReader["id_pos"].ToString()),
                    id_venta = new Guid(dataReader["id_venta"].ToString()),
                    fecha_dev = DateTime.Parse(dataReader["fecha_dev"].ToString()),
                    cant_dev = decimal.Parse(dataReader["cant_dev"].ToString()),
                    vendedor = dataReader["vendedor"].ToString(),
                    supervisor = dataReader["supervisor"].ToString()
                });
            }
            dataReader.Dispose();
            if (list.Count <= 0)
            {
                return null;
            }
            return list;
        }
    }
}
