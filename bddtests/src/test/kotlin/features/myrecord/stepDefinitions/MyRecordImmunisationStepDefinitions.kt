package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.data.myrecord.ImmunisationsData
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.immunisationsView
import mocking.vision.VisionConstants.xmlResponseFormat
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import worker.models.myrecord.MyRecordResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordImmunisationStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled immunisations functionality and multiple immunisation records exist for (.*)$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleImmunisationRecordsExistFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getImmunisationsData())
                }
            }
            "TPP" -> {

            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                                    view = immunisationsView,
                                    responseFormat = xmlResponseFormat
                    ).respondWithSuccess(ImmunisationsData.getVisionImmunisationsData(2))

                }
            }
        }
    }

    @Given("^no immunisation records exist for the patient for (.*)$")
    fun givenNoImmunisationRecordsExistForThePatientFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
                }
            }
            "TPP" -> {

            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                                    view = immunisationsView,
                                    responseFormat = xmlResponseFormat
                    ).respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())
                }
            }

        }
    }

    @Given("^the user does not have access to view immunisations for (.*)$")
    fun givenUserDoesNotHaveAccessToViewImmunisationsFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.immunisationsRequest(patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {

            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                                    view = immunisationsView,
                                    responseFormat = xmlResponseFormat
                    ).respondWithAccessDeniedError()
                }
            }
        }
    }

    @Given("^there is an error retrieving immunisations data for (.*)$")
    fun givenThereIsAnErrorRetrievingImmunisationsDatafor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.immunisationsRequest(patient).respondWithNonDataAccessException()
                }
            }
            "TPP" -> {

            }
            "VISION" -> {
                mockingClient.forVision {
                    getPatientDataRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId),
                            serviceDefinition = ServiceDefinition(
                                    name = VisionConstants.patientDataName,
                                    version = VisionConstants.patientDataVersion),
                                    view = immunisationsView,
                                    responseFormat = xmlResponseFormat
                    ).respondWithUnknownError()
                }
            }
        }
    }

    @When("^I get the users immunisations$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" immunisations as part of the my record object$")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.immunisations.data.count())
    }
}
