namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class NhsLoginErrorModel: NhsLoginModel
    {
        public NhsLoginErrorModel(NhsLoginModel nhsLoginModel, string errorReferenceCode) : base(nhsLoginModel)
        {
            ServiceDeskReference = errorReferenceCode;
        }

        internal string ServiceDeskReference { get; }
    }
}