package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.defaults.MockDefaults.Companion.patient
import mocking.MockingClient
import features.myrecord.mockData.ImmunisationsData
import worker.models.myrecord.MyRecordResponse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordImmunisationStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Given("the GP Practice has enabled immunisations functionality and multiple immunisation records exist for (.*)")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleImmunisationRecordsExistfor(getService:String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getImmunisationsData())
                }
            }
            "TPP"->{

            }
        }
    }

    @Given("no immunisation records exist for the patient for (.*)")
    fun givenNoImmunisationRecordsExistForThePatientfor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getDefaultImmunisationsModel())
                }
            }
            "TPP"->{

            }
        }
    }

    @Given("the user does not have access to view immunisations for (.*)")
    fun givenUserDoesNotHaveAccessToViewImmunisationsfor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    immunisationsRequest(patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP"->{

            }
        }
    }

    @Given("there is an error retrieving immunisations data for (.*)")
    fun givenThereIsAnErrorRetrievingImmunisationsDatafor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService){
            "EMIS"->{
                mockingClient.forEmis {
                    immunisationsRequest(patient).respondWithNonDataAccessException()
                }
            }
            "TPP"->{

            }
        }
    }

    @When("I get the users immunisations")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" immunisations as part of the my record object")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.immunisations.data.count())
    }
}
