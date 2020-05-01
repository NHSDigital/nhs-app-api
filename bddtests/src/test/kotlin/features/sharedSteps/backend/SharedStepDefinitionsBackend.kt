package features.sharedSteps.backend

import constants.Supplier
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.serenitybdd.core.Serenity
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.session.UserSessionRequest
import java.util.*

open class SharedStepDefinitionsBackend{

    val mockingClient = MockingClient.instance

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)
        iHaveLoggedInAndHaveAValidSessionCookie()
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun iHaveLoggedInAndHaveAValidSessionCookie() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSystem)
        val isProofLevel9 = patient.identityProofingLevel == IdentityProofingLevel.P9
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)

        if( isProofLevel9 ) {
            SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        }

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = redirectUri))

        if( isProofLevel9 ) {
            DemographicsFactory
                    .getForSupplier(SerenityHelpers.getGpSupplier())
                    .enableForPatientProxyAccounts(patient)
        }

        val patientConfig = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getPatientLinkedAccountsConfiguration()
        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(patientConfig.id)
    }

    @Given("^I am a user with proof level 5 and have a valid session cookie$")
    fun iAmAUserWithProofLevel5AndHaveAValidSessionCookie() {
        val gpSystem = Supplier.valueOf("EMIS")
        val patient = Patient.getDefault(gpSystem).copy(identityProofingLevel = IdentityProofingLevel.P5)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        iHaveLoggedInAndHaveAValidSessionCookie()
    }

    @But("the patient id sent in the request is not valid")
    fun thePatientIdSendInTheRequestIsNotValid() {
        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(UUID.randomUUID().toString())
    }
}
