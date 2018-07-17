package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.tpp.models.*
import models.Patient

class TppSessionCreateJourneyFactory(val client: MockingClient) :SessionCreateJourneyFactory (){

    override fun createFor(patient: Patient) {
        client.forTpp {
            authenticateRequest(Authenticate(
                    apiVersion = "1",
                    accountId = patient.accountId,
                    passphrase = patient.passphrase,
                    unitId = MockDefaults.DEFAULT_ODS_CODE_TPP,
                    uuid = MockDefaults.DEFAULT_TPP_UUID,
                    application = Application(
                            name = "NhsApp",
                            version = "1.0",
                            providerId = MockDefaults.DEFAULT_TPP_PROVIDER_ID,
                            deviceType = "NhsApp"
                    )))
                   .respondWithSuccesssWithoutSuid(AuthenticateReply(
                           patientId = patient.patientId,
                           onlineUserId = patient.patientId,
                           uuid = "01068966-0a47-429c-9abd-e5c05736a6f7",
                           user = User(
                                   person = Person(
                                           patientId = patient.patientId,
                                           dateOfBirth = patient.dateOfBirth,
                                           gender = patient.sex.name,
                                           nationalId = NationalId(
                                                   type = "NHS",
                                                   value = patient.nhsNumbers.first()
                                           ),
                                           personName = PersonName(
                                                   name = "${patient.title} ${patient.firstName} ${patient.surname}"
                                           )
                                   )
                           )))
        }
    }
}