using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET
{
    public enum ResultCode:byte
    {
        Success = 0x00,
        Failed = 0xff,
        NotImplemented = 0x01
    }

    public class TraCIResponse<T> : IResponseInfo
    {
        public byte Identifier { get; set; }

        public byte? ResponseIdentifier { get; set; }

        public ResultCode Result { get; set; }

        public T Content { get; set; }

        public string ErrorMessage { get; set; }

        public byte? Variable { get; set; }

        public U GetContentAs<U>()
        {
            return (U)Convert.ChangeType(Content, typeof(U));
        }
    }
}
