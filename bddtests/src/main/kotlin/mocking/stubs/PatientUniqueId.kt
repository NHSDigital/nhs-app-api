package mocking.stubs

enum class PatientUniqueId(val Id: String) {
        GoodPatientEMIS("1"),
        TimeoutPatientEMIS("2"),
        ServiceNotEnabledPatientEMIS("3"),
        SessionErrorPatientEMIS("4"),
        ServerErrorPatientEMIS("5"),
        GoodPatientTPP("6")
}