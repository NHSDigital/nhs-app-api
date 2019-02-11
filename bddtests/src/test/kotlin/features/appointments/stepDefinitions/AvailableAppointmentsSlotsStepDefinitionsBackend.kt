package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsSlotsFactory
import features.appointments.steps.AvailableAppointmentsSteps
import features.myrecord.factories.DemographicsFactory
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.emis.demographics.ContactDetails
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 90L

class AvailableAppointmentsSlotsStepDefinitionsBackend {

    @Steps
    private lateinit var availableAppointments: AvailableAppointmentsSteps

    @Given("^the system will time out when trying to retrieve (.*) appointment slots$")
    fun appointmentSlotsTimesOut(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
        }
    }

    @Given("^I have (.*) telephone number\\(s\\) stored$")
    fun iHaveTelephoneNumbersStored(telephoneNumberTypes: String) {
        var invalidPhoneNumberTypes = true
        val contactDetails = ContactDetails()
        if (telephoneNumberTypes == "no") {
            invalidPhoneNumberTypes = false
        } else {
            if (telephoneNumberTypes.contains("home")) {
                contactDetails.telephoneNumber = "01234 456789"
                invalidPhoneNumberTypes = false
            }
            if (telephoneNumberTypes.contains("mobile")) {
                contactDetails.mobileNumber = "07912 345678"
                invalidPhoneNumberTypes = false
            }
        }
        Assert.assertFalse("No valid telephone number type passed into the step. ", invalidPhoneNumberTypes)
        // Currently only applicable for EMIS
        val patient = Patient.getDefault("EMIS").copy(contactDetails = contactDetails)
        SerenityHelpers.setPatient(patient)
        val factory = DemographicsFactory.getForSupplier("EMIS")
        factory.enabled(patient)
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

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.getAppointmentSlots(fromDate,
                    toDate,
                    cookie)
            Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
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
        if  (!patient.contactDetails.telephoneNumber.isNullOrEmpty()) Assert.assertTrue(
                "Telephone Number not found in response. Only found $actualTelephoneNumbers. ",
                actualTelephoneNumbers.contains(patient.contactDetails.telephoneNumber)
        )
        if  (!patient.contactDetails.mobileNumber.isNullOrEmpty()) Assert.assertTrue(
                "Mobile Telephone Number not found in response. Only found $actualTelephoneNumbers. ",
                actualTelephoneNumbers.contains(patient.contactDetails.mobileNumber)
        )
    }
}
