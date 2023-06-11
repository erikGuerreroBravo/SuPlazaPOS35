using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DsiCodeTech.Common.Util;
using DsiCodeTech.Repository.PosCaja;
using SuPlazaPOS35.controller;
using SuPlazaPOS35.domain;
using SuPlazaPOS35.model;

namespace SuPlazaPOS35.DAO
{
    public class VentaDAO : POSCaja
    {
        #region Metodo Original
        //public SuPlazaPOS35.model.venta getSaleOutLast()
        //{
        //    string sql = "SELECT TOP 1 id_pos,id_venta,folio,fecha_venta,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc FROM venta ORDER BY folio DESC";
        //    SqlDataReader dataReader = GetDataReader(sql);
        //    if (dataReader.Read())
        //    {
        //        SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
        //        venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
        //        venta.id_venta = new Guid(dataReader["id_venta"].ToString());
        //        venta.folio = long.Parse(dataReader["folio"].ToString());
        //        venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
        //        venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
        //        venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
        //        venta.pago_cheque = decimal.Parse(dataReader["pago_cheque"].ToString());
        //        venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
        //        venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
        //        return venta;
        //    }
        //    return null;
        //}
        #endregion

        public SuPlazaPOS35.model.venta getSaleOutLast()
        {
            string sql = "SELECT TOP 1 id_pos,id_venta,folio,fecha_venta,total_vendido,pago_efectivo,pago_vales,pago_tc,pago_td,pago_spei FROM venta ORDER BY folio DESC";
            SqlDataReader dataReader = GetDataReader(sql);
            if (dataReader.Read())
            {
                SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
                venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
                venta.id_venta = new Guid(dataReader["id_venta"].ToString());
                venta.folio = long.Parse(dataReader["folio"].ToString());
                venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
                venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
                venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
                venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
                venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
                venta.pago_td = decimal.Parse(dataReader["pago_td"].ToString());
                venta.pago_spei = decimal.Parse(dataReader["pago_spei"].ToString());
                return venta;
            }
            return null;
        }




        #region Metodo Original
        //public SuPlazaPOS35.model.venta getSaleOutByFolio(long folio)
        //{
        //    string sql = $"SELECT id_pos,id_venta,folio,fecha_venta,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc FROM venta WHERE folio={folio}";
        //    SqlDataReader dataReader = GetDataReader(sql);
        //    if (dataReader.Read())
        //    {
        //        SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
        //        venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
        //        venta.id_venta = new Guid(dataReader["id_venta"].ToString());
        //        venta.folio = long.Parse(dataReader["folio"].ToString());
        //        venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
        //        venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
        //        venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
        //        venta.pago_cheque = decimal.Parse(dataReader["pago_cheque"].ToString());
        //        venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
        //        venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
        //        SuPlazaPOS35.model.venta result = venta;
        //        dataReader.Dispose();
        //        return result;
        //    }
        //    return null;
        //}
        #endregion

        /// <summary>
        /// Consulta la venta por el Folio de la venta
        /// </summary>
        /// <param name="folio">el numero de folio generadoo</param>
        /// <returns>regresa la entidad venta con los datos persistidos</returns>
        public SuPlazaPOS35.model.venta getSaleOutByFolio(long folio)
        {
            string sql = $"SELECT id_pos,id_venta,folio,fecha_venta,total_vendido,pago_efectivo,pago_vales,pago_tc,pago_td,pago_spei FROM venta WHERE folio={folio}";
            SqlDataReader dataReader = GetDataReader(sql);
            if (dataReader.Read())
            {
                SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
                venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
                venta.id_venta = new Guid(dataReader["id_venta"].ToString());
                venta.folio = long.Parse(dataReader["folio"].ToString());
                venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
                venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
                venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
                venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
                venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
                venta.pago_td = decimal.Parse(dataReader["pago_td"].ToString());
                venta.pago_spei = decimal.Parse(dataReader["pago_spei"].ToString());
                SuPlazaPOS35.model.venta result = venta;
                dataReader.Dispose();
                return result;
            }
            return null;
        }

