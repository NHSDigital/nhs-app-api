package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.data.myrecord.ProblemsData
import mocking.vision.VisionConstants
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import worker.models.myrecord.MyRecordResponse
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordProblemsStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled problems functionality and the patient has 3 problems$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityAndPatientHasSomeProblems() {
        mockingClient.forEmis {
            myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemsData())
        }
    }

    @Given("^the GP Practice has enabled problems functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemsData())
                }
            }
            "TPP"->{
            }
            "VISION"->{
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
                            view = VisionConstants.problemsView,
                            responseFormat = VisionConstants.xmlResponseFormat
                    ).respondWithSuccess(ProblemsData.getVisionProblemsData())
                }
            }
        }
    }

    @Given("^the GP Practice has enabled problems functionality and has 3 different problems with different date formats$")
    fun givenTheGPPracticeHasEnabledProblemsFunctionalityAndHasThreeDifferentProblemsWithDifferentDateFormats() {
        mockingClient.forEmis {
            myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getProblemRecordsWithDifferentDateParts())
        }
    }

    @But("^the GP Practice has disabled problems functionality for (.*)$")
    fun butTheGPPracticeHasDisabledProblemsFunctionality(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP"->{
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
                            view = VisionConstants.problemsView,
                            responseFormat = VisionConstants.xmlResponseFormat
                    ).respondWithAccessDeniedError()
                }
            }
        }
    }
    @Given("^no Problems records exist for the patient for (.*)$")
    fun givenNoProblemsRecordsExistForThePatient(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithSuccess(ProblemsData.getDefaultProblemModel())
                }
            }
            "TPP"->{
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
                            view = VisionConstants.problemsView,
                            responseFormat = VisionConstants.xmlResponseFormat
                    ).respondWithSuccess(ProblemsData.getVisionProblemsDataWithNoProblems())
                }
            }
        }
    }

    @Given("^the user does not have access to view Problems for (.*)$")
    fun givenUserDoesNotHaveAccessToViewProblems(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP"->{
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
                            view = VisionConstants.problemsView,
                            responseFormat = VisionConstants.xmlResponseFormat
                    ).respondWithAccessDeniedError()
                }
            }
        }
    }

    @Given("^there is an error retrieving Problems data for (.*)$")
    fun givenThereIsAnErrorRetrievingProblemsData(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.problemsRequest(this@MyRecordProblemsStepDefinitions.patient).respondWithNonDataAccessException()
                }
            }
            "TPP"->{
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
                            view = VisionConstants.problemsView,
                            responseFormat = VisionConstants.xmlResponseFormat
                    ).respondWithUnknownError()
                }
            }
        }
    }

    @When("^I get the users Problems$")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" problems as part of the my record object$")
    fun thenIReceiveAnProblemsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.problems.data.count())
    }

    @And("^the flag informing that the patient has access to the problem data is set to \"(.*)\"$")
    fun andHasAccessToProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the problem data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingProblemsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.problems.hasErrored)
    }
}
