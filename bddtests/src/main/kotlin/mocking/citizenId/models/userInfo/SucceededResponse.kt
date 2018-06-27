package mocking.citizenId.models.userInfo

import mocking.defaults.MockDefaults
import models.Patient

class SucceededResponse(patient: Patient = MockDefaults.patient) {
    var sub: String? = "9ffaa2cb-3714-4309-ad22-9d4f6bf0f531"
    var name: String? = patient.firstName
    var preferred_username: String? = patient.firstName
    var given_name: String? = patient.firstName
    var family_name: String? = patient.surname
    var email: String? = patient.contactDetails.emailAddress
    var im1_connection_token: String? = patient.connectionToken
    var ods_code: String? = patient.odsCode
}