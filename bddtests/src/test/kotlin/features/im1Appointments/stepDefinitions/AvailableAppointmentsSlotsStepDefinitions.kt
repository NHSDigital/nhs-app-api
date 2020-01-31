package features.im1Appointments.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.im1Appointments.steps.AvailableAppointmentsSteps
import mocking.MockingClient
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.data.appointments.AppointmentsSlotsExampleForFiltering
import mocking.data.appointments.FilterSlotDetails
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.SC_OK
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import pages.assertIsVisible
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

class AvailableAppointmentsSlotsStepDefinitions {

    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    val mockingClient = MockingClient.instance
    private val appointmentSlotsExample = AppointmentsSlotsExample()
    private val appointmentSlotsExampleForFiltering = AppointmentsSlotsExampleForFiltering()

    private val expiredCookie = Cookie(
            "Set-Cookie",
            "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJLpr5j" +
                    "IZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuGh3Dc9Dqe9" +
                    "jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0iFNsJTXJm5FZBV" +
                    "BeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR4tU-rHpdyz0JcbStge" +
                    "PRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A6zxTv1y0nwu8AAXcUg7EF" +
                    "lSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
    )

    @Given("^there are available appointment slots with different criteria for (\\w+)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForSupplier(supplier)
    }

    private fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForSupplier(supplier: Supplier) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample()
    }

    @Given("^there are no available appointment slots for (.*)$")
    fun thereAreNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample(AppointmentsSlotsExampleBuilderWithExpectations().build())
    }

    @Given("^there is 1 available appointment slot for (.*)$")
    fun thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample(appointmentSlotsExample.singleSlotExample())
    }

    @Given("^there are available appointment slots for (.*) for 1 location$")
    fun thereAreAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample(appointmentSlotsExample.multipleSlotsOneLocation())
    }

    @Given("^there are appointment slots on some days other than tomorrow, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsButNotForTomorrowForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val example = appointmentSlotsExample.slotForDayAfterTomorrow()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days in This week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsThisWeekForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val example = appointmentSlotsExample.slotForEndOfToday()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days this week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysThisWeekButNotAllForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val example = appointmentSlotsExample.slotForEndOfToday()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days next week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysNextWeekButNotAllForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val example = appointmentSlotsExample.slotForThisTimeNextWeek()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days in the next few weeks but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsInTheNextFewWeeksForGPSystem(gpSystem: String) {
        thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem)
    }

    @Given("^there are available appointment slots with different slot types and locations for (\\w+)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentSlotTypesForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val exampleForFiltering = appointmentSlotsExampleForFiltering.getExampleForFilteringSlotTypesAndLocations()
        appointmentsSlotsFactory.generateExample(exampleForFiltering)
    }

    @Given("^there are available appointment slots with different clinician for (\\w+)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentClinicianForGPSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val exampleForFiltering = appointmentSlotsExampleForFiltering.getExampleForFilteringClinincian()
        appointmentsSlotsFactory.generateExample(exampleForFiltering)
    }

    @Given("^there are available EMIS appointment slots with different criteria " +
            "but there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier(Supplier.EMIS)
        val genericExample = appointmentSlotsExample.getGenericExample()
        factory.generateExample {
            respondWithSuccess(genericExample)
                    .delayedBy(Duration.ofSeconds(1))
        }
    }

    @Given("^Appointments are disabled for VISION at a GP Practice level")
    fun appointmentsAreDisabledForVisionAtAGPLevel() {
        Serenity.setSessionVariable(gpAppointmentsDisabled).to("true")
        AppointmentsSlotsFactory.getForSupplier(Supplier.VISION).generateDefaultUserData()
    }

    @Given("^(.*) is unavailable for available appointment slots$")
    fun gpSystemIUnavailableForAvailableAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithServiceUnavailable()
        }
    }

    @Given("^(.*) returns corrupt data for appointment slots$")
    fun gpSystemReturnsCorruptDataForAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithCorrupted()
        }
    }

    @Given("^there are available EMIS appointment slots, but session has expired$")
    fun thereAreAvailableAppointmentSlotsButExpiredSession() {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForSupplier(Supplier.EMIS)
        Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
    }

    @Given("^an unknown exception will occur when wanting to view (.*) appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        factory.generateExample {
            respondWithUnknownException()
        }
    }

    @Given("^I have selected a time when multiple slots are available$")
    fun iSelectATimeWhenMultipleSlotsAreAvailable() {
        val targetSlotDetails = sessionVariableCalled<FilterSlotDetails>(
                AppointmentSessionVariableKeys.APPOINTMENT_TO_SELECT
        )
        availableAppointments.availableAppointmentsPage.assertDateHeadingPresent(targetSlotDetails.dateAsUIString)
        availableAppointments.availableAppointmentsPage.timeSlotForDateTimeSession(
                targetSlotDetails.dateAsUIString,
                targetSlotDetails.timeAsUIString,
                targetSlotDetails.sessionName
        ).assertIsVisible()
        availableAppointments.assertOnlyOneTimeSlotPresent(
                targetSlotDetails.dateAsUIString,
                targetSlotDetails.timeAsUIString,
                targetSlotDetails.sessionName
        )
        availableAppointments.availableAppointmentsPage.selectSlot(
                targetSlotDetails.dateAsUIString,
                targetSlotDetails.timeAsUIString,
                targetSlotDetails.sessionName
        )
    }

    @When("^the available appointment slots are retrieved$")
    fun theAvailableAppointmentSlotsAreRetrieved() {
        availableAppointments.theAvailableAppointmentSlotsAreRetrieved()
    }

    @When("^I acknowledge that there are no appointments and go back to my appointments$")
    fun iAcknowledgeThatThereAreNoAppointmentsAndGoBackToMyAppointments() {
        availableAppointments.clickOnBackLink()
    }

    @When("^I decide I don't want to select an appointment and go back$")
    fun iDecideIDoNotWantToSelectAnAppointmentAndGoBack() {
        availableAppointments.clickOnBackLink()
    }

    @Then("^available slots are returned for the full date range$")
    fun availableSlotsAreReturned() {
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(
                AppointmentSessionVariableKeys.EXPECTED_APPOINTMENT_SESSIONS_KEY
        )
        val expectedAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
        for (appointmentSession in expectedAppointmentSessions) {
            expectedAppointmentSlots.addAll(appointmentSession.slots)
        }
        val httpStatus = SerenityHelpers.getValueOrNull<NhsoHttpException>("HttpException")?.statusCode ?: SC_OK

        assertEquals("Http Error Status. ", SC_OK, httpStatus)

        val actualResult = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull("Expected actualResult not null", actualResult)
        assertNotNull("Expected actualResult.slots not null", actualResult.slots)
        assertEquals("Incorrect number of slots. ", expectedAppointmentSlots.size, actualResult.slots.size)
    }

    @Then("^available slots are returned containing id, start date and time, " +
            "end date and time, location, clinicians, type$")
    fun availableSlotsAreReturnedWithAppropriateFields() {
        availableAppointments.verifyThatAvailableSlotsAreReturnedWithAppropriateFields()
    }

    @Then("^the Available Appointments page is displayed$")
    fun theAvailableAppointmentsPageIsDisplayed() {
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @Then("^no available slots are displayed$")
    fun noAvailableSlotsAreDisplayed() {
        availableAppointments.verifyThatNoSlotsAreDisplayed()
    }

    @Then("^a message is displayed indicating there are no slots available$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotAvailable() {
        availableAppointments.availableAppointmentsPage.warning().assertVisible(
                arrayListOf("No appointments available",
                        "There are currently no appointments available to book online right now. " +
                                "If you need to book one now, call your GP surgery.",
                        "If it's urgent and you do not know what to do, call 111 to get help near you."))
    }

    @Then("^a message is displayed indicating there are no slots for selected criteria$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotsForSelectedCriteria() {
        availableAppointments.availableAppointmentsPage.warning("No appointments available").assertVisible(
                arrayListOf(
                        "Try to filter appointments by a different period or " +
                                "select \"No preference\" for the practice member. " +
                                "If you cannot find the appointment you need, call your GP surgery.",
                        "If it's urgent and you do not know what to do, call 111 to get help near you."))
    }

    @Then("^I only see results for days that have available slots$")
    fun onlyAvailableSlotsAreDisplayed() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots.keys
        availableAppointments.assertThatOtherDatesAreNotDisplayed(expectedDates)
    }

    @Then("^I only see results for the selected filter options$")
    fun onlyAvailableSlotsAreDisplayedForFilterOptions() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
    }

    private fun availableSlotsAreDisplayedThatMeetTheNewCriteria() {
        val expectedSlots = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots
        assertTrue("Invalid test as there are no expected slots stored. ", expectedSlots.isNotEmpty())
        val expectedNumberOfSlots = expectedSlots.flatMap { it.value.toList() }.size
        for (date in expectedSlots.keys) {
            for (slotDetails in expectedSlots[date].orEmpty()) {
                availableAppointments.assertOnlyOneTimeSlotPresent(
                        slotDetails.dateAsUIString,
                        slotDetails.timeAsUIString,
                        slotDetails.sessionName
                )
            }
        }
        availableAppointments.assertNumberOfSlotsPresent(expectedNumberOfSlots)
    }
}
