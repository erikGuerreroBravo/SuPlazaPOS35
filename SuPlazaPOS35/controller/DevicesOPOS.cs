using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.PointOfService;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.model;
using SuPlazaPOS35.view;

using NLog;
using DsiCodeTech.Common.Util;
using System.Security;

namespace SuPlazaPOS35.controller
{
    public class DevicesOPOS
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        private enum Align
        {
            toLeft,
            toRight,
            toMiddle
        }

        public enum PrintTicket
        {
            normal,
            reimprimir,
            devolucion,
            devoluciones,
            corte_x,
            corte_z
        }

        private PosExplorer pe;

        private CashDrawer cashDrawer;

        private PosPrinter printer;

        private Scanner scanner;

        private decimal items_qty;

        private decimal subTotal;

        private decimal iva;

        private decimal ieps;

        private decimal descuento;

        private decimal impuestos;

        private decimal total;

        public static LineDisplay display;

        private string escAlignCenter = $"{'\u001b'}|cA";

        private string escScaleHor = $"{'\u001b'}|3hC";

        private string escScaleVer = $"{'\u001b'}|3vC";

        private string escAlignRight = $"{'\u001b'}|rA";

        private string escBoldOn = $"{'\u001b'}|bC";

        private string escNewLine = $"{'\u001b'}|1lF";

        private string escPaperCut = $"{'\u001b'}|7sP";

        private string escNormal = $"{'\u001b'}|N";

        private string escUnderlineOn = $"{'\u001b'}|uC";

        private string escHigherText = $"{'\u001b'}|4C";

        private string escBiggerText = $"{'\u001b'}|3C";

        private string escNormalText = $"{'\u001b'}|1C";

        private string escLogo = $"{'\u001b'}|1B";


        public DevicesOPOS()
        {
            pe = new PosExplorer();
            openDevicesOpos();
        }

        private void openDevicesOpos()
        {
            openCashDrawer(POS.caja.pos_csh_enable, POS.caja.pos_csh_name);
            openDisplay(POS.caja.pos_dsp_enable, POS.caja.pos_dsp_name);
            openPrinter(POS.caja.pos_ptr_enable, POS.caja.pos_ptr_name);
            openScanner(POS.caja.pos_scn_enable, POS.caja.pos_scn_name);
        }

        public void closeDevicesOpos()
        {
            if (getCashDrawer() != null)
            {
                getCashDrawer().Close();
            }
            if (getPosPrinter() != null)
            {
                getPosPrinter().Close();
            }
            if (getScanner() != null)
            {
                getScanner().Close();
            }
            if (display != null)
            {
                display.Close();
            }
        }

        private void openCashDrawer(bool enable, string nameDeviceType)
        {
            if (enable)
            {
                logger.Info($"Se inicializa proceso para instancia CashDrawer {nameDeviceType}");
                DeviceInfo deviceInfo = getDeviceInfo(DeviceType.CashDrawer, nameDeviceType);
                if (deviceInfo == null)
                {
                    throw new Exception(nameDeviceType + " no se pudo localizar");
                }
                cashDrawer = (CashDrawer)pe.CreateInstance(deviceInfo);
                cashDrawer.Open();
                cashDrawer.Claim(5000);
                cashDrawer.DeviceEnabled = true;
                logger.Info($"Se finaliza proceso para instancia CashDrawer {cashDrawer}");
            }
        }

        private void openDisplay(bool enable, string nameDeviceType)
        {
            if (enable)
            {
                logger.Info($"Se inicializa proceso para instancia LineDisplay {nameDeviceType}");
                DeviceInfo deviceInfo = getDeviceInfo("LineDisplay", nameDeviceType);
                if (deviceInfo == null)
                {
                    throw new Exception(nameDeviceType + " no se pudo localizar");
                }
                display = (LineDisplay)pe.CreateInstance(deviceInfo);
                display.Open();
                display.Claim(200);
                display.DeviceEnabled = true;
                logger.Info($"Se finaliza proceso para instancia LineDisplay {nameDeviceType}");
            }
        }

