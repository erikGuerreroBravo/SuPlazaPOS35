using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SuPlazaPOS35.DomainServer;
using SuPlazaPOS35.domain;

namespace SuPlazaPOS35.Synchronizer
{
    public class Download
    {
        private pos_adminDataContext dcServer;

        private DataClassesPOSDataContext dcLocal;

        private DateTime LastUpdate = DateTime.Parse("01/01/2016 07:00:00");

        private short TimeThread = 500;

        public static bool isWorking { get; set; }

        public Download()
        {
            dcServer = new pos_adminDataContext();
            dcLocal = new DataClassesPOSDataContext();
        }

        public string DownloadNow()
        {
            try
            {
                isWorking = true;
                Thread.Sleep(TimeThread);
                Empresa();
                Thread.Sleep(TimeThread);
                Permisos();
                Thread.Sleep(TimeThread);
                Usuarios();
                Thread.Sleep(TimeThread);
                Empleados();
                Thread.Sleep(TimeThread);
                Articulos();
                Thread.Sleep(TimeThread);
                UnidadesMedida();
                Thread.Sleep(TimeThread);
                Ofertas();
                Thread.Sleep(TimeThread);
                Facturas();
                dcLocal.Dispose();
                dcServer.Dispose();
                GC.Collect();
                isWorking = false;
                return string.Format("Última descarga: {0}", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
            }
            catch (Exception ex)
            {
                return string.Format(ex.Message);
            }
        }

        private void Empresa()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.empresa.FirstOrDefault() != null) ? dcLocal.empresa.Max((SuPlazaPOS35.domain.empresa e) => e.fecha_registro) : LastUpdate);
                SuPlazaPOS35.DomainServer.empresa eRemote = dcServer.empresa.FirstOrDefault((SuPlazaPOS35.DomainServer.empresa e) => e.fecha_registro > lastChangeUpdate);
                if (eRemote != null)
                {
                    SuPlazaPOS35.domain.empresa empresa = dcLocal.empresa.FirstOrDefault((SuPlazaPOS35.domain.empresa e) => e.rfc.Equals(eRemote.rfc));
                    if (empresa == null)
                    {
                        empresa = new SuPlazaPOS35.domain.empresa();
                        empresa.rfc = eRemote.rfc;
                        empresa.razon_social = eRemote.razon_social;
                        empresa.representante = eRemote.representante;
                        empresa.calle = eRemote.calle;
                        empresa.no_ext = eRemote.no_ext;
                        empresa.no_int = eRemote.no_int;
                        empresa.colonia = eRemote.colonia;
                        empresa.municipio = eRemote.municipio;
                        empresa.estado = eRemote.estado;
                        empresa.pais = eRemote.pais;
                        empresa.codigo_postal = eRemote.codigo_postal;
                        empresa.tel_principal = eRemote.tel_principal;
                        empresa.e_mail = eRemote.e_mail;
                        empresa.logo = eRemote.logo;
                        empresa.fecha_registro = eRemote.fecha_registro;
                        dcLocal.empresa.InsertOnSubmit(empresa);
                    }
                    else
                    {
                        empresa.razon_social = eRemote.razon_social;
                        empresa.representante = eRemote.representante;
                        empresa.calle = eRemote.calle;
                        empresa.no_ext = eRemote.no_ext;
                        empresa.no_int = eRemote.no_int;
                        empresa.colonia = eRemote.colonia;
                        empresa.municipio = eRemote.municipio;
                        empresa.estado = eRemote.estado;
                        empresa.pais = eRemote.pais;
                        empresa.codigo_postal = eRemote.codigo_postal;
                        empresa.tel_principal = eRemote.tel_principal;
                        empresa.e_mail = eRemote.e_mail;
                        empresa.logo = eRemote.logo;
                        empresa.fecha_registro = eRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Empresa");
            }
        }

