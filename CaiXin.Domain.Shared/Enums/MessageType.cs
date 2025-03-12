using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Shared.Enums
{
    /// <summary>
    ///
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// none
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter), true)]
        None = 0,

        /// <summary>
        /// info
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter), true)]
        Info = 1,

        /// <summary>
        /// warn
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter), true)]
        Warning = 2,

        /// <summary>
        /// error
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter), true)]
        Error = 3,

        /// <summary>
        /// success
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter), true)]
        Success = 4,
    }
}
