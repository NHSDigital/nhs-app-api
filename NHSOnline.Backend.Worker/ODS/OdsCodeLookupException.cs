using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NHSOnline.Backend.Worker.Ods
{
    [Serializable]
    public class OdsCodeLookupException : Exception
    {
        public string OdsCode { get; set; }

        public OdsCodeLookupException()
        {
        }

        public OdsCodeLookupException(string message) : base(message)
        {
        }

        public OdsCodeLookupException(string message, string odsCode) : base(message)
        {
            this.OdsCode = odsCode;
        }

        public OdsCodeLookupException(string message, string odsCode, Exception inner)
            : base(message, inner)
        {
            this.OdsCode = odsCode;
        }

        public OdsCodeLookupException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OdsCodeLookupException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            this.OdsCode = info.GetString(nameof(OdsCode));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(OdsCode), this.OdsCode);

            base.GetObjectData(info, context);
        }
    }
}