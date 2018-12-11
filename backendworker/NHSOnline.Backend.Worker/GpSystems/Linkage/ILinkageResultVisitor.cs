namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ILinkageResultVisitor<out T>
    {
        T Visit(LinkageResult.SuccessfullyRetrieved result);

        T Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result);

        T Visit(LinkageResult.SuccessfullyCreated result);

        T Visit(LinkageResult.NotFoundErrorRetrievingNhsUser result);

        T Visit(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount result);

        T Visit(LinkageResult.SupplierSystemUnavailable result);

        T Visit(LinkageResult.InternalServerError result);

        T Visit(LinkageResult.PracticeNotLive result);

        T Visit(LinkageResult.PatientMarkedAsArchived result);

        T Visit(LinkageResult.PatientNonCompetentOrUnderMinimumAge result);

        T Visit(LinkageResult.AccountStatusInvalid result);

        T Visit(LinkageResult.PatientNotRegisteredAtPractice result);

        T Visit(LinkageResult.NoRegisteredOnlineUserFound result);

        T Visit(LinkageResult.NotFoundErrorCreatingNhsUser result);

        T Visit(LinkageResult.BadRequestErrorRetrievingNhsUser result);

        T Visit(LinkageResult.BadRequestErrorCreatingNhsUser result);

        T Visit(LinkageResult.ForbiddenErrorRetrievingNhsUser result);

        T Visit(LinkageResult.LinkageKeyRevoked result);
    }
}
