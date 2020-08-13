package features.myrecord.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.myrecord.factories.ImmunisationsFactory
import mocking.MockingClient
import mocking.data.myrecord.ImmunisationsData
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MedicalRecordImmunisationStepDefinitionsBackend {

    private val mockingClient = MockingClient.instance

    @Given("^the GP Practice has enabled immunisations functionality and multiple immunisation records exist$")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleRecordsExist() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^no immunisation records exist for the patient$")
    fun givenNoImmunisationRecordsExistForThePatient() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the user does not have access to view immunisations$")
    fun givenUserDoesNotHaveAccessToViewImmunisations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^the GP practice returns a bad immunisations response$")
    fun givenThereIsACorruptedImmunisationsResponse() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        ImmunisationsFactory.getForSupplier(gpSystem).respondWithACorruptedResponse(SerenityHelpers.getPatient())
    }

    @Given("^the EMIS GP Practice has two immunisation results where the first record has no date$")
    fun givenTheEmisGpPracticeHasAnImmunisationResultWithNoDate() {
        mockingClient.forEmis.mock {
            myRecord.immunisationsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(ImmunisationsData.getTwoImmunisationResultsWhereTheFirstRecordHasNoDate())
        }
    }

    @When("^I get the users immunisations$")
    fun whenIGetTheUsersImmunisations() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .myRecord.getMyRecord(patientId)

        Serenity.setSessionVariable(MyRecordResponse::class).to(result)
    }

    @Then("^I receive \"(.*)\" immunisations as part of the my record object$")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.immunisations.data.count())
    }
}