        #region  Metodo Original

        //public void loadSaleOut(long folio)
        //{
        //    string sql = $"SELECT id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,supervisor FROM venta WHERE folio={folio}";
        //    SqlDataReader dataReader = GetDataReader(sql);
        //    if (dataReader.Read())
        //    {
        //        SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
        //        venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
        //        venta.id_venta = new Guid(dataReader["id_venta"].ToString());
        //        venta.vendedor = dataReader["vendedor"].ToString();
        //        venta.folio = long.Parse(dataReader["folio"].ToString());
        //        venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
        //        venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
        //        venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
        //        venta.pago_cheque = decimal.Parse(dataReader["pago_cheque"].ToString());
        //        venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
        //        venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
        //        venta.supervisor = dataReader["supervisor"].ToString();
        //        SuPlazaPOS35.model.venta venta2 = venta;
        //        string cmdText = string.Format("INSERT INTO venta(id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,supervisor) VALUES({0},'{1}','{2}',{3},'{4}',{5},{6},{7},{8},{9},{10})", venta2.id_pos, venta2.id_venta, venta2.vendedor, venta2.folio, venta2.fecha_venta.ToString("dd/MM/yyyy HH:mm:ss"), venta2.total_vendido, venta2.pago_efectivo, venta2.pago_cheque, venta2.pago_vales, venta2.pago_tc, (venta2.supervisor.Length > 0) ? $"'{venta2.supervisor}'" : "NULL");
        //        new SqlCommand(cmdText, POSCaja.getConnectionRemote()).ExecuteNonQuery();
        //        List<SuPlazaPOS35.model.venta_articulo> saleOutList = new VentaArticuloDAO().getSaleOutList(venta2.id_venta);
        //        foreach (SuPlazaPOS35.model.venta_articulo item in saleOutList)
        //        {
        //            cmdText = string.Format("INSERT INTO venta_articulo(id_pos,id_venta,no_articulo,cod_barras,cantidad,articulo_ofertado,precio_regular,cambio_precio,iva,precio_vta,porcent_desc,cant_dev,[user_name],id_devolucion) VALUES({0},'{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})", item.id_pos, item.id_venta, item.no_articulo, item.cod_barras, item.cantidad, item.articulo_ofertado ? "1" : "0", item.precio_regular, item.cambio_precio ? "1" : "0", item.iva, item.precio_vta, item.porcent_desc, item.cant_devuelta, (item.user_name.Length > 0) ? $"'{item.user_name}'" : "NULL", (item.id_devolucion.Length > 0) ? $"'{item.id_devolucion}'" : "NULL");
        //            new SqlCommand(cmdText, POSCaja.getConnectionRemote()).ExecuteNonQuery();
        //        }
        //        POSCaja.closeRemoteConnection();
        //        return;
        //    }
        //    throw new Exception("La Venta no existe");
        //}
        #endregion



