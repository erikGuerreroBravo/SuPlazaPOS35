using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.Constant
{
    public static class DsiCodeConst
    {
        public static readonly string RESULT_WITHOUT_DATA_ID = "POS-SYSADM-001";
        public static readonly string RESULT_WITHOUT_DATA = "No se encontraron resultados para esta consulta realizada.";

        public static readonly string RESULT_WITHIN_DATA_ID = "POS-SYSADM-002";
        public static readonly string RESULT_WITHIN_DATA = "Se encontraron resultados para esta consulta realizada.";

        public static readonly string RESULT_SUCCESS_OPERATION_ID = "POS-SYSADM-003";
        public static readonly string RESULT_SUCCESS_OPERATION = "La operación se ejecuto de forma satisfactoria.";

        public static readonly string RESULT_WITHEXCPETION_ID = "POS-SYSADM-100";
        public static readonly string RESULT_WITHEXCPETION = "Ocurrió un error inesperado dentro del sistema, intente de nuevo y si el error persiste, contante al administrador.";

        public static readonly string RESULT_RABBITMQ_ERROR_ID = "POS-SYSADM-101";
        public static readonly string RESULT_RABBITMQ_ERROR_MESSAGE = "No fue posible entregar el mensaje a rabbit mq, contacte al administrador.";

        public static readonly string RESULT_ERROR_REGISTER_PRINCIPAL_ID = "POS-SYSADM-102";
        public static readonly string RESULT_ERROR_REGISTER_PRINCIPAL = "El artículo principal que está intentando registrar ya existe, busque la relación del código o intente con otro.";

        public static readonly string RESULT_ERROR_REGISTER_ANEXO_ID = "POS-SYSADM-103";
        public static readonly string RESULT_ERROR_REGISTER_ANEXO = "El artículo anexo que está intentando registrar ya existe, busque la relación del código o intente con otro.";

        public static readonly string RESULT_ERROR_REGISTER_ASOCIADO_ID = "POS-SYSADM-104";
        public static readonly string RESULT_ERROR_REGISTER_ASOCIADO = "El artículo asociado que está intentando registrar ya existe, busque la relación del código o intente con otro.";

        public static readonly string RESULT_ERROR_POS_SETTINGS_ID = "POS-SYSADM-105";
        public static readonly string RESULT_ERROR_POS_SETTINGS = "No se encontro un código establecido en settings, por favor informe al administrador del sistema.";

        public static readonly string RESULT_ERROR_SALE_CANCEL_ID = "POS-SYSADM-106";
        public static readonly string RESULT_ERROR_SALE_CANCEL = "El número de folio que desea facturar, se encontro como cancelado, lo sentimos.";

        public static readonly string RESULT_ERROR_INVOIVE_KO_ID = "POS-SYSADM-107";
        public static readonly string RESULT_ERROR_INVOIVE_KO = "El número de folio que desea facturar, ya se encuentra con estado facturado.";

        public static readonly string RESULT_ERROR_INVOIVE_NOT_SALES_ID = "POS-SYSADM-108";
        public static readonly string RESULT_ERROR_INVOIVE_NOT_SALES = "No hay folios que facturar.";

        public static readonly string RESULT_ERROR_INVOIVE_TOKEN_SERVICE_ID = "POS-SYSADM-109";
        public static readonly string RESULT_ERROR_INVOIVE_TOKEN_SERVICE = "No fue posible obtener el token de SW Sapien, consulte al administrador.";

        public static readonly string RESULT_ERROR_INVOIVE_INCOMPLETE_DATA_ID = "POS-SYSADM-112";
        public static readonly string RESULT_ERROR_INVOIVE_INCOMPLETE_DATA = "Uno o varios de los artículos que desdea facturar, no cumplen con los requisitos mínimos para la factura, consulte al administrador.";

        public static readonly string RESULT_ERROR_INVOIVE_SERVICE_ID = "POS-SYSADM-110";
        public static readonly string RESULT_ERROR_INVOIVE_SERVICE = "No fue posible generar la factura con SW Sapien, consulte al administrador.";

        public static readonly string RESULT_ERROR_INVOIVE_ERROR_ID = "POS-SYSADM-111";
        public static readonly string RESULT_ERROR_INVOIVE_ERROR = "Se presento un error al establecer comunucación con SW Sapien, consulte al administrador.";

        public static readonly string RESULT_ERROR_INVENTORY_ERROR_ID = "POS-SYSADM-112";
        public static readonly string RESULT_ERROR_INVENTORY_ERROR = "Hay un inventario abierto, primero finalize el en curso y comience uno nuevo.";

        public static readonly string RESULT_ERROR_OFFER_ERROR_ID = "POS-SYSADM-113";
        public static readonly string RESULT_ERROR_OFFER_ERROR = "No hay artículos que ofertar, agregue uno o varios artículos a la lista de ofertas.";

        public static readonly string RESULT_NOTFOUND_OFFER_ERROR_ID = "POS-SYSADM-114";
        public static readonly string RESULT_NOTFOUND_OFFER_ERROR = "No hay coincidencias con la busqueda de la oferta.";

        public static readonly string RESULT_EXITS_OFFER_ERROR_ID = "POS-SYSADM-115";
        public static readonly string RESULT_EXITS_OFFER_ERROR = "El artículo que desea ofertar ya existe dentro de otra oferta.";

        public static readonly string NOT_HANDLE_ERROR_MESSAGE_ID = "POS-SYSADM-200";
        public static readonly string NOT_HANDLE_ERROR_MESSAGE = "Ocurrió un error controlado por el sistema.";

        public static readonly string HANDLE_ERROR_MESSAGE_ID = "POS-SYSADM-500";
        public static readonly string HANDLE_ERROR_MESSAGE = "Ocurrió un error, contacte al administrador para validar los detalles.";

        public static readonly string RESULT_UPDATED_DATA = "La actualización se ejecutó de forma exitosa.";

        public static readonly string RESULT_REGISTER_DATA = "Se ejecutó de forma exitosa, el registro de información.";

        public static readonly string RESULT_PUBLISH_DATA = "La información fue enviada a los puntos de venta, valide con el supervidor de cajas.";

        public static readonly string RESULT_REGISTER_EXCEL_DONE = "Cambio de precios realizados satisfactoriamente.";
        public static readonly string RESULT_REGISTER_EXCEL_ERROR = "No se pudo cargo toda la información, contacte al administrador.";


        public static readonly string EXCEL_DOWNLOAD_PATH = @"C:\suPlaza\layouts\layout_cp.xlsx";
        public static readonly string EXCEL_DOWNLOAD_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static readonly string EXCEL_SAVE_PATH = @"C:\suPlaza\tmp\";

        public static readonly string EXCEL_WORKSHEET_CP = "CambioPrecios";

        public static readonly string PATTERN_DECIMAL = @"^-?[0-9]+(.[0-9]+)?$";

        public static readonly string SYSTEM_ENVIRONMENT_ACCESS_SQL_SERVER = "SYSTEM_ENVIRONMENT_ACCESS_SQL_SERVER";
        public static readonly string SYSTEM_ENVIRONMENT_ACCESS_SQL_USER = "SYSTEM_ENVIRONMENT_ACCESS_SQL_USER";
        public static readonly string SYSTEM_ENVIRONMENT_ACCESS_SQL_PASSWORD = "SYSTEM_ENVIRONMENT_ACCESS_SQL_PASSWORD";

        public static readonly string SYSTEM_ENVIRONMENT_ACCESS_MONGODB = "SYSTEM_ENVIRONMENT_ACCESS_MONGODB";

        public static readonly string TIPO_FACTOR_TASA = "Tasa";
        public static readonly string TIPO_FACTOR_CUOTA = "Cuota";
        public static readonly string TIPO_FACTOR_EXENTO = "Exento";

        public static readonly string IMPUESTO_001 = "001"; //ISR
        public static readonly string IMPUESTO_002 = "002"; //IVA
        public static readonly string IMPUESTO_003 = "003"; //IEPS

        public static readonly string ERROR = "error";
        public static readonly string SUCCESS = "success";

        public const string PRINCIPAL = "principal";
        public const string ASOCIADO = "asociado";
        public const string ANEXO = "anexo";

        public const string DISPONIBLE = "disponible";
        public const string CANCELADO = "cancelada";
        public const string SUSPENDIDO = "suspendido";

        public const string PROVEEDOR_ACTIVO = "activo";

        public static readonly string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

    }
}