        private void openPrinter(bool enable, string nameDeviceType)
        {
            if (enable)
            {
                logger.Info($"Se inicializa proceso para instancia PosPrinter {nameDeviceType}");
                DeviceInfo deviceInfo = getDeviceInfo(DeviceType.PosPrinter, nameDeviceType);
                if (deviceInfo == null)
                {
                    throw new Exception(nameDeviceType + " no se pudo localizar");
                }
                printer = (PosPrinter)pe.CreateInstance(deviceInfo);
                printer.Open();
                if (!printer.Claimed)
                {
                    printer.Claim(2000);
                }
                printer.AsyncMode = false;
                printer.DeviceEnabled = true;
                printer.RecLetterQuality = true;
                string fileName = Application.StartupPath + "\\logo_SuPlaza.bmp";
                if (POS.IsLogoEnable())
                {
                    printer.SetBitmap(1, PrinterStation.Receipt, fileName, -11, -2);
                }
                logger.Info($"Se finaliza proceso para instancia PosPrinter {nameDeviceType}");
            }

        }

        private void checkPosPrinter()
        {
            if (printer == null)
            {
                throw new Exception("La impresora PosPrinter no está disponible.");
            }
            if (printer.CoverOpen)
            {
                throw new Exception("Cierre la tapa de la impresora");
            }
        }

        public void printInitalMessage()
        {
            checkPosPrinter();
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(escLogo + "\r\n");
            if (printer.CapRecBold)
            {
                stringBuilder.Append(escBoldOn);
            }
            stringBuilder.Append(setTicketHeader());
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Bienvenido: " + new EmpleadoDAO().getEmployeeByUserName(POS.user.user_name).fullName(), Align.toMiddle));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            finallyPosPrint(stringBuilder.ToString());
            stringBuilder = null;
            GC.Collect();
        }

        //public void printTicketSaleOnPosPrinter(long folio)
        //{
        //    printTicketOnPosPrinter(new VentaDAO().getSaleOutByFolio(folio), PrintTicket.normal);
        //}
        public void printTicketSaleOnPosPrinter(venta v)
        {
            printTicketOnPosPrinter(v, PrintTicket.normal);
        }

        public void printLastOneTicketOnPosPrinter()
        {
            venta saleOutLast = new VentaDAO().getSaleOutLast();
            if (saleOutLast == null)
            {
                throw new Exception("No hay ventas registradas");
            }
            printTicketOnPosPrinter(saleOutLast, PrintTicket.reimprimir);
            saleOutLast = null;
            GC.Collect();
        }

        public void printLastTicketOnPosPrinter(long folio)
        {
            venta saleOutByFolio = new VentaDAO().getSaleOutByFolio(folio);
            if (saleOutByFolio == null)
            {
                throw new Exception("La venta indicada no existe");
            }
            printTicketOnPosPrinter(saleOutByFolio, PrintTicket.reimprimir);
            GC.Collect();
        }