        public void loadSaleOut(long folio)
        {
            string sql = $"SELECT id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_vales,pago_tc,pago_td,supervisor FROM venta WHERE folio={folio}";
            SqlDataReader dataReader = GetDataReader(sql);
            if (dataReader.Read())
            {
                SuPlazaPOS35.model.venta venta = new SuPlazaPOS35.model.venta();
                venta.id_pos = int.Parse(dataReader["id_pos"].ToString());
                venta.id_venta = new Guid(dataReader["id_venta"].ToString());
                venta.vendedor = dataReader["vendedor"].ToString();
                venta.folio = long.Parse(dataReader["folio"].ToString());
                venta.fecha_venta = DateTime.Parse(dataReader["fecha_venta"].ToString());
                venta.total_vendido = decimal.Parse(dataReader["total_vendido"].ToString());
                venta.pago_efectivo = decimal.Parse(dataReader["pago_efectivo"].ToString());
                venta.pago_vales = decimal.Parse(dataReader["pago_vales"].ToString());
                venta.pago_tc = decimal.Parse(dataReader["pago_tc"].ToString());
                venta.pago_td = decimal.Parse(dataReader["pago_td"].ToString());
                venta.supervisor = dataReader["supervisor"].ToString();
                SuPlazaPOS35.model.venta venta2 = venta;
                string cmdText = string.Format("INSERT INTO venta(id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_td,pago_vales,pago_tc,supervisor) VALUES({0},'{1}','{2}',{3},'{4}',{5},{6},{7},{8},{9},{10})", venta2.id_pos, venta2.id_venta, venta2.vendedor, venta2.folio, venta2.fecha_venta.ToString("dd/MM/yyyy HH:mm:ss"), venta2.total_vendido, venta2.pago_efectivo, venta2.pago_td, venta2.pago_vales, venta2.pago_tc, (venta2.supervisor.Length > 0) ? $"'{venta2.supervisor}'" : "NULL");
                new SqlCommand(cmdText, POSCaja.getConnectionRemote()).ExecuteNonQuery();
                List<SuPlazaPOS35.model.venta_articulo> saleOutList = new VentaArticuloDAO().getSaleOutList(venta2.id_venta);
                foreach (SuPlazaPOS35.model.venta_articulo item in saleOutList)
                {
                    cmdText = string.Format("INSERT INTO venta_articulo(id_pos,id_venta,no_articulo,cod_barras,cantidad,articulo_ofertado,precio_regular,cambio_precio,iva,precio_vta,porcent_desc,cant_dev,[user_name],id_devolucion) VALUES({0},'{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})", item.id_pos, item.id_venta, item.no_articulo, item.cod_barras, item.cantidad, item.articulo_ofertado ? "1" : "0", item.precio_regular, item.cambio_precio ? "1" : "0", item.iva, item.precio_vta, item.porcent_desc, item.cant_devuelta, (item.user_name.Length > 0) ? $"'{item.user_name}'" : "NULL", (item.id_devolucion.Length > 0) ? $"'{item.id_devolucion}'" : "NULL");
                    new SqlCommand(cmdText, POSCaja.getConnectionRemote()).ExecuteNonQuery();
                }
                POSCaja.closeRemoteConnection();
                return;
            }
            throw new Exception("La Venta no existe");
        }

        #region GetTotales
        /// <summary>
        /// Este metodo se encarga de consultar el total de la venta para impresion del resultado en un ticket
        /// </summary>
        /// <param name="id_venta">el identificador de la venta</param>
        /// <returns>la entidad total</returns>
        public totales GetTotales(Guid id_venta)
        {
            totales total = new totales();
            using (DataClassesPOSDataContext contexto = new DataClassesPOSDataContext())
            {
                var venta = contexto.venta.Where(p => p.id_venta.Equals(id_venta)).First();
                total.sub_total = venta.subtotal;
                total.iva = venta.iva_desglosado;
                total.ieps = venta.ieps_desglosado;
                total.descuento = venta.descuento;
                total.impuestos = venta.impuestos;
                total.total = venta.total_vendido;

                total.total_articulos = contexto.venta_articulo
                    .Join(contexto.articulo, va => va.cod_barras, a => a.cod_barras, (va, a) => new { venta_articulo = va, articulo = a })
                    .Join(contexto.unidad_medida, va => va.articulo.id_unidad, um => um.id_unidad, (va, um) => new { venta_articulo = va.venta_articulo, unidad_medida = um })
                    .Where(va => va.venta_articulo.id_venta.Equals(id_venta))
                    .Sum(va => va.unidad_medida.descripcion.Contains("Kg") || va.unidad_medida.descripcion.Contains("Gms") ? 1.0m : va.venta_articulo.cantidad);

                decimal subtotal = total.sub_total;
                new SuPlazaPosUtil().CuadrarTotales(ref subtotal, total.total, DsiCodeUtil.Sum(venta.subtotal, venta.impuestos));
                total.sub_total = subtotal;
            }
            return total;
        }
        #endregion

