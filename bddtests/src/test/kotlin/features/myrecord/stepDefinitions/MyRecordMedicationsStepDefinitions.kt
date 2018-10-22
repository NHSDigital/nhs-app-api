package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.data.myrecord.MedicationsData
import mocking.tpp.models.Error
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.medicationsView
import mocking.vision.VisionConstants.xmlResponseFormat
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordMedicationsStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Then("^I receive the medications object$")
    fun thenIReceiveAMedicationsObject() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertNotNull(result.response.medications.data)
    }

    @Given("^the GP Practice has enabled medications functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityfor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    myRecord.medicationsRequest(this@MyRecordMedicationsStepDefinitions.patient).respondWithSuccess(MedicationsData.getEmisMedicationData())
                }
            }
            "TPP"->{
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(this@MyRecordMedicationsStepDefinitions.patient.tppUserSession!!).respondWithSuccess(MedicationsData.getTppMedicationData())
                }
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
                            view = medicationsView,
                            responseFormat = xmlResponseFormat
                    ).respondWithSuccess(MedicationsData.getVisionMedicationsData())
                }
            }
        }
    }

    @Given("^the GP Practice has enabled medication functionality and the patient has no medications for (.*)$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityandpatienthasnomedicationsfor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                try {
                    mockingClient.forEmis {
                        myRecord.medicationsRequest(this@MyRecordMedicationsStepDefinitions.patient).respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
                    }

                    val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

                    Serenity.setSessionVariable(MyRecordResponse::class).to(result)
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
            "TPP" -> {
                try {
                    mockingClient.forTpp {
                        myRecord.viewPatientOverviewPost(this@MyRecordMedicationsStepDefinitions.patient.tppUserSession!!).respondWithSuccess(MedicationsData.getTppDefaultMedicationsModel())
                }

                val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

                Serenity.setSessionVariable(MyRecordResponse::class).to(result)

                }
                catch(httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
            "VISION" -> {
                try {
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
                                view = medicationsView,
                                responseFormat = xmlResponseFormat
                        ).respondWithSuccess(MedicationsData.getEmptySetOfVisionMedicationData())
                    }
                    val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

                    Serenity.setSessionVariable(MyRecordResponse::class).to(result)

                }
                catch(httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
        }
    }

    @But("^the GP Practice has disabled medications functionality for (.*)$")
    fun butTheGPPracticeHasDisabledMedicationsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.medicationsRequest(this@MyRecordMedicationsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(this@MyRecordMedicationsStepDefinitions.patient.tppUserSession!!).respondWithError(Error("6", "Requested record access is disabled by the practice", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    @Then("^I receive \"(.*)\" acute medications as part of the my record object$")
    fun thenIReceiveAnAcuteMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.acuteMedications.count())
    }

    @Then("^I receive \"(.*)\" current repeat medications as part of the my record object$")
    fun thenIReceiveACurrentRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.currentRepeatMedications.count())
    }

    @Then("^I receive \"(.*)\" discontinued repeat medications as part of the my record object$")
    fun thenIReceiveADiscontinuedRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.discontinuedRepeatMedications.count())
    }

    @And("^the flag informing that the patient has access to the medications data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the medications data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasErrored)
    }
}

