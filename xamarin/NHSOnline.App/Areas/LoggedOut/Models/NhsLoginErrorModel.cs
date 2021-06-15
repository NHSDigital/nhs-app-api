using System.Text.RegularExpressions;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class NhsLoginErrorModel: NhsLoginModel
    {
        public NhsLoginErrorModel(NhsLoginModel nhsLoginModel, string errorReferenceCode)
            : base(nhsLoginModel)
        {
            ServiceDeskReference = errorReferenceCode;
            AccessibleServiceDeskReference = Regex.Replace(errorReferenceCode, ".{1}", "$0 ");
        }

        internal string AccessibleServiceDeskReference { get; set; }

        internal string ServiceDeskReference { get; }
    }
}