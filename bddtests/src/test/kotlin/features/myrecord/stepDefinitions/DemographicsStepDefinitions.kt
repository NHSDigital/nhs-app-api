package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.data.myrecord.DemographicsData
import mocking.defaults.MockDefaults
import mocking.tpp.models.Error
import mocking.vision.VisionConstants
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.demographics.Demographics

open class DemographicsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @When("^I get the users demographic data$")
    fun whenIGetTheUsersDemographicsDataFor() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getDemographics()

            Serenity.setSessionVariable(Demographics::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Given("^the GP Practice has enabled demographics functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledDemographicsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    demographicsRequest(this@DemographicsStepDefinitions.patient).respondWithSuccess(DemographicsData.getEmisDemographicData(this@DemographicsStepDefinitions.patient))
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    patientSelectedPost(this@DemographicsStepDefinitions.patient.tppUserSession!!).respondWithSuccess(DemographicsData.getTppDemographicsData(this@DemographicsStepDefinitions.patient))
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    demographicsRequest(visionUserSession = VisionUserSession(
                            this@DemographicsStepDefinitions.patient.rosuAccountId,
                            this@DemographicsStepDefinitions.patient.apiKey,
                            Patient.aderynCanon.odsCode, this@DemographicsStepDefinitions.patient.patientId)
                    ).respondWithSuccess(MockDefaults.visionDemographicsResponse)

                }
            }
        }
    }

    @Given("^there is an error getting demographics for (.*)$")
    fun thereIsAnErrorGettingTheDemographicsFor(getService:String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "VISION" -> {
                mockingClient.forVision {
                    demographicsRequest(visionUserSession = VisionUserSession(
                            this@DemographicsStepDefinitions.patient.rosuAccountId,
                            this@DemographicsStepDefinitions.patient.apiKey,
                            Patient.aderynCanon.odsCode, this@DemographicsStepDefinitions.patient.patientId)
                    ).respondWithUnknownError()
                }
            }
        }
    }

    @Given("^the GP Practice has disabled demographics functionality for (.*)$")
    fun butTheGPPracticeHasDisabledDemographicsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                try {
                    mockingClient.forEmis {
                        demographicsRequest(this@DemographicsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                    }
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
            "TPP" -> {
                try {
                    mockingClient.forTpp {
                        patientSelectedPost(this@DemographicsStepDefinitions.patient.tppUserSession!!).respondWithError(Error("6", "Error Occurred", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                    }
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
            "VISION" -> {
                try {
                    mockingClient.forVision {
                            demographicsRequest(visionUserSession = VisionUserSession(
                                    this@DemographicsStepDefinitions.patient.rosuAccountId,
                                    this@DemographicsStepDefinitions.patient.apiKey,
                                    Patient.aderynCanon.odsCode, this@DemographicsStepDefinitions.patient.patientId)
                            ).respondWithAccessDeniedError()
                    }
                } catch (httpException: NhsoHttpException) {
                    Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
                }
            }
        }
    }

    @Then("^I receive the demographic object$")
    fun thenIReceiveADemographicObject() {
        val result = Serenity.sessionVariableCalled<Demographics>(Demographics::class)
        Assert.assertNotNull(result)
    }
}

