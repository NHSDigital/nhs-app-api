package mocking.emis.models

import java.util.*

class UserPatientLink(val title: String,
                      val firstName: String,
                      val surname: String,
                      val userPatientLinkToken: String,
                      val odsCode: String,
                      val associationType: AssociationType) {
    val age = 42
    val patientActivityContextGuid = UUID.randomUUID()
}