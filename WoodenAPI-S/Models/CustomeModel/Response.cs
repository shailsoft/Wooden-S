using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoodenAPI_S.Models.CustomeModel;

namespace WoodenAPI_S.Models.CustomeModel
{
    public class Response<T>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public T Result { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The type of the status.
        /// </value>
        public StatusCode Status { get; set; }

        /// <value>
        /// The type of the status.
        /// </value>
        public string StatusMessage { get; set; }

        public string Error { get; set; }
    }
}
