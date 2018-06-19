package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.AllergiesData
import features.myrecord.MedicationsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordMedicationsStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance

    val HTTP_EXCEPTION = "HttpException"

    @Then("I receive the medications object")
    fun thenIReceiveAMedicationsObject() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertNotNull(result.response.medications.data)
    }

    @Given("the GP Practice has enabled medications functionality")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionality() {
        mockingClient.forEmis {
            medicationsRequest(MockDefaults.patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }

    @But("the GP Practice has disabled medications functionality")
    fun butTheGPPracticeHasDisabledMedicationsFunctionality() {
        try {
            mockingClient.forEmis {
                medicationsRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
            }

            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" acute medications as part of the my record object")
    fun thenIReceiveAnAcuteMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.medications.data.acuteMedications.count())
    }

    @Then("I receive \"(.*)\" current repeat medications as part of the my record object")
    fun thenIReceiveACurrentRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.medications.data.currentRepeatMedications.count())
    }

    @Then("I receive \"(.*)\" discontinued repeat medications as part of the my record object")
    fun thenIReceiveADiscontinuedRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.medications.data.discontinuedRepeatMedications.count())
    }

    @And("the flag informing that the patient has access to the medications data is set to \"(.*)\"")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.medications.hasAccess)
    }

    @And("the flag informing that there was an error retrieving the medications data is set to \"(.*)\"")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.medications.hasErrored)
    }
}

