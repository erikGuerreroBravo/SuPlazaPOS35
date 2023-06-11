using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class VentaArticuloDAO : POSCaja
    {
        public List<domain.venta_articulo> getSaleOutDetail(Guid id_venta)
        {
            string sql2 = $"SELECT va.no_articulo,\r\nva.cod_barras,\r\na.descripcion,\r\na.descripcion_corta,\r\na.precio_compra,\r\na.utilidad,\r\nva.cantidad,\r\num.descripcion medida,\r\nva.iva,\r\nva.ieps,\r\nva.precio_vta,\r\nva.porcent_desc,\r\nva.cant_devuelta \r\nFROM venta_articulo va \r\nINNER JOIN articulo a \r\nON va.cod_barras=a.cod_barras \r\nINNER JOIN unidad_medida um \r\nON a.id_unidad=um.id_unidad\r\nWHERE va.id_venta='{{id_venta}}' ORDER BY va.no_articulo";
            //string sql = $"SELECT va.no_articulo,va.cod_barras,a.descripcion,a.descripcion_corta,va.cantidad,um.descripcion medida,va.iva,va.precio_vta,va.porcent_desc,va.cant_devuelta FROM venta_articulo va INNER JOIN articulo a ON va.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE va.id_venta='{id_venta}' ORDER BY va.no_articulo";
            DataSet dataSet = GetDataSet(sql2);
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
                        precio_compra = decimal.Parse(row["precio_compra"].ToString()),
                        utilidad = decimal.Parse(row["utilidad"].ToString()),
                        impuestos = new()
                        {
                            new domain.impuestos
                            {
                                iva = decimal.Parse(row["iva"].ToString()),
                                ieps = decimal.Parse(row["ieps"].ToString()),
                            }
                        },
                        unidad_medida = new domain.unidad_medida
                        {
                            descripcion = row["medida"].ToString(),
                        },

                    },

                    cantidad = decimal.Parse(row["cantidad"].ToString()),
                    precio_vta = decimal.Parse(row["precio_vta"].ToString()),
                    porcent_desc = decimal.Parse(row["porcent_desc"].ToString())
                });
            }
            dataSet.Dispose();
            if (list.Count <= 0)
            {
                return null;
            }
            return list;
        }


        public List<domain.venta_articulo> getSaleOutGroupDetail(Guid id_venta)
        {
            string sql2 = $"SELECT va.cod_barras,\r\nMIN(va.no_articulo) no_articulo,\r\na.descripcion,\r\na.descripcion_corta,\r\na.precio_compra,\r\na.precio_venta,\r\na.utilidad,\r\num.descripcion medida,\r\nSUM(va.cantidad) cantidad,\r\nAVG(va.iva) iva,\r\nAVG(va.ieps) ieps,\r\nAVG(va.precio_vta) precio_vta,\r\nAVG(va.porcent_desc) porcent_desc \r\nFROM venta_articulo va \r\nINNER JOIN articulo a\r\nON va.cod_barras=a.cod_barras INNER JOIN\r\nunidad_medida um ON\r\na.id_unidad=um.id_unidad \r\nWHERE va.id_venta='{id_venta}'\r\nGROUP BY va.cod_barras,a.descripcion,a.descripcion_corta,um.descripcion,a.precio_compra,precio_venta,\r\na.utilidad";

            /// string sql = $"SELECT va.cod_barras, MIN(va.no_articulo) no_articulo,a.descripcion,a.descripcion_corta,um.descripcion medida,SUM(va.cantidad) cantidad,AVG(va.iva) iva,AVG(va.precio_vta) precio_vta,AVG(va.porcent_desc) porcent_desc FROM venta_articulo va INNER JOIN articulo a ON va.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE va.id_venta='{id_venta}' GROUP BY va.cod_barras,a.descripcion,a.descripcion_corta,um.descripcion";
            DataSet dataSet = GetDataSet(sql2);
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
                        precio_compra = decimal.Parse(row["precio_compra"].ToString()),
                        utilidad = decimal.Parse(row["utilidad"].ToString()),
                        precio_venta = decimal.Parse(row["precio_venta"].ToString()),
                        impuestos = new()
                        { 
                            new domain.impuestos
                            {
                                iva = decimal.Parse(row["iva"].ToString()),
                                ieps = decimal.Parse(row["ieps"].ToString()),
                            }
                        },

                        unidad_medida = new domain.unidad_medida
                        {
                            descripcion = row["medida"].ToString(),
                        },


                    },
                    //descripcion = row["descripcion"].ToString(),
                    //descripcion_corta = row["descripcion_corta"].ToString(),
                    cantidad = decimal.Parse(row["cantidad"].ToString()),
                    //medida = row["medida"].ToString(),
                    //iva = decimal.Parse(row["iva"].ToString()),
                    //ieps = decimal.Parse(row["ieps"].ToString()),
                    precio_vta = decimal.Parse(row["precio_vta"].ToString()),
                    porcent_desc = decimal.Parse(row["porcent_desc"].ToString())
                });
            }
            dataSet.Dispose();
            if (list.Count <= 0)
            {
                return null;
            }
            return list.OrderBy((domain.venta_articulo i) => i.no_articulo).ToList();
        }



        //public List<venta_articulo> getSaleOutGroupDetail(Guid id_venta)
        //{
        //	string sql = $"SELECT va.cod_barras, MIN(va.no_articulo) no_articulo,a.descripcion,a.descripcion_corta,um.descripcion medida,SUM(va.cantidad) cantidad,AVG(va.iva) iva,AVG(va.precio_vta) precio_vta,AVG(va.porcent_desc) porcent_desc FROM venta_articulo va INNER JOIN articulo a ON va.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE va.id_venta='{id_venta}' GROUP BY va.cod_barras,a.descripcion,a.descripcion_corta,um.descripcion";
        //	DataSet dataSet = GetDataSet(sql);
        //	List<venta_articulo> list = new List<venta_articulo>();
        //	foreach (DataRow row in dataSet.Tables[0].Rows)
        //	{
        //		list.Add(new venta_articulo
        //		{
        //			cod_barras = row["cod_barras"].ToString(),
        //			no_articulo = int.Parse(row["no_articulo"].ToString()),
        //			descripcion = row["descripcion"].ToString(),
        //			descripcion_corta = row["descripcion_corta"].ToString(),
        //			cantidad = decimal.Parse(row["cantidad"].ToString()),
        //			medida = row["medida"].ToString(),
        //			iva = decimal.Parse(row["iva"].ToString()),
        //			precio_vta = decimal.Parse(row["precio_vta"].ToString()),
        //			porcent_desc = decimal.Parse(row["porcent_desc"].ToString())
        //		});
        //	}
        //	dataSet.Dispose();
        //	if (list.Count <= 0)
        //	{
        //		return null;
        //	}
        //	return list.OrderBy((venta_articulo i) => i.no_articulo).ToList();
        //}

        public List<venta_articulo> getSaleOutDevolutionDetail(Guid id_devolucion)
        {
            string sql = $"SELECT va.no_articulo,va.cod_barras,a.descripcion,a.descripcion_corta,va.cantidad,um.descripcion medida,va.iva,va.precio_vta,va.porcent_desc,va.cant_devuelta FROM venta_articulo va INNER JOIN articulo a ON va.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE va.id_devolucion='{id_devolucion}'";
            DataSet dataSet = GetDataSet(sql);
            List<venta_articulo> list = new List<venta_articulo>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                list.Add(new venta_articulo
                {
                    no_articulo = int.Parse(row["no_articulo"].ToString()),
                    cod_barras = row["cod_barras"].ToString(),
                    descripcion = row["descripcion"].ToString(),
                    descripcion_corta = row["descripcion_corta"].ToString(),
                    cantidad = decimal.Parse(row["cantidad"].ToString()),
                    medida = row["medida"].ToString(),
                    iva = decimal.Parse(row["iva"].ToString()),
                    precio_vta = decimal.Parse(row["precio_vta"].ToString()),
                    porcent_desc = decimal.Parse(row["porcent_desc"].ToString()),
                    cant_devuelta = decimal.Parse(row["cant_devuelta"].ToString())
                });
            }
            dataSet.Dispose();
            if (list.Count <= 0)
            {
                return null;
            }
            return list;
        }

        public List<venta_articulo> getSaleOutList(Guid id_venta)
        {
            string sql = $"SELECT va.id_pos,va.id_venta,va.no_articulo,va.cod_barras,a.descripcion,a.descripcion_corta,va.cantidad,um.descripcion medida,va.iva,va.precio_vta,va.porcent_desc,va.articulo_ofertado,va.precio_regular,va.cambio_precio,va.[user_name],va.id_devolucion FROM venta_articulo va INNER JOIN articulo a ON va.cod_barras=a.cod_barras INNER JOIN unidad_medida um ON a.id_unidad=um.id_unidad WHERE va.id_venta='{id_venta}'";
            DataSet dataSet = GetDataSet(sql);
            List<venta_articulo> list = new List<venta_articulo>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                venta_articulo venta_articulo = new venta_articulo();
                venta_articulo.id_pos = int.Parse(row["id_pos"].ToString());
                venta_articulo.id_venta = new Guid(row["id_venta"].ToString());
                venta_articulo.no_articulo = int.Parse(row["no_articulo"].ToString());
                venta_articulo.cod_barras = row["cod_barras"].ToString();
                venta_articulo.cantidad = decimal.Parse(row["cantidad"].ToString());
                venta_articulo.articulo_ofertado = bool.Parse(row["articulo_ofertado"].ToString());
                venta_articulo.precio_regular = decimal.Parse(row["precio_regular"].ToString());
                venta_articulo.cambio_precio = bool.Parse(row["cambio_precio"].ToString());
                venta_articulo.iva = decimal.Parse(row["iva"].ToString());
                venta_articulo.precio_vta = decimal.Parse(row["precio_vta"].ToString());
                venta_articulo.porcent_desc = decimal.Parse(row["porcent_desc"].ToString());
                venta_articulo.user_name = row["user_name"].ToString();
                venta_articulo.id_devolucion = row["id_devolucion"].ToString();
                list.Add(venta_articulo);
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
