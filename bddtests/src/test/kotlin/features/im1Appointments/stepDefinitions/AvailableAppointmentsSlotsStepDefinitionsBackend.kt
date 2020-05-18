package features.im1Appointments.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DemographicsFactory
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrNull
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 90L

class AvailableAppointmentsSlotsStepDefinitionsBackend {

    @Given("^the system will time out when trying to retrieve (.*) appointment slots$")
    fun appointmentSlotsTimesOut(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithSuccess(AppointmentsSlotsExample().getGenericExample())
                    .delayedBy(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
        }
    }

    @Given("^I have (.*) telephone number\\(s\\) stored for (.*)$")
    fun iHaveTelephoneNumbersStored(telephoneNumberTypes: String, gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        var invalidPhoneNumberTypes = true

        val patient = Patient.getDefault(supplier)
        patient.contactDetails.telephoneFirst = ""
        patient.contactDetails.telephoneSecond = ""

        if (telephoneNumberTypes == "no") {
            invalidPhoneNumberTypes = false
        } else {
            if (telephoneNumberTypes.contains("first")) {
                patient.contactDetails.telephoneFirst = "01234 456789"
                invalidPhoneNumberTypes = false
            }
            if (telephoneNumberTypes.contains("second")) {
                patient.contactDetails.telephoneSecond = "07912 345678"
                invalidPhoneNumberTypes = false
            }
        }
        Assert.assertFalse("No valid telephone number type passed into the step. ", invalidPhoneNumberTypes)

        SerenityHelpers.setPatient(patient)
        val factory = DemographicsFactory.getForSupplier(supplier)
        factory.enabled(patient)
    }

    @Given("^the system will respond with forbidden when trying to retrieve (.*) appointment slots\$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithGPErrorWhenNotEnabled()
        }
    }

    @When("^the available appointment slots are retrieved without a cookie$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutACookie() {
        retrieveAppointmentSlots(null, null, includeCookie = false)
    }

    private fun retrieveAppointmentSlots(fromDate: String? = null,
                                         toDate: String? = null,
                                         includeCookie: Boolean = true) {
        var cookie: Cookie? = null
        if (includeCookie) {
            cookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
        }

        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .appointments.getAppointmentSlots(
                        patientId,
                        fromDate,
                        toDate,
                        cookie)
        Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
    }

    @Then("^(\\d{1}) telephone number\\(s\\) are retrieved$")
    fun telephoneNumbersAreRetrieved(expectedNumberOfTelephoneNumbers: Int) {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        Assert.assertEquals(
                "Incorrect number of telephone numbers retrieved. ",
                expectedNumberOfTelephoneNumbers,
                result.telephoneNumbers.size
        )

        val actualTelephoneNumbers = ArrayList<String?>()
        for (telephoneNumber in result.telephoneNumbers) {
            actualTelephoneNumbers.add(telephoneNumber.telephoneNumber)
        }

        val patient = SerenityHelpers.getPatient()
        if  (!patient.contactDetails.telephoneFirst.isNullOrEmpty()) Assert.assertTrue(
                "Telephone Number not found in response. Only found $actualTelephoneNumbers. ",
                actualTelephoneNumbers.contains(patient.contactDetails.telephoneFirst)
        )
        if  (!patient.contactDetails.telephoneSecond.isNullOrEmpty()) Assert.assertTrue(
                "Mobile Telephone Number not found in response. Only found $actualTelephoneNumbers. ",
                actualTelephoneNumbers.contains(patient.contactDetails.telephoneSecond)
        )
    }
}
