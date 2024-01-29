using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET
{
    public interface IResponseInfo
    {
        string ErrorMessage { get; set; }

        byte Identifier { get; set; }

        byte? ResponseIdentifier { get; set; }

        ResultCode Result { get; set; }

        /// <summary>
        /// The Variable type 
        /// </summary>
        byte? Variable { get; set; }

        T GetContentAs<T>();
    }
}
