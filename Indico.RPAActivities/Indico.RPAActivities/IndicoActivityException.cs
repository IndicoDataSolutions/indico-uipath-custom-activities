using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Indico.RPAActivities
{
    class IndicoActivityException : SystemException
    {
        public IndicoActivityException()
        { }

        public IndicoActivityException(string message) : base(message)
        { }

        public IndicoActivityException(string message, SystemException inner) : base(message, inner)
        { }

        public IndicoActivityException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        { }
    }
}
