using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTransferAppBackend.Constants
{
    public enum AuthResults
    {

        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 00,

        /// <summary>
        /// invalid username or password
        /// </summary>
        FAILURE = 101,

    }
}
