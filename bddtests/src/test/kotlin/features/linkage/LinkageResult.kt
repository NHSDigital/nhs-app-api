package features.linkage

enum class LinkageResult {
    NoneSpecified,
    SuccessfullyRetrievedFirstTime,
    SuccessfullyRetrieved,
    AccountStatusInvalid,
    PatientNonCompetentOrUnder16,
    PatientMarkedAsArchived,
    NoRegisteredOnlineUserFound,
    PatientNotRegisteredAtPractice,
    PracticeNotLive,
    PatientAlreadyHasAnOnlineAccount,
    SuccessfullyCreated,
    InternalServerError,
}