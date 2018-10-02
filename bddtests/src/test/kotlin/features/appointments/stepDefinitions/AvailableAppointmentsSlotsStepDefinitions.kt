package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.data.AppointmentsBookingData
import features.appointments.data.AppointmentsSlotsExample
import features.appointments.data.AppointmentsSlotsExampleBuilderWithExpectations
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentDateKey
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentTimeKey
import features.appointments.factories.AppointmentsSlotsFactory
import features.appointments.steps.AvailableAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.BaseStepDefinition
import features.sharedSteps.NavigationSteps
import features.sharedSteps.SerenityHelpers
import mocking.MockingClient
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie


class AvailableAppointmentsSlotsStepDefinitions : BaseStepDefinition() {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    val mockingClient = MockingClient.instance

    private val expiredCookie = Cookie(
            "Set-Cookie",
            "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJLpr5jIZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuGh3Dc9Dqe9jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0iFNsJTXJm5FZBVBeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR4tU-rHpdyz0JcbStgePRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A6zxTv1y0nwu8AAXcUg7EFlSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
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

    @Given("^there are available appointment slots with different criteria for EMIS when no appointment slot guidance is provided$")
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

    @Given("^EMIS doesn't respond a timely fashion for available appointment slots$")
    fun emis_doesn_t_respond_a_timely_fashion_for_available_appointment_slots() {
        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateExample() {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(90))
        }
    }

    @Given("^there are available EMIS appointment slots with different criteria but there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateExample() {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(1))
        }
    }

    @When("^EMIS responds a timely fashion for available appointment slots$")
    fun emis_responds_a_timely_fashion_for_available_appointment_slots() {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem("EMIS")
    }

    @Given("^EMIS is unavailable for available appointment slots$")
    fun emis_is_unavailable_for_available_appointment_slots() {
        val gpSystem = "EMIS"
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateDefaultUserData()
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWith(SC_INTERNAL_SERVER_ERROR) {
                        andHtmlBody("Internal server Error")
                    }
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(patient)
                    .respondWith(SC_INTERNAL_SERVER_ERROR) {
                        andHtmlBody("Internal server Error")
                    }
        }
    }

    @Given("^EMIS returns corrupt data for appointment slots$")
    fun emis_returns_corrupt_data_for_appointment_slots() {
        val gpSystem = "EMIS"
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateDefaultUserData()
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWith(SC_OK, 0) {
                        andHtmlBody("appointment slots metadata")
                    }
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(patient)
                    .respondWith(SC_OK, 0) {
                        andHtmlBody("appointment slots")
                    }
        }
    }


    @Given("^there are available EMIS appointment slots, but session has expired$")
    fun thereAreAvailableAppointmentSlotsButExpiredSession() {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem("EMIS")
        Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
    }

    @Given("^an unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateExample {
            respondWithUnknownException()
        }
    }

    @Given("^I have filtered such that there is one time displayed that represents multiple slots$")
    fun iSelectAOptionsFromTheFiltersThatIncludeTimesWhenMultipleSlotsExist() {
        availableAppointments.selectOptionsToRevealSlots()
    }

    @Given("^I have selected a time when multiple slots are available$")
    fun iSelectATimeWhenMultipleSlotsAreAvailable() {
        val date = sessionVariableCalled<String>(TargetAppointmentDateKey)
        val time = sessionVariableCalled<String>(TargetAppointmentTimeKey)
        availableAppointments.assertTimeSlotPresent(date, time)
        availableAppointments.assertOnlyOneTimeSlotPresent(date, time)
        availableAppointments.selectSlot(date, time)
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
        availableAppointments.verifyGuidanceIsDisplayed()
        availableAppointments.verifyGuidanceContentIsNotDisplayed()
        availableAppointments.verifyTheLabelIsCorrect()
        availableAppointments.expandAppointmentSlotGuidance()
    }

    @When("^I select a type and location that have available slots$")
    fun iFilterTypeAndLocation() {
        availableAppointments.selectFilterOptionsToRevealSlots()
    }

    @When("^I select time period for '(.*)'$")
    fun iFilterTimePeriod(timePeriod: String) {
        availableAppointments.selectTimePeriodOption(timePeriod)
    }

    @When("^I select an option from each of the filters$")
    fun iSelectAnOptionFromEachOfTheFilters() {
        availableAppointments.selectOptionsToRevealSlots()
    }

    @When("^I select options from the filters that don't yield any results$")
    fun iSelectAnOptionsFromTheFiltersThatDoNotYieldAnyResults() {
        availableAppointments.selectOptionsToRevealNoResults()
    }

    @Then("^available slots are returned for the given date-time range$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsAreReturned() {
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(
                AvailableAppointmentsSteps.AppointmentSessionVariableKeys.EXPECTED_APPOINTMENT_SESSIONS_KEY
        )
        val expectedAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
        for (appointmentSession in expectedAppointmentSessions) {
            expectedAppointmentSlots.addAll(appointmentSession.slots)
        }
        val actualResult = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull("Expected actualResult not null", actualResult)
        assertNotNull("Expected actualResult.slots not null", actualResult.slots)
        assertEquals("Incorrect number of slots. ", expectedAppointmentSlots.size, actualResult.slots.size)
    }

    @Then("^available slots are returned containing id, start date and time, end date and time, location, clinicians, type$")
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

    @Then("^I see appropriate information message for time-outs$")
    fun iSeeAppropriateInformationMessageAfterSecondsWhenItTimesOut() {
        availableAppointments.checkTimeoutErrorMessage()
    }

    @Then("^there should be a button to try again$")
    fun there_should_be_a_button_to_try_again() {
        availableAppointments.checkIfTryAgainButtonDisplayed()
    }

    @Then("^I see appropriate information message when there is a error retrieving data$")
    fun i_see_appropriate_information_message_when_there_is_a_error_retrieving_data() {
        availableAppointments.checkUnavailableErrorMessage()
    }

    @Then("^there should not be an option to try again$")
    fun there_should_not_be_an_option_to_try_again() {
        availableAppointments.checkIfTryAgainButtonIsNotDisplayed()
    }

    @When("^I click try again button on appointment page$")
    fun i_click_try_again_button_on_appointment_page() {
        availableAppointments.clickOnTryAgainButton()
    }

    @Then("^I am taken to the available appointment slots screen$")
    fun i_am_taken_to_the_available_appointment_slots_screen() {
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @Then("^there is a filter for the appointment types$")
    fun thereIsAFilterForAppointmentTypes() {
        availableAppointments.verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment locations$")
    fun thereIsAFilterForLocations() {
        availableAppointments.verifyThatLocationsFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment doctors/nurses$")
    fun thereIsAFilterForDoctorsNurses() {
        availableAppointments.verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated()
    }

    @Then("^there is a filter for the appointment time period$")
    fun thereIsAFilterForTimePeriod() {
        availableAppointments.verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated()
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
        availableAppointments.verifyThatTheFiltersAreNotDisplayed()
    }

    @Then("^appointment type is not selected$")
    fun appointmentTypeIsNotSelected() {
        availableAppointments.verifyThatNoAppointmentTypesIsSelected()
    }

    @Then("^the only location is selected$")
    fun theOnlyLocationIsSelected() {
        availableAppointments.verifyThatLocationIsSelected()
    }

    @Then("^options for doctors/nurses remains as \"no preference\"$")
    fun optionForClinicianRemainsAsNoPreference() {
        availableAppointments.verifyThatNoSpecificClinicianIsSelected()
    }

    @Then("^time period remains as that for this week$")
    fun timePeriodRemainsAsThatForThisWeek() {
        availableAppointments.verifyThatTimePeriodIsSetAsTheDefault()
    }

    @Then("^a message is displayed indicating there are no slots available$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotAvailable() {
        availableAppointments.verifyThatNoAppointmentsErrorIsDisplayed()
    }

    @Then("^a message is displayed indicating there are no slots for selected criteria$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotsForSelectedCriteria() {
        availableAppointments.verifyThatNoAppointmentsForSelectedCriteriaErrorIsDisplayed()
    }

    @Then("^a message is displayed indicating that the slot has already been taken$")
    fun aMessageIsDisplayedInformingTheSlotHasAlreadyBeenTaken() {
        availableAppointments.verifyThatSlotNoLongerAvailableMessageIsDisplayed()
    }

    @Then("^available slots are displayed that meet the new criteria$")
    fun availableSlotsAreDisplayedThatMeetTheNewCriteria() {
        val expectedDatesAndTimes = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots
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
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatOtherDatesAreNotDisplayed(expectedDates)
    }

    @Then("^I see results for each of the remaining days for this week, with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForThisWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates, AppointmentsSlotsExample.remainingDatesForThisWeek)
    }

    @Then("^I see results for each of the days for next week, with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForNextWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates, AppointmentsSlotsExample.datesForNextWeek)
    }

    @Then("^I see a timeout on the appointment booking page$")
    fun iSeeATimeOutOnTheAppointmentBookingPage() {
        availableAppointments.waitForSpinnerToDisappearBecauseOfTimeout()
        availableAppointments.checkIfTryAgainButtonDisplayed()
    }

    @Then("^the appointment slot guidance content is displayed$")
    fun appointmentSlotGuidanceContentIsDisplayed() {
        availableAppointments.verifyThatAppointmentGuidanceContentIsDisplayed()
    }

    @Then("^the appointment slot guidance is collapsible$")
    fun appointmentSlotGuidanceIsCollapsible() {
        availableAppointments.collapseAppointmentSlotGuidance()
        iExpandTheAppointmentSlotGuidance()
    }

    @Then("^I cannot see any appointment slot guidance$")
    fun iCannotSeeAnyAppointmentSlotGuidance() {
        availableAppointments.verifyThatAppointmentGuidanceIsNotDisplayedAtAll()
    }
}