        #region Metodo Eliminado    
        /************** Este metodo es el original y candidato a liminarse con los nuevos cambios***********************/
        /// <summary>
        /// Este metodo se encarga de realizar la insercion de la venta y la venta_articulo
        /// </summary>
        /// <param name="v">la venta que se realiza en el punto de venta</param>
        //public void SaleOut(SuPlazaPOS35.domain.venta v)
        //{
        //    string sql = string.Format("INSERT INTO venta(id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_cheque,pago_vales,pago_tc,num_registros) VALUES({0},'{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}',{10})", v.id_pos, v.id_venta, v.vendedor, v.folio, v.fecha_venta, v.total_vendido.ToString("F3"), v.pago_efectivo.ToString("F3"),/* v.pago_cheque.ToString("F3"),*/ v.pago_vales.ToString("F3"), v.pago_tc.ToString("F3"), v.venta_articulo.Count);
        //    ExecuteSQL(sql);
        //    foreach (SuPlazaPOS35.domain.venta_articulo item in v.venta_articulo)
        //    {
        //        sql = string.Format("INSERT INTO venta_articulo(id_pos,id_venta,no_articulo,cod_barras,cantidad,articulo_ofertado,precio_regular,cambio_precio,iva,precio_vta,porcent_desc,user_name) VALUES({0},'{1}',{2},'{3}','{4}',{5},'{6}',{7},'{8}','{9}','{10}',{11})", item.id_pos, item.id_venta, item.no_articulo, item.cod_barras, item.cantidad, item.articulo_ofertado ? "1" : "0", item.precio_regular.ToString("F3"), item.cambio_precio ? "1" : "0", item.iva.ToString("F3"), item.precio_vta.ToString("F3"), item.porcent_desc.ToString("F3"), (item.user_name != null) ? $"'{item.user_name}'" : "NULL");
        //        ExecuteSQL(sql);
        //    }
        //}
        #endregion

        #region Metodo Corregido para la Venta del Sistema SaleOut
        //public void SaleOut(SuPlazaPOS35.domain.venta v)
        //{
        //    string sql = string.Format("INSERT INTO venta(id_pos,id_venta,vendedor,folio,fecha_venta,total_vendido,pago_efectivo,pago_vales,pago_tc,pago_td,pago_spei,num_registros) VALUES( {0}, '{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11})", v.id_pos,v.id_venta,v.vendedor,v.folio,v.fecha_venta,v.total_vendido.ToString("F3"),v.pago_efectivo.ToString("F3"),v.pago_vales.ToString("F3"),v.pago_tc.ToString("F3"),v.pago_td.ToString("F3"),v.pago_spei.ToString("F3"), v.venta_articulo.Count);
        //    //ejecutamos la sentencia de sql
        //    ExecuteSQL(sql);
        //    //iteramos sobre la coleccion que se lleva la venta y dentro contiene la venta articulo con todos los items de la venta
        //    foreach (SuPlazaPOS35.domain.venta_articulo item in v.venta_articulo)
        //    {
        //        sql = String.Format("INSERT INTO venta_articulo(id_pos,id_venta,no_articulo,cod_barras,cantidad,articulo_ofertado,precio_regular,cambio_precio,iva,precio_vta,porcent_desc,ieps,precio_compra,utilidad,user_name) VALUES({0},'{1}',{2},'{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14})", item.id_pos,item.id_venta,item.no_articulo, item.cod_barras,item.cantidad, item.articulo_ofertado ? "1": "0", item.precio_regular.ToString("F3"),item.cambio_precio ? "1": "0", item.iva.ToString("F3"), item.precio_vta.ToString("F3"), item.porcent_desc.ToString("F3"), item.ieps.HasValue ? item.ieps : 0, item.precio_compra.ToString("F3"), item.utilidad.ToString("F3"), (item.user_name != null) ? $"'{item.user_name}'" : "NULL");
        //        ExecuteSQL(sql);
        //    }

        //}

        public SuPlazaPOS35.domain.venta SaleOut(SuPlazaPOS35.domain.venta v)
        {
            v.num_registros = (short)v.venta_articulo.Count;
            using (var contexto = new DataClassesPOSDataContext())
            {
                contexto.venta.InsertOnSubmit(v);
                contexto.SubmitChanges();
                SuPlazaPOS35.domain.venta entidad = v;
                return entidad;
            }
        }

        #endregion

    }
}