        private void Permisos()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.permiso.FirstOrDefault() != null) ? dcLocal.permiso.Max((SuPlazaPOS35.domain.permiso e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.permiso> list = dcServer.permiso.Where((SuPlazaPOS35.DomainServer.permiso e) => e.fecha_registro > lastChangeUpdate).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.permiso eRemote in list)
                {
                    SuPlazaPOS35.domain.permiso permiso = dcLocal.permiso.FirstOrDefault((SuPlazaPOS35.domain.permiso e) => e.id_permiso.Equals(eRemote.id_permiso));
                    if (permiso == null)
                    {
                        permiso = new SuPlazaPOS35.domain.permiso();
                        permiso.id_permiso = eRemote.id_permiso;
                        permiso.descripcion = eRemote.descripcion;
                        permiso.tipo_sistema = eRemote.tipo_sistema;
                        permiso.fecha_registro = eRemote.fecha_registro;
                        dcLocal.permiso.InsertOnSubmit(permiso);
                        dcLocal.SubmitChanges();
                    }
                    else
                    {
                        permiso.descripcion = eRemote.descripcion;
                        permiso.tipo_sistema = eRemote.tipo_sistema;
                        permiso.fecha_registro = eRemote.fecha_registro;
                        dcLocal.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Permisos");
            }
        }

        private void Usuarios()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.usuario.FirstOrDefault() != null) ? dcLocal.usuario.Max((SuPlazaPOS35.domain.usuario e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.usuario> list = dcServer.usuario.Where((SuPlazaPOS35.DomainServer.usuario e) => e.fecha_registro > lastChangeUpdate).ToList();
                if (list != null)
                {
                    foreach (SuPlazaPOS35.DomainServer.usuario eRemote in list)
                    {
                        SuPlazaPOS35.domain.usuario usuario = dcLocal.usuario.FirstOrDefault((SuPlazaPOS35.domain.usuario e) => e.user_name.Equals(eRemote.user_name));
                        if (usuario == null)
                        {
                            usuario = new SuPlazaPOS35.domain.usuario();
                            usuario.user_name = eRemote.user_name;
                            usuario.password = eRemote.password;
                            usuario.tipo_usuario = eRemote.tipo_usuario;
                            usuario.enable = eRemote.enable;
                            usuario.user_code_bascula = eRemote.user_code_bascula;
                            usuario.fecha_registro = eRemote.fecha_registro;
                            dcLocal.usuario.InsertOnSubmit(usuario);
                        }
                        else
                        {
                            usuario.password = eRemote.password;
                            usuario.tipo_usuario = eRemote.tipo_usuario;
                            usuario.enable = eRemote.enable;
                            usuario.user_code_bascula = eRemote.user_code_bascula;
                            usuario.fecha_registro = eRemote.fecha_registro;
                        }
                        List<SuPlazaPOS35.domain.usuario_permiso> entities = dcLocal.usuario_permiso.Where((SuPlazaPOS35.domain.usuario_permiso e) => e.user_name.Equals(eRemote.user_name)).ToList();
                        dcLocal.usuario_permiso.DeleteAllOnSubmit(entities);
                        dcLocal.SubmitChanges();
                        List<SuPlazaPOS35.DomainServer.usuario_permiso> list2 = dcServer.usuario_permiso.Where((SuPlazaPOS35.DomainServer.usuario_permiso e) => e.user_name.Equals(eRemote.user_name)).ToList();
                        if (list2 == null)
                        {
                            continue;
                        }
                        foreach (SuPlazaPOS35.DomainServer.usuario_permiso item in list2)
                        {
                            SuPlazaPOS35.domain.usuario_permiso usuario_permiso = new SuPlazaPOS35.domain.usuario_permiso();
                            usuario_permiso.user_name = item.user_name;
                            usuario_permiso.id_permiso = item.id_permiso;
                            usuario_permiso.valor_num = item.valor_num;
                            usuario_permiso.fecha_registro = item.fecha_registro;
                            dcLocal.usuario_permiso.InsertOnSubmit(usuario_permiso);
                            dcLocal.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Usuarios");
            }
            UsuariosPermisos();
        }

        private void UsuariosPermisos()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.usuario_permiso.FirstOrDefault() != null) ? dcLocal.usuario_permiso.Max((SuPlazaPOS35.domain.usuario_permiso e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.usuario_permiso> list = dcServer.usuario_permiso.Where((SuPlazaPOS35.DomainServer.usuario_permiso e) => e.fecha_registro > lastChangeUpdate).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.usuario_permiso eRemote in list)
                {
                    SuPlazaPOS35.domain.usuario_permiso usuario_permiso = dcLocal.usuario_permiso.FirstOrDefault((SuPlazaPOS35.domain.usuario_permiso e) => e.user_name.Equals(eRemote.user_name) && e.id_permiso.Equals(eRemote.id_permiso));
                    if (usuario_permiso == null)
                    {
                        usuario_permiso = new SuPlazaPOS35.domain.usuario_permiso();
                        usuario_permiso.user_name = eRemote.user_name;
                        usuario_permiso.id_permiso = eRemote.id_permiso;
                        usuario_permiso.valor_num = eRemote.valor_num;
                        usuario_permiso.fecha_registro = eRemote.fecha_registro;
                        dcLocal.usuario_permiso.InsertOnSubmit(usuario_permiso);
                    }
                    else
                    {
                        usuario_permiso.valor_num = eRemote.valor_num;
                        usuario_permiso.fecha_registro = eRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Permisos de Usuario");
            }
        }

        private void Empleados()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.empleado.FirstOrDefault() != null) ? dcLocal.empleado.Max((SuPlazaPOS35.domain.empleado e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.empleado> list = dcServer.empleado.Where((SuPlazaPOS35.DomainServer.empleado e) => e.fecha_registro > lastChangeUpdate).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.empleado eRemote in list)
                {
                    SuPlazaPOS35.domain.empleado empleado = dcLocal.empleado.FirstOrDefault((SuPlazaPOS35.domain.empleado e) => e.id_empleado.Equals(eRemote.id_empleado));
                    if (empleado == null)
                    {
                        empleado = new SuPlazaPOS35.domain.empleado();
                        empleado.id_empleado = eRemote.id_empleado;
                        empleado.nombre = eRemote.nombre;
                        empleado.a_paterno = eRemote.a_paterno;
                        empleado.a_materno = eRemote.a_materno;
                        empleado.user_name = eRemote.user_name;
                        empleado.fecha_registro = eRemote.fecha_registro;
                        dcLocal.empleado.InsertOnSubmit(empleado);
                    }
                    else
                    {
                        empleado.nombre = eRemote.nombre;
                        empleado.a_paterno = eRemote.a_paterno;
                        empleado.a_materno = eRemote.a_materno;
                        empleado.user_name = eRemote.user_name;
                        empleado.fecha_registro = eRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Empleados");
            }
        }

        private void UnidadesMedida()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.unidad_medida.FirstOrDefault() != null) ? dcLocal.unidad_medida.Max((SuPlazaPOS35.domain.unidad_medida e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.unidad_medida> list = dcServer.unidad_medida.Where((SuPlazaPOS35.DomainServer.unidad_medida e) => e.fecha_registro > lastChangeUpdate).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.unidad_medida eRemote in list)
                {
                    SuPlazaPOS35.domain.unidad_medida unidad_medida = dcLocal.unidad_medida.FirstOrDefault((SuPlazaPOS35.domain.unidad_medida e) => e.id_unidad.Equals(eRemote.id_unidad));
                    if (unidad_medida == null)
                    {
                        unidad_medida = new SuPlazaPOS35.domain.unidad_medida();
                        unidad_medida.id_unidad = eRemote.id_unidad;
                        unidad_medida.descripcion = eRemote.descripcion;
                        unidad_medida.fecha_registro = eRemote.fecha_registro;
                        unidad_medida.cve_sat = eRemote.cve_sat;
                        dcLocal.unidad_medida.InsertOnSubmit(unidad_medida);
                    }
                    else
                    {
                        unidad_medida.descripcion = eRemote.descripcion;
                        unidad_medida.fecha_registro = eRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Medidas");
            }
        }

        private void Articulos()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.articulo.FirstOrDefault() != null) ? dcLocal.articulo.Max((SuPlazaPOS35.domain.articulo e) => e.fecha_registro) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.articulo> list = (from e in dcServer.articulo
                                                                 where e.fecha_registro > lastChangeUpdate
                                                                 orderby e.fecha_registro
                                                                 select e).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.articulo eRemote in list)
                {
                    SuPlazaPOS35.domain.articulo articulo = dcLocal.articulo.FirstOrDefault((SuPlazaPOS35.domain.articulo e) => e.cod_barras.Equals(eRemote.cod_barras));
                    if (articulo == null)
                    {
                        articulo = new SuPlazaPOS35.domain.articulo();
                        articulo.cod_barras = eRemote.cod_barras;
                        articulo.cod_asociado = eRemote.cod_asociado;
                        articulo.id_clasificacion = eRemote.id_clasificacion;
                        articulo.cod_interno = eRemote.cod_interno;
                        articulo.descripcion = eRemote.descripcion;
                        articulo.descripcion_corta = eRemote.descripcion_corta;
                        articulo.cantidad_um = eRemote.cantidad_um;
                        articulo.id_unidad = eRemote.id_unidad;
                        articulo.id_proveedor = eRemote.id_proveedor;
                        articulo.precio_compra = eRemote.precio_compra;
                        articulo.utilidad = eRemote.utilidad;
                        articulo.precio_venta = eRemote.precio_venta;
                        articulo.tipo_articulo = eRemote.tipo_articulo;
                        articulo.stock = eRemote.stock;
                        articulo.stock_min = eRemote.stock_min;
                        articulo.stock_max = eRemote.stock_max;
                        /***********************************************************correcion*/
                        articulo.impuestos[0].iva = eRemote.iva;
                        /***********************************************************correcion*/
                        
                        articulo.kit_fecha_ini = eRemote.kit_fecha_ini;
                        articulo.kit_fecha_fin = eRemote.kit_fecha_fin;
                        articulo.articulo_disponible = eRemote.articulo_disponible;
                        articulo.kit = eRemote.kit;
                        articulo.visible = eRemote.visible;
                        articulo.puntos = eRemote.puntos;
                        articulo.last_update_inventory = eRemote.last_update_inventory;
                        articulo.fecha_registro = eRemote.fecha_registro;
                        articulo.cve_producto = eRemote.cve_producto;
                        dcLocal.articulo.InsertOnSubmit(articulo);
                    }
                    else
                    {
                        articulo.cod_asociado = eRemote.cod_asociado;
                        articulo.id_clasificacion = eRemote.id_clasificacion;
                        articulo.cod_interno = eRemote.cod_interno;
                        articulo.descripcion = eRemote.descripcion;
                        articulo.descripcion_corta = eRemote.descripcion_corta;
                        articulo.cantidad_um = eRemote.cantidad_um;
                        articulo.id_unidad = eRemote.id_unidad;
                        articulo.id_proveedor = eRemote.id_proveedor;
                        articulo.precio_compra = eRemote.precio_compra;
                        articulo.utilidad = eRemote.utilidad;
                        articulo.precio_venta = eRemote.precio_venta;
                        articulo.tipo_articulo = eRemote.tipo_articulo;
                        articulo.stock = eRemote.stock;
                        articulo.stock_min = eRemote.stock_min;
                        articulo.stock_max = eRemote.stock_max;
                        /***************************************************************************************coreecion*/
                        articulo.impuestos[0].iva = eRemote.iva;
                        /****************************************************************************************************/
                        articulo.kit_fecha_ini = eRemote.kit_fecha_ini;
                        articulo.kit_fecha_fin = eRemote.kit_fecha_fin;
                        articulo.articulo_disponible = eRemote.articulo_disponible;
                        articulo.kit = eRemote.kit;
                        articulo.visible = eRemote.visible;
                        articulo.puntos = eRemote.puntos;
                        articulo.last_update_inventory = eRemote.last_update_inventory;
                        articulo.fecha_registro = eRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                    Thread.Sleep(TimeThread);
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message  +" "+ "ERROR Here!"+ex.StackTrace);
                throw new Exception("Error en Articulos dsi_code");
            }
        }

        private void Ofertas()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.oferta.FirstOrDefault() != null) ? dcLocal.oferta.Max((SuPlazaPOS35.domain.oferta e) => e.fecha_oferta) : LastUpdate);
                List<SuPlazaPOS35.DomainServer.oferta> list = (from e in dcServer.oferta
                                                               where e.fecha_oferta > lastChangeUpdate && e.status_oferta.Equals("disponible")
                                                               orderby e.fecha_oferta
                                                               select e).ToList();
                if (list != null)
                {
                    foreach (SuPlazaPOS35.DomainServer.oferta eRemote in list)
                    {
                        SuPlazaPOS35.domain.oferta oferta = dcLocal.oferta.FirstOrDefault((SuPlazaPOS35.domain.oferta e) => e.id_oferta.Equals(eRemote.id_oferta));
                        if (oferta == null)
                        {
                            oferta = new SuPlazaPOS35.domain.oferta();
                            oferta.id_oferta = eRemote.id_oferta;
                            oferta.num_oferta = eRemote.num_oferta;
                            oferta.descripcion = eRemote.descripcion;
                            oferta.fecha_oferta = eRemote.fecha_oferta;
                            oferta.fecha_ini = eRemote.fecha_ini;
                            oferta.fecha_fin = eRemote.fecha_fin;
                            oferta.status_oferta = eRemote.status_oferta;
                            oferta.fecha_cancelacion = eRemote.fecha_cancelacion;
                            oferta.user_name = eRemote.user_name;
                            dcLocal.oferta.InsertOnSubmit(oferta);
                            dcLocal.SubmitChanges();
                            List<SuPlazaPOS35.DomainServer.oferta_articulo> list2 = dcServer.oferta_articulo.Where((SuPlazaPOS35.DomainServer.oferta_articulo e) => e.id_oferta.Equals(eRemote.id_oferta)).ToList();
                            if (list2 == null)
                            {
                                continue;
                            }
                            foreach (SuPlazaPOS35.DomainServer.oferta_articulo item in list2)
                            {
                                SuPlazaPOS35.domain.oferta_articulo oferta_articulo = new SuPlazaPOS35.domain.oferta_articulo();
                                oferta_articulo.id_oferta = item.id_oferta;
                                oferta_articulo.cod_barras = item.cod_barras;
                                oferta_articulo.precio_oferta = item.precio_oferta;
                                oferta_articulo.status_oferta = item.status_oferta;
                                oferta_articulo.fecha_cancelacion = item.fecha_cancelacion;
                                oferta_articulo.fecha_registro = item.fecha_registro;
                                dcLocal.oferta_articulo.InsertOnSubmit(oferta_articulo);
                                dcLocal.SubmitChanges();
                            }
                        }
                        else
                        {
                            oferta.num_oferta = eRemote.num_oferta;
                            oferta.descripcion = eRemote.descripcion;
                            oferta.fecha_oferta = eRemote.fecha_oferta;
                            oferta.fecha_ini = eRemote.fecha_ini;
                            oferta.fecha_fin = eRemote.fecha_fin;
                            oferta.status_oferta = eRemote.status_oferta;
                            oferta.fecha_cancelacion = eRemote.fecha_cancelacion;
                            oferta.user_name = eRemote.user_name;
                            dcLocal.SubmitChanges();
                        }
                    }
                }
                if (dcLocal.oferta_articulo.FirstOrDefault() == null)
                {
                    return;
                }
                lastChangeUpdate = dcLocal.oferta_articulo.Max((SuPlazaPOS35.domain.oferta_articulo e) => e.fecha_registro);
                List<SuPlazaPOS35.DomainServer.oferta_articulo> list3 = (from e in dcServer.oferta_articulo
                                                                         where e.fecha_registro > lastChangeUpdate
                                                                         orderby e.fecha_registro
                                                                         select e).ToList();
                if (list3 == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.oferta_articulo oaRemote in list3)
                {
                    SuPlazaPOS35.domain.oferta_articulo oferta_articulo2 = dcLocal.oferta_articulo.FirstOrDefault((SuPlazaPOS35.domain.oferta_articulo e) => e.id_oferta.Equals(oaRemote.id_oferta) && e.cod_barras.Equals(oaRemote.cod_barras));
                    if (oferta_articulo2 == null)
                    {
                        oferta_articulo2 = new SuPlazaPOS35.domain.oferta_articulo();
                        oferta_articulo2.id_oferta = oaRemote.id_oferta;
                        oferta_articulo2.cod_barras = oaRemote.cod_barras;
                        oferta_articulo2.precio_oferta = oaRemote.precio_oferta;
                        oferta_articulo2.status_oferta = oaRemote.status_oferta;
                        oferta_articulo2.fecha_cancelacion = oaRemote.fecha_cancelacion;
                        oferta_articulo2.fecha_registro = oaRemote.fecha_registro;
                        dcLocal.oferta_articulo.InsertOnSubmit(oferta_articulo2);
                    }
                    else
                    {
                        oferta_articulo2.precio_oferta = oaRemote.precio_oferta;
                        oferta_articulo2.status_oferta = oaRemote.status_oferta;
                        oferta_articulo2.fecha_cancelacion = oaRemote.fecha_cancelacion;
                        oferta_articulo2.fecha_registro = oaRemote.fecha_registro;
                    }
                    dcLocal.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Ofertas");
            }
        }

        private void Facturas()
        {
            try
            {
                DateTime lastChangeUpdate = ((dcLocal.factura_venta.FirstOrDefault() != null) ? dcLocal.factura_venta.Max((SuPlazaPOS35.domain.factura_venta e) => e.fecha_registro) : LastUpdate);
                if (dcLocal.pos_settings.FirstOrDefault() == null)
                {
                    return;
                }
                int id_pos = dcLocal.pos_settings.FirstOrDefault().id_pos;
                List<SuPlazaPOS35.DomainServer.factura_venta> list = (from e in dcServer.factura_venta
                                                                      where e.fecha_registro > lastChangeUpdate && e.id_pos.Equals(id_pos)
                                                                      orderby e.fecha_registro
                                                                      select e).ToList();
                if (list == null)
                {
                    return;
                }
                foreach (SuPlazaPOS35.DomainServer.factura_venta eRemote in list)
                {
                    SuPlazaPOS35.domain.factura_venta factura_venta = dcLocal.factura_venta.FirstOrDefault((SuPlazaPOS35.domain.factura_venta e) => e.id_pos.Equals(eRemote.id_pos) && e.id_venta.Equals(eRemote.id_venta));
                    if (factura_venta == null)
                    {
                        factura_venta = new SuPlazaPOS35.domain.factura_venta();
                        factura_venta.id_pos = eRemote.id_pos;
                        factura_venta.id_venta = eRemote.id_venta;
                        factura_venta.id_factura = eRemote.id_factura;
                        factura_venta.fecha_registro = eRemote.fecha_registro;
                        dcLocal.factura_venta.InsertOnSubmit(factura_venta);
                        dcLocal.SubmitChanges();
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                CtrlException.SetError(ex.Message);
                throw new Exception("Error en Facturas");
            }
        }
    }

}
