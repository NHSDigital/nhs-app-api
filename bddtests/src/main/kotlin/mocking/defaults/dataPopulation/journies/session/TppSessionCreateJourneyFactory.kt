package mocking.defaults.dataPopulation.journies.session

import constants.TppConstants
import mocking.defaults.TppMockDefaults
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.NationalId
import mocking.tpp.models.PatientAccess
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.Registration
import mocking.tpp.models.User
import models.Patient

class TppSessionCreateJourneyFactory : SessionCreateJourneyFactory() {

    private fun authenticateRequest(patient: Patient): Authenticate {
        return Authenticate(
                apiVersion = TppMockDefaults.TPP_API_VERSION,
                accountId = patient.accountId,
                passphrase = patient.passphrase,
                unitId = patient.odsCode,
                uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                application = Application(
                        name = "NhsApp",
                        version = "1.0",
                        providerId = TppMockDefaults.DEFAULT_TPP_PROVIDER_ID,
                        deviceType = "NhsApp"
                ))
    }

    private fun authenticationReply(patient: Patient): AuthenticateReply {
        val person = createPerson(patient)

        val reply =  AuthenticateReply(
                patientId = patient.patientId,
                onlineUserId = patient.patientId,
                uuid = "01068966-0a47-429c-9abd-e5c05736a6f7",
                user = User(person),
                person = mutableListOf<Person>(person),
                registration = Registration(
                        mutableListOf(PatientAccess(patientId = patient.patientId))
                )
        )

        patient.linkedAccounts.forEach {
            reply.person.add(createPerson(it))
            reply.registration.patientAccess.add(PatientAccess(patientId = it.patientId))
        }

        return reply
    }

    fun createAuthenticateRequest(patient: Patient) {
        client.forTpp.mock {
            authentication.authenticateRequest(authenticateRequest(patient))
                    .respondWithSuccess(authenticationReply(patient))
        }
    }

    override fun createFor(patient: Patient, defaultPracticeSettings:Boolean) {
        createAuthenticateRequest(patient)
        client.forTpp.mock { authentication.logOffRequest().respondWithSuccess() }
    }

    private fun createPerson(patient: Patient): Person {
        return Person(
                patientId = patient.patientId,
                dateOfBirth = patient.age.dateOfBirth,
                gender = patient.sex.name,
                nationalId = NationalId(
                        type = TppConstants.NationalIdTypeNhs,
                        value = patient.nhsNumbers.firstOrNull() ?: ""
                ),
                personName = PersonName(
                        name = "${patient.name.title} ${patient.name.firstName} ${patient.name.surname}"
                )
        )
    }
}
