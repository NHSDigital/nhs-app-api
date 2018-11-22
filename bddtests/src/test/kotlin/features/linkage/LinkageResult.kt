package features.linkage

enum class LinkageResult {
    NoneSpecified,
    SuccessfullyRetrievedFirstTime,
    SuccessfullyRetrieved,
    AccountStatusInvalid,
    PatientNonCompetentOrUnderMinimumAge,
    PatientMarkedAsArchived,
    NoRegisteredOnlineUserFound,
    PatientNotRegisteredAtPractice,
    PracticeNotLive,
    PatientAlreadyHasAnOnlineAccount,
    SuccessfullyCreated,
    InternalServerError,
}