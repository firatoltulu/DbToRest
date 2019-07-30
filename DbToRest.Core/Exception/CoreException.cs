using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DbToRest.Core.Exception
{
    [Serializable]
    public class CoreException : System.Exception
    {
        private IDictionary _data;
        public CoreException()
        {
        }

        public CoreException(string message)
            : base(message)
        {
        }

        public CoreException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        public CoreException(string message, IDictionary data)
            : base(message)
        {
            _data = data;
        }

        public override IDictionary Data
        {
            get
            {
                return _data;
            }
        }

        protected CoreException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        public CoreException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}