package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentDateKey
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentTimeKey
import features.appointments.factories.AppointmentsSlotsFactory
import features.appointments.steps.AvailableAppointmentFilterSteps
import features.appointments.steps.AvailableAppointmentFilterSteps.Companion.ALL_OPTION
import features.appointments.steps.AvailableAppointmentFilterSteps.Companion.TODAY_OPTION
import features.appointments.steps.AvailableAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.AppointmentsBookingData
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
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
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 90L

class AvailableAppointmentsSlotsStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps
    @Steps
    lateinit var availableAppointmentsFilter: AvailableAppointmentFilterSteps

    val mockingClient = MockingClient.instance

    private val expiredCookie = Cookie(
            "Set-Cookie",
            "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJLpr5j" +
                    "IZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuGh3Dc9Dqe9" +
                    "jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0iFNsJTXJm5FZBV" +
                    "BeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR4tU-rHpdyz0JcbStge" +
                    "PRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A6zxTv1y0nwu8AAXcUg7EF" +
                    "lSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
    )

    @Given("^there are available (.*) appointment slots for an explicit date-time range$")
    fun thereAreAvailableAppointmentSlotsForAnExplicitDateTimeRange(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample(
                AppointmentsBookingData.defaultSessionStartDateRaw,
                AppointmentsBookingData.defaultSessionEndDateRaw)
    }

    @Given("^there are available (.*) appointment slots$")
    fun thereAreAvailableAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
    }

    @Given("^there are available appointment slots with different criteria for (\\w+)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample()
    }

    @Given("^there are available appointment slots with different criteria for EMIS " +
            "when no appointment slot guidance is provided$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForEmisWithNoGuidance() {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample(guidanceMessage = false)
    }

    @Given("^there are available appointment slots with different criteria for EMIS when guidance cannot be retrieved$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForEmisWhenGuidanceCannotBeRetrieved() {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExampleWithoutBeingAbleToAccessGuidanceMessage()
    }

    @Given("^there are no available appointment slots for (.*)$")
    fun thereAreNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample(AppointmentsSlotsExampleBuilderWithExpectations().build())
    }

    @Given("^there is 1 available appointment slot for (.*)$")
    fun thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample(AppointmentsSlotsExample.singleSlotExample())
    }

    @Given("^there are available appointment slots for (.*) for 1 location$")
    fun thereAreAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample(AppointmentsSlotsExample.multipleSlotsOneLocation())
    }

    @Given("^there are appointment slots on some days other than tomorrow, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsButNotForTomorrowForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = AppointmentsSlotsExample.slotForDayAfterTomorrow()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days this week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysThisWeekButNotAllForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = AppointmentsSlotsExample.slotForEndOfToday()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days next week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysNextWeekButNotAllForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = AppointmentsSlotsExample.slotForThisTimeNextWeek()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days in the next few weeks but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsInTheNextFewWeeksForGPSystem(gpSystem: String) {
        thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem)
    }

    @Given("^the (.*) doesn't respond a timely fashion for available appointment slots$")
    fun theGpSystemDoesntRespondATimelyFashionForAvailableAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                    .respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
        }
    }

    @Given("^there are available EMIS appointment slots with different criteria " +
            "but there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateExample {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(1))
        }
    }

    @When("^(.*) responds a timely fashion for available appointment slots$")
    fun respondsATimelyFashionForAvailableAppointmentSlots(gpSystem: String) {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem)
    }

    @Given("^Appointments are disabled for VISION at a GP Practice level")
    fun appointmentsAreDisabledForVisionAtAGPLevel() {
        Serenity.setSessionVariable(gpAppointmentsDisabled).to("true")
    }


    @Given("^(.*) is unavailable for available appointment slots$")
    fun gpSystemIUnavailableForAvailableAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateServiceUnavailableSlotResponse()
    }

    @Given("^(.*) returns corrupt data for appointment slots$")
    fun gpSystemReturnsCorruptDataForAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateCorruptedSlotResponse()
    }


    @Given("^there are available EMIS appointment slots, but session has expired$")
    fun thereAreAvailableAppointmentSlotsButExpiredSession() {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem("EMIS")
        Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
    }

    @Given("^an unknown exception will occur when wanting to view (.*) appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithUnknownException()
        }
    }

    @Given("^I have filtered such that there is one time displayed that represents multiple slots$")
    fun iSelectAOptionsFromTheFiltersThatIncludeTimesWhenMultipleSlotsExist() {
        availableAppointmentsFilter.selectOptionsToRevealSlots()
    }

    @Given("^I have selected a time when multiple slots are available$")
    fun iSelectATimeWhenMultipleSlotsAreAvailable() {
        val date = sessionVariableCalled<String>(TargetAppointmentDateKey)
        val time = sessionVariableCalled<String>(TargetAppointmentTimeKey)
        availableAppointments.assertTimeSlotPresent(date, time)
        availableAppointments.assertOnlyOneTimeSlotPresent(date, time)
        availableAppointments.availableAppointmentsPage.selectSlot(date, time)
    }

    @When("^the available appointment slots are retrieved$")
    fun theAvailableAppointmentSlotsAreRetrieved() {
        availableAppointments.theAvailableAppointmentSlotsAreRetrieved()
    }

    @When("^the available appointment slots are retrieved for explicit date-time range$")
    fun theAvailableAppointmentSlotsAreRetrievedForExplicitDateTimeRange() {
        availableAppointments.theAvailableAppointmentSlotsAreRetrievedForExplicitDateTimeRange()
    }

    @When("^I acknowledge that there are no appointments and go back to my appointments$")
    fun iAcknowledgeThatThereAreNoAppointmentsAndGoBackToMyAppointments() {
        availableAppointments.clickOnBackButton()
    }

    @When("^I decide I don't want to select an appointment and go back$")
    fun iDecideIDoNotWantToSelectAnAppointmentAndGoBack() {
        availableAppointments.clickOnBackButton()
    }

    @When("^I expand the appointment slot guidance$")
    fun iExpandTheAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.appointmentSlotGuidance.assertIsVisible()
        availableAppointments.availableAppointmentsPage.guidance.content.assertElementNotPresent()
        assertEquals("Appointment guidance help text is incorrect. ",
                "Which type of appointment do I need?",
                availableAppointments.availableAppointmentsPage.guidance.label.element.text)
        availableAppointments.availableAppointmentsPage.guidance.expand.element.click()
    }

    @When("^I select a type and location that have available slots$")
    fun iFilterTypeAndLocation() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
    }

    @When("^I select time period for '(.*)'$")
    fun iFilterTimePeriod(timePeriod: String) {
        availableAppointments.availableAppointmentsPage.timePeriodFilter.selectByText(timePeriod)
    }

    @When("^I select an option from each of the filters$")
    fun iSelectAnOptionFromEachOfTheFilters() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
        availableAppointments.availableAppointmentsPage.timePeriodFilter.selectByText(
                ALL_OPTION)
    }

    @When("^I select options from the filters that don't yield any results$")
    fun iSelectAnOptionsFromTheFiltersThatDoNotYieldAnyResults() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
        availableAppointments.availableAppointmentsPage.timePeriodFilter.selectByText(
                TODAY_OPTION)
    }

    @Then("^available slots are returned for the given date-time range$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsAreReturned() {
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

    @Then("^I get a response with an empty set of slots$")
    fun emptySetsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertEquals(0, result.slots.size)
    }

    @Then("^I am taken to the available appointment slots screen$")
    fun i_am_taken_to_the_available_appointment_slots_screen() {
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @Then("^there is a filter for the appointment types$")
    fun thereIsAFilterForAppointmentTypes() {
        availableAppointmentsFilter.verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment locations$")
    fun thereIsAFilterForLocations() {
        availableAppointmentsFilter.verifyThatLocationsFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment doctors/nurses$")
    fun thereIsAFilterForDoctorsNurses() {
        availableAppointmentsFilter.verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment time period$")
    fun thereIsAFilterForTimePeriod() {
        availableAppointmentsFilter.verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^no available slots are displayed$")
    fun noAvailableSlotsAreDisplayed() {
        availableAppointments.verifyThatNoSlotsAreDisplayed()
    }

    @Then("^I am able to filter on available slots$")
    fun iAmAbleToFilterOnSlots() {
        thereIsAFilterForAppointmentTypes()
        thereIsAFilterForLocations()
        thereIsAFilterForDoctorsNurses()
        thereIsAFilterForTimePeriod()
        noAvailableSlotsAreDisplayed()
    }

    @Then("^I don't see filters for available slots$")
    fun iDoNotSeeFiltersForAvailableSlots() {
        availableAppointmentsFilter.verifyThatTheFiltersAreNotDisplayed()
    }

    @Then("^appointment type is not selected$")
    fun appointmentTypeIsNotSelected() {
        availableAppointmentsFilter.verifyThatNoAppointmentTypesIsSelected()
    }

    @Then("^the only location is selected$")
    fun theOnlyLocationIsSelected() {
        availableAppointmentsFilter.verifyThatLocationIsSelected()
    }

    @Then("^options for doctors/nurses remains as \"no preference\"$")
    fun optionForClinicianRemainsAsNoPreference() {
        availableAppointmentsFilter.verifyThatNoSpecificClinicianIsSelected()
    }

    @Then("^time period remains as that for this week$")
    fun timePeriodRemainsAsThatForThisWeek() {
        availableAppointmentsFilter.verifyThatTimePeriodIsSetAsTheDefault()
    }

    @Then("^a message is displayed indicating there are no slots available$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotAvailable() {
        availableAppointments.verifyThatNoAppointmentsErrorIsDisplayed()
    }

    @Then("^a message is displayed indicating there are no slots for selected criteria$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotsForSelectedCriteria() {
        availableAppointments.verifyThatNoAppointmentsForSelectedCriteriaErrorIsDisplayed()
    }

    @Then("^available slots are displayed that meet the new criteria$")
    fun availableSlotsAreDisplayedThatMeetTheNewCriteria() {
        val expectedDatesAndTimes =
                sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots
        assertTrue("Invalid test as there are no expected slots stored. ", expectedDatesAndTimes.isNotEmpty())
        val expectedNumberOfSlots = expectedDatesAndTimes.flatMap { it.value.toList() }.size
        for (date in expectedDatesAndTimes.keys) {
            for (time in expectedDatesAndTimes[date].orEmpty()) {
                availableAppointments.assertOnlyOneTimeSlotPresent(date, time)
            }
        }
        availableAppointments.assertNumberOfSlotsPresent(expectedNumberOfSlots)
    }

    @Then("^I only see results for days that have available slots$")
    fun onlyAvailableSlotsAreDisplayed() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates =
                sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatOtherDatesAreNotDisplayed(expectedDates)
    }

    @Then("^I see results for each of the remaining days for this week, " +
            "with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForThisWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates =
                sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates,
                AppointmentsSlotsExample.remainingDatesForThisWeek)
    }

    @Then("^I see results for each of the days for next week, with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForNextWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates =
                sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates,
                AppointmentsSlotsExample.datesForNextWeek)
    }

    @Then("^the appointment slot guidance content is displayed$")
    fun appointmentSlotGuidanceContentIsDisplayed() {
        availableAppointments.verifyThatAppointmentGuidanceContentIsDisplayed()
    }

    @Then("^the appointment slot guidance is collapsible$")
    fun appointmentSlotGuidanceIsCollapsible() {
        availableAppointments.  availableAppointmentsPage.guidance.collapse.element.click()
        iExpandTheAppointmentSlotGuidance()
    }

    @Then("^I cannot see any appointment slot guidance$")
    fun iCannotSeeAnyAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.content.assertElementNotPresent()
    }
}