        private void printTicketOnPosPrinter(venta sale_out, PrintTicket modePrint)
        {
            checkPosPrinter();
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(escLogo + "\r\n");
            if(printer.CapRecBold)
            {
                stringBuilder.Append(escBoldOn);
            }
            stringBuilder.Append(setTicketHeader());
            if (modePrint.CompareTo(PrintTicket.normal) != 0)
            {
                stringBuilder.Append(setSubTitle(modePrint));
            }
            stringBuilder.Append(setLineAlignLn("Caja No: " + sale_out.id_pos, Align.toLeft));
            stringBuilder.Append(setLineAlignLn(string.Concat(sale_out.fecha_venta, "  Ticket # ", sale_out.folio), Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Cantidad Uni Descripcion", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Código                P.Unit      Importe", Align.toLeft));
            stringBuilder.Append(setDotLine());

            stringBuilder.Append(setTicketDetail(POS.IsConcentredTicket()
                ? new VentaArticuloDAO().getSaleOutGroupDetail(sale_out.id_venta)
                : new VentaArticuloDAO().getSaleOutDetail(sale_out.id_venta)));

            var totales = new VentaDAO().GetTotales(sale_out.id_venta);
            total = totales.total;

            stringBuilder.Append(setTotales(totales.sub_total, totales.descuento, totales.impuestos));
            stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
            stringBuilder.Append("   Total:  " + totales.total.ToString("C2"));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));

            stringBuilder.Append(setDetailsSale(sale_out, totales.total_articulos));

            stringBuilder.Append(setTicketFooter());

            logger.Info(stringBuilder.ToString());
            finallyPosPrint(stringBuilder.ToString());
        }

        public void printTicketDevolutionOnPosPrinter(Guid id_devolucion)
        {
            checkPosPrinter();
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(escLogo + "\r\n");
            if (printer.CapRecBold)
            {
                stringBuilder.Append(escBoldOn);
            }
            venta_devolucion saleOutDevolution = new VentaDevolucionDAO().getSaleOutDevolution(id_devolucion);
            stringBuilder.Append(setTicketHeader());
            stringBuilder.Append(setSubTitle(PrintTicket.devolucion));
            stringBuilder.Append(setLineAlignLn("Caja No: " + saleOutDevolution.id_pos, Align.toLeft));
            stringBuilder.Append(setLineAlignLn(string.Concat(saleOutDevolution.fecha_dev, "  Devolucion # ", saleOutDevolution.folio), Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Cantidad Uni Descripción", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Código                P.Unit      Importe", Align.toLeft));
            stringBuilder.Append(setDotLine());
            items_qty = (subTotal = (iva = (descuento = (total = 0.0m))));
            List<domain.venta_articulo> saleOutDevolutionDetail = new VentaDevolucionDAO().getSaleOutDevolutionDetail(id_devolucion);
            stringBuilder.Append(setTicketDevolutionDetail(saleOutDevolutionDetail));


            new SuPlazaPosUtil().CuadrarTotales(ref subTotal, saleOutDevolution.cant_dev, DsiCodeUtil.Sum(subTotal, saleOutDevolution.impuestos));

            stringBuilder.Append(setTotales(subTotal, saleOutDevolution.descuento, saleOutDevolution.impuestos));

            stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
            stringBuilder.Append(string.Concat("   Total:  (" + DsiCodeUtil.CurrencyFormat(POS.totalVenta), ")"));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            stringBuilder.Append(setDetailsDevolution(items_qty));
            stringBuilder.Append(setTicketFooter());

            logger.Info(stringBuilder.ToString());
            finallyPosPrint(stringBuilder.ToString());
            GC.Collect();
        }

        #region Impresion del Corte de la venta
        public void imprimirCorte(PrintTicket modePrint, corte cut)
        {
            int no_transacciones = cut.no_transacciones;

            decimal efectivo = cut.efectivo;
            decimal vales = cut.pago_vales;
            //decimal vales = cut.vales;

            decimal cheques = cut.cheques;
            decimal tc = cut.pago_tc;
            //decimal tc = cut.tc;
            decimal spei = cut.pago_spei;
            decimal td = cut.pago_td;
            decimal total_vendido = cut.total_vendido;
            decimal total_devuelto = cut.total_devuelto;
            decimal num = total_vendido;// - total_devuelto;
            decimal total_desglosado_iva = cut.total_desglosado_iva;
            decimal total_iva = cut.iva;
            decimal total_desglosado_ieps = cut.total_desglosado_ieps;
            decimal total_ieps = cut.ieps;

            decimal num2 = cut.iva;
            decimal ieps = cut.ieps;
            decimal total_exentos = cut.total_exentos;

            StringBuilder stringBuilder = new StringBuilder("");
            if(printer.CapRecBold)
            {
                stringBuilder.Append(escBoldOn);
            }
            stringBuilder.Append(setTicketHeader());
            stringBuilder.Append(setSubTitle(modePrint));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Fecha Inicial  : " + cut.fecha_ini, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Fecha Final    : " + cut.fecha_fin, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Folio Inicial  : " + cut.folio_ini, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Folio Final    : " + cut.folio_fin, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Caja           : " + POS.caja.id_pos, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Cajero         : " + new EmpleadoDAO().getEmployeeByUserName(POS.user.user_name).shortName(), Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Fecha impresion: " + DateTime.Now, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setDotLine());
            string text = "Efectivo: $";
            string text2 = "Vales   : $";
            string text3 = "T.D.    : $";
            string text4 = "T.C.    : $";
            string txt_pago_spei = "Spei      :$";
            string text5 = "# Transacciones:";
            string text6 = "Total de Venta:";

            string text7 = "IVA 16%:";
            string text8 = "Impuesto 16%:";

            string textIeps = "IEPS:";
            string textImpuestoIeps = "Impuesto IEPS%:";

            string exento = "Exento%:";

            string efectivoFormat = DsiCodeUtil.CurrencyFormat(efectivo);
            string valesFormat = DsiCodeUtil.CurrencyFormat(vales);
            string debitoFormat = DsiCodeUtil.CurrencyFormat(td);
            string creditoFormat = DsiCodeUtil.CurrencyFormat(tc);
            string speiFormat = DsiCodeUtil.CurrencyFormat(spei);
            string ventaFormat = DsiCodeUtil.CurrencyFormat(total_vendido + total_devuelto);

            stringBuilder.Append(text + new string(' ', 29 - efectivoFormat.Length) + efectivoFormat + "\r\n");
            stringBuilder.Append(text2 + new string(' ', 29 - valesFormat.Length) + valesFormat + "\r\n");
            stringBuilder.Append(text3 + new string(' ', 29 - debitoFormat.Length) + debitoFormat + "\r\n");
            stringBuilder.Append(text4 + new string(' ', 29 - creditoFormat.Length) + creditoFormat + "\r\n");
            stringBuilder.Append(txt_pago_spei + new string(' ', 29 - speiFormat.Length) + speiFormat + "\r\n");
            stringBuilder.Append(text5 + new string(' ', 24 - no_transacciones.ToString().Length) + no_transacciones + "\r\n");
            stringBuilder.Append(setDotLine());
            stringBuilder.Append(text6 + new string(' ', 29 - total_vendido.ToString().Length) + ventaFormat + "\r\n");
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));


            stringBuilder.Append(text7 + new string(' ', 30 - efectivoFormat.Length) + DsiCodeUtil.CurrencyFormat(total_desglosado_iva) + "\r\n");
            stringBuilder.Append(text8 + new string(' ', 25 - efectivoFormat.Length) + DsiCodeUtil.CurrencyFormat(total_iva) + "\r\n");
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));

            //TODO - quitar comentarios para que aparezca el IEPS
            /*stringBuilder.Append(textIeps + new string(' ', 33 - efectivoFormat.Length) + DsiCodeUtil.CurrencyFormat(total_desglosado_ieps) + "\r\n");
            stringBuilder.Append(textImpuestoIeps + new string(' ', 23 - efectivoFormat.Length) + DsiCodeUtil.CurrencyFormat(total_ieps) + "\r\n");
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));*/

            stringBuilder.Append(exento + new string(' ', 30 - efectivoFormat.Length) + DsiCodeUtil.CurrencyFormat(total_exentos) + "\r\n");


            stringBuilder.Append(setDotLine());
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
            stringBuilder.Append("   Total:  " + ventaFormat); 
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            List<venta_devolucion> listSaleOutDevolution = new VentaDevolucionDAO().getListSaleOutDevolution(cut.fecha_ini, cut.fecha_fin);
            if (listSaleOutDevolution != null)
            {
                stringBuilder.Append(setDotLine());
                stringBuilder.Append(setSubTitle(PrintTicket.devoluciones));
                foreach (venta_devolucion item in listSaleOutDevolution)
                {
                    stringBuilder.Append(new string(' ', 5 - item.folio.ToString().Length) + item.folio + " | ");
                    stringBuilder.Append(string.Concat(item.fecha_dev, " | "));
                    stringBuilder.Append(new string(' ', 9 - item.cant_dev.ToString().Length) + "-" + DsiCodeUtil.CurrencyFormat(item.cant_dev) + "\r\n");
                }
            }
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("Supervisor: " + new EmpleadoDAO().getEmployeeByUserName(POS.supervisor.user_name).fullName(), Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            logger.Info(stringBuilder.ToString());
            finallyPosPrint(stringBuilder.ToString());
            GC.Collect();
        }

        #endregion
        private string setTicketHeader()
        {
            empresa empresa = POS.getEmpresa();
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(setLineAlignLn(empresa.razon_social, Align.toMiddle));
            stringBuilder.Append(setLineAlignLn(empresa.calle, Align.toMiddle));
            stringBuilder.Append(setLineAlignLn("COLONIA " + empresa.colonia + "," + empresa.municipio + "," + empresa.estado + " CP: " + empresa.codigo_postal, Align.toMiddle));
            stringBuilder.Append(setLineAlignLn(empresa.rfc, Align.toMiddle));
            GC.Collect();
            return stringBuilder.ToString();
        }

        #region Impresion de Detalle del Ticket
        private string setTicketDetail(List<domain.venta_articulo> items)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            foreach (domain.venta_articulo item in items)
            {
                items_qty += (decimal)((item.articulo.unidad_medida.descripcion.CompareTo("Kg") == 0) ? ((item.cant_vta() > 0m) ? 1 : 0) : ((item.articulo.unidad_medida.descripcion.CompareTo("Gms") == 0) ? ((item.cant_vta() > 0m) ? 1 : 0) : ((int)item.cant_vta())));
                subTotal += item.subTotal();
                descuento += item.descuento();
                total += item.total();
                iva += item.getIVA();
                stringBuilder.Append(setLineItem(item));
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region Impresion de Devolucion del Ticket y Detalle
        private string setTicketDevolutionDetail(List<domain.venta_articulo> items)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            foreach (domain.venta_articulo item in items)
            {
                items_qty += (decimal)((item.articulo.unidad_medida.descripcion.CompareTo("Kg") == 0) ? ((item.cantidad > 0m) ? 1 : 0) : ((item.articulo.unidad_medida.descripcion.CompareTo("Gms") == 0) ? ((item.cantidad > 0m) ? 1 : 0) : ((int)item.cant_devuelta)));
                subTotal += item.subTotalDevolucion();
                descuento += item.descuentoDevolucion();
                total += item.totalDevolucion();
                iva += item.getIVADevolucion();
                stringBuilder.Append(setLineItem(item));
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region Footer del ticket
        private string setTicketFooter()
        {
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(setDotLine());
            stringBuilder.Append(setLineAlignLn("Le Atendio: " + new EmpleadoDAO().getEmployeeByUserName(POS.user.user_name).fullName(), Align.toMiddle));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("!Gracias Por Su Compra!", Align.toMiddle));
            stringBuilder.Append(setLineAlignLn("Comprobante No Fiscal", Align.toMiddle));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn("", Align.toLeft));
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            return stringBuilder.ToString();
        }
        #endregion

        #region Alineacion de lineas
        private string setLineAlignLn(string str, Align align)
        {
            str = str.Trim();
            return align switch
            {
                Align.toLeft => str + "\r\n",
                Align.toRight => new string(' ', 44 - str.Length) + str + "\r\n",
                Align.toMiddle => new string(' ', (44 - str.Length) / 2) + str + "\r\n",
                _ => "",
            };
        }
        #endregion

        #region Linea Punteada
        private string setDotLine()
        {
            return new string('-', 44) + "\r\n";
        }
        #endregion

        #region Sutitulo de el tiket
        private string setSubTitle(PrintTicket mode)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            switch (mode)
            {
                case PrintTicket.reimprimir:
                    stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
                    stringBuilder.Append(setLineAlignLn("REIMPRESION DE TICKET", Align.toLeft));
                    break;
                case PrintTicket.devoluciones:
                    stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
                    stringBuilder.Append(setLineAlignLn("*** DEVOLUCIONES ***", Align.toLeft));
                    break;
                case PrintTicket.devolucion:
                    stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
                    stringBuilder.Append(setLineAlignLn("*** DEVOLUCION ***", Align.toLeft));
                    break;
                case PrintTicket.corte_x:
                    stringBuilder.Append(setLineAlignLn("CORTE X", Align.toLeft));
                    break;
                case PrintTicket.corte_z:
                    stringBuilder.Append(setLineAlignLn(escHigherText, Align.toLeft));
                    stringBuilder.Append(setLineAlignLn("CORTE Z", Align.toLeft));
                    break;
            }
            stringBuilder.Append(setLineAlignLn(escNormalText, Align.toLeft));
            return stringBuilder.ToString();
        }
        #endregion
        private string setLineItem(domain.venta_articulo va)

        {
            StringBuilder stringBuilder = new StringBuilder("");
            decimal num = ((SuPlazaPOS.statusModeOperation.CompareTo(SuPlazaPOS.modeOperation.Devolution) == 0) ? va.cant_devuelta : va.cant_vta());
            string numFormat = DsiCodeUtil.CurrencyFormat(num);
            string precio_ventaFormat = DsiCodeUtil.CurrencyFormat(va.articulo.precio_venta);

            stringBuilder.Append(new string(' ', 8 - numFormat.Length) + numFormat);

            //stringBuilder.Append(new string(' ', 8 - num.ToString("F3").Length) + num.ToString("F3"));
            stringBuilder.Append(string.Concat(" ", va.articulo.unidad_medida.descripcion + new string(' ', 4 - va.articulo.unidad_medida.descripcion.Length)));
            stringBuilder.Append(va.articulo.descripcion_corta);
            stringBuilder.Append("\r\n");
            stringBuilder.Append(va.articulo.cod_barras + new string(' ', 15 - va.articulo.cod_barras.Length));
            stringBuilder.Append(new string(' ', 13 - precio_ventaFormat.Length) + precio_ventaFormat);
            //stringBuilder.Append(new string(' ', 13 - va.articulo.precio_venta.ToString("F2").Length) + va.articulo.precio_venta.ToString("F2"));
            decimal num2 = ((SuPlazaPOS.statusModeOperation.CompareTo(SuPlazaPOS.modeOperation.Devolution) == 0) ? va.totalDevolucion() : va.precio_vta * va.cant_vta());
            string num2Format = DsiCodeUtil.CurrencyFormat(num2);
            stringBuilder.Append(new string(' ', 13 - num2Format.Length) + num2Format);
            stringBuilder.Append("\r\n");
            return stringBuilder.ToString();
        }

        #region Establecimiento de los Totales de la Venta
        private string setTotales(decimal subtotal, decimal descuento, decimal impuestos)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            string text = "SubTotal:";
            string text2 = "Descto:";
            string text3 = "Impuestos:";

            string subTotalFormat = DsiCodeUtil.CurrencyFormat(subtotal);
            string descuentoFormat = DsiCodeUtil.CurrencyFormat(descuento);
            string impuestosFormat = DsiCodeUtil.CurrencyFormat(impuestos);

            stringBuilder.Append(setDotLine());
            stringBuilder.Append(new string(' ', 28 - text.Length) + text);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - subTotalFormat.Length) + subTotalFormat));
            stringBuilder.Append("\r\n");

            stringBuilder.Append(new string(' ', 28 - text2.Length) + text2);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - descuentoFormat.Length) + descuentoFormat));
            stringBuilder.Append("\r\n");

            stringBuilder.Append(new string(' ', 28 - text3.Length) + text3);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - impuestosFormat.Length) + impuestosFormat));
            stringBuilder.Append("\r\n");
            return stringBuilder.ToString();
        }
        #endregion

        #region Detalles de la Venta

        private string setDetailsSale(venta vta, decimal qty_items)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            string text = "Total de artículos:";
            string text2 = "Efectivo:";
            string text3 = "Pago con T.D.:";
            string text4 = "Pago con vales:";
            string text5 = "Pago con T.C.:";
            string text6 = "Su cambio:";
            string text7 = "SPEI:";

            decimal num = 0m;
            stringBuilder.Append(setDotLine());
            stringBuilder.Append(new string(' ', 28 - text.Length) + text);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - qty_items.ToString("F0").Length) + qty_items.ToString("F0")));
            stringBuilder.Append("\r\n");
            string pago_efectivoFormat = DsiCodeUtil.CurrencyFormat(vta.pago_efectivo);
            
            if (vta.pago_efectivo > 0m)
            {
                stringBuilder.Append(new string(' ', 28 - text2.Length) + text2);
                stringBuilder.Append(string.Concat(" ", new string(' ', 12 - pago_efectivoFormat.Length) + pago_efectivoFormat));
                //stringBuilder.Append(string.Concat(" ", new string(' ', 12 - vta.pago_efectivo.ToString("F2").Length) + vta.pago_efectivo.ToString("F2")));
                stringBuilder.Append("\r\n");
                num += (decimal)vta.pago_efectivo;
            }
            //if (vta.pago_cheque > 0m)
            //{
            //    stringBuilder.Append(new string(' ', 28 - text3.Length) + text3);
            //    stringBuilder.Append(string.Concat(" ", new string(' ', 12 - vta.pago_cheque.ToString("F2").Length) + vta.pago_cheque.ToString("F2")));
            //    stringBuilder.Append("\r\n");
            //    num += vta.pago_cheque;
            //}
            if (vta.pago_vales > 0m)
            {
                string pago_vales_Format = DsiCodeUtil.CurrencyFormat(vta.pago_vales);
                stringBuilder.Append(new string(' ', 28 - text4.Length) + text4);
                stringBuilder.Append(string.Concat(" ", new string(' ', 12 - pago_vales_Format.Length) + pago_vales_Format));
                //stringBuilder.Append(string.Concat(" ", new string(' ', 12 - vta.pago_vales.ToString("F2").Length) + vta.pago_vales.ToString("F2")));
                stringBuilder.Append("\r\n");
                num += (decimal)vta.pago_vales;
            }
            if (vta.pago_tc > 0m)
            {
                string pago_tc_Format = DsiCodeUtil.CurrencyFormat(vta.pago_tc);
                stringBuilder.Append(new string(' ', 28 - text5.Length) + text5);
                stringBuilder.Append(string.Concat(" ", new string(' ', 12 - pago_tc_Format.Length) + pago_tc_Format));
                //stringBuilder.Append(string.Concat(" ", new string(' ', 12 - vta.pago_tc.ToString("F2").Length) + vta.pago_tc.ToString("F2")));
                stringBuilder.Append("\r\n");
                num += (decimal)vta.pago_tc;
            }
            if (vta.pago_td > 0m)
            {
                string pago_td_Format = DsiCodeUtil.CurrencyFormat(vta.pago_td);
                stringBuilder.Append(new string(' ', 28 - text3.Length) + text3);
                stringBuilder.Append(string.Concat(" ", new string(' ', 12 - pago_td_Format.Length) + pago_td_Format));
                stringBuilder.Append("\r\n");
                num += (decimal)vta.pago_td;
            }
            if (vta.pago_spei > 0m)
            {
                string pago_spei_Format = DsiCodeUtil.CurrencyFormat(vta.pago_spei);
                stringBuilder.Append(new string(' ', 28 - text7.Length) + text7);
                stringBuilder.Append(string.Concat(" ", new string(' ', 12 - pago_spei_Format.Length) + pago_spei_Format));
                stringBuilder.Append("\r\n");
                num += (decimal)vta.pago_spei;
            }
            num -= total;
            string num_Format = DsiCodeUtil.CurrencyFormat(num);
            stringBuilder.Append(new string(' ', 28 - text6.Length) + text6);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - num_Format.Length) + num_Format));
            //stringBuilder.Append(string.Concat(" ", new string(' ', 12 - num.ToString("F2").Length) + num.ToString("F2")));
            stringBuilder.Append("\r\n");
            return stringBuilder.ToString();
        }

        #endregion

        #region Detalles de la Devolucion

        private string setDetailsDevolution(decimal qty_items)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            string text = "Total de articulos:";
            string text2 = "Efectivo:";
            string text3 = "Su cambio:";
            stringBuilder.Append(setDotLine());
            stringBuilder.Append(new string(' ', 28 - text.Length) + text);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - qty_items.ToString("F0").Length) + qty_items.ToString("F0")));
            stringBuilder.Append("\r\n");
            stringBuilder.Append(new string(' ', 28 - text2.Length) + text2);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - total.ToString("F2").Length) + total.ToString("F2")));
            stringBuilder.Append("\r\n");
            stringBuilder.Append(new string(' ', 28 - text3.Length) + text3);
            stringBuilder.Append(string.Concat(" ", new string(' ', 12 - total.ToString("F2").Length) + total.ToString("F2")));
            stringBuilder.Append("\r\n");
            return stringBuilder.ToString();
        }
        #endregion

        #region POS Printer
        private void finallyPosPrint(string printString)
        {
            try
            {
                printer.ClearOutput();
                printer.PrintNormal(PrinterStation.Receipt, printString);
            }
            catch
            {
                while (printer.CoverOpen)
                {
                    MessageBox.Show("Favor de cambiar el papel de la impresora.", "Impresora sin Papel", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                printer.PrintNormal(PrinterStation.Receipt, printString);
            }
            finally
            {
                printer.CutPaper(90);
                printer.ClearOutput();
                GC.Collect();
            }
        }
        #endregion
        private void openScanner(bool enable, string nameDeviceType)
        {
            if (enable)
            {
                logger.Info($"Se inicializa proceso para instancia Scanner {nameDeviceType}");
                DeviceInfo deviceInfo = getDeviceInfo("Scanner", nameDeviceType);
                if (deviceInfo == null)
                {
                    throw new Exception(nameDeviceType + " no se pudo localizar");
                }
                scanner = (Scanner)pe.CreateInstance(deviceInfo);
                scanner.Open();
                scanner.Claim(1000);
                scanner.DeviceEnabled = true;
                scanner.DecodeData = true;
                scanner.DataEventEnabled = true;
                logger.Info($"Se finaliza proceso para instancia Scanner {nameDeviceType}");
            }
        }

        public void openNowCashDrawer()
        {
            try
            {
                if (cashDrawer != null)
                {
                    cashDrawer.OpenDrawer();
                }
            }
            catch (PosControlException ex)
            {
                logger.Error($"Se produjo la siguiente excepción: {ex}");

            }
        }

        public Scanner getScanner()
        {
            return scanner;
        }

        public CashDrawer getCashDrawer()
        {
            return cashDrawer;
        }

        public PosPrinter getPosPrinter()
        {
            return printer;
        }

        private DeviceInfo getDeviceInfo(string deviceType, string nameDeviceType)
        {
            DeviceInfo result = null;
            DeviceCollection devices = pe.GetDevices(deviceType);
            foreach (DeviceInfo item in devices)
            {
                if (item.ServiceObjectName == nameDeviceType)
                {
                    return item;
                }
            }
            return result;
        }

        public bool isOpenCashDrawer()
        {
            if (cashDrawer == null)
            {
                return false;
            }
            return cashDrawer.DrawerOpened;
        }

        public static string[] getDisplaysOPOS()
        {
            return getDevicesInfoList("LineDisplay").Cast<string>().ToArray();
        }

        public static string[] getPrintersOPOS()
        {
            return getDevicesInfoList("PosPrinter").Cast<string>().ToArray();
        }

        public static string[] getCashDrawerOPOS()
        {
            return getDevicesInfoList("CashDrawer").Cast<string>().ToArray();
        }

        public static string[] getScannerOPOS()
        {
            return getDevicesInfoList("Scanner").Cast<string>().ToArray();
        }



        private static List<string> getDevicesInfoList(string deviceType)
        {
            List<string> list = new List<string>();
            DeviceCollection devices = new PosExplorer().GetDevices(deviceType);
            foreach (DeviceInfo item in devices)
            {
                list.Add(item.ServiceObjectName.ToString());
            }
            return list;
        }

        public static void showMessageDisplay(string msgLine1, string msgLine2)
        {
            if (POS.caja.pos_dsp_enable)
            {
                display.ClearText();
                display.DisplayTextAt(0, 0, msgLine1);
                display.DisplayTextAt(1, 0, msgLine2);
            }
        }
    }
}
