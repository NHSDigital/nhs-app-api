package worker

object WorkerPaths {
    val patientIm1ConnectionV2 = "/v2/patient/im1connection"
    val patientIm1ConnectionV1 = "/v1/patient/im1connection"
    val sessionConnection = "/v1/session"
    val getPrescriptionsConnection = "/v1/patient/prescriptions"
    val postPrescriptionsConnection = "/v1/patient/prescriptions"
    val getCoursesConnection = "/v1/patient/courses"
    val myAppointments = "/v1/patient/appointments"
    val appointmentSlots = "/v1/patient/appointment-slots"
    val getMyRecordConnection = "/v1/patient/my-record"
    val getDemographicsConnection = "/v1/patient/demographics"
    val sessionConnectionExtend = "/v1/session/extend"
    val LinkageKey = "/v1/patient/linkage"
    val ndopConnection = "/v1/patient/ndop"
    val organDonationConnection = "/v1/patient/organdonation"
    val serviceJourneyRules = "/v1/patient/journey-configuration"
}
