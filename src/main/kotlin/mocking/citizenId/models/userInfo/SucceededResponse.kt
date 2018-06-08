package mocking.citizenId.models.userInfo

import mocking.defaults.MockDefaults
import models.Patient

class SucceededResponse(patient: Patient = MockDefaults.patient) {
    var sub: String? = "9ffaa2cb-3714-4309-ad22-9d4f6bf0f531"
    var name: String? = "Realm1 Admin"
    var preferred_username: String? = "realmadmin@gmail.com"
    var given_name: String? = "realmadmin@gmail.com"
    var family_name: String? = "Admin"
    var email: String? = "realmadmin@gmail.com"
    var im1_connection_token: String? = patient.connectionToken
    var ods_code: String? = patient.odsCode

}