package mocking.microtest.prescriptions

data class PrescriptionOrderPartialSuccessResponse(
        var PatientRequests: ArrayList<PatientRequest> = arrayListOf()
)
