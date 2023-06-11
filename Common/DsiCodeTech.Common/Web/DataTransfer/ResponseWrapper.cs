using System.Net;

namespace DsiCodeTech.Common.Web.DataTransfer
{
    public class ResponseWrapper<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
