using System;
using System.Runtime.Serialization;

namespace DsiCodeTech.Common.Exception
{
    [Serializable]
    public class BusinessException : System.Exception
    {
        private readonly string _id;
        private readonly string _detail;

        public BusinessException(string message) : base(message)
        {
            this._detail = message;
        }

        public BusinessException(string id, string message) : base(message)
        {
            this._id = id;
            this._detail = message;
        }

        public BusinessException(string message, System.Exception exception) : base(message, exception)
        {
            this._detail = message;
        }

        public BusinessException(string id, string message, System.Exception exception) : base(message, exception)
        {
            this._id = id;
            this._detail = message;
        }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string Id { get { return this._id; } }
        public string Detail { get { return this._detail; } }
    }
}
