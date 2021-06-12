using System;
using MIE.Entity.Enum;

namespace MIE.Utils
{
    public class ResponseUtil
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }

        public static ResponseUtil SuccessfulResponse(string msg,
            object data = null) => new ResponseUtil(0, msg, data);

        public static ResponseUtil ErrorResponse(int code,
            string msg, object data = null) => new ResponseUtil(code, msg, data);

        public static ResponseUtil ErrorResponse(ResponseEnum resEnum)
            => new ResponseUtil(resEnum);

        public ResponseUtil(int code, string msg, object data)
        {
            this.Code = code;
            this.Msg = msg;
            this.Data = data;
        }

        public ResponseUtil(ResponseEnum resEnum) : this(resEnum.Code, resEnum.Msg, null)
        {
            
        }

    }
}
