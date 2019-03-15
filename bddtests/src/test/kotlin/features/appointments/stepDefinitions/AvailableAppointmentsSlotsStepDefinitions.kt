package features.appointments.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsSlotsFactory
import features.appointments.steps.AvailableAppointmentFilterSteps
import features.appointments.steps.AvailableAppointmentFilterSteps.Companion.TODAY_OPTION
import features.appointments.steps.AvailableAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.data.appointments.FilterSlotDetails
import mocking.vision.VisionConstants.gpAppointmentsDisabled
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.SC_OK
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import pages.assertIsVisible
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 90L

@Suppress("LargeClass", "Do not duplicate this suppression in other classes, " +
        "if possible, break down steps into functional areas")
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
    private val appointmentSlotsExample = AppointmentsSlotsExample()

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
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample()
    }

    @Given("^there are available appointment slots with different criteria for (.*) " +
            "when (.*) appointment slot guidance is provided$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaWithCustomGuidance(
            gpSystem: String, guidanceMessageDescription: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)

        val guidanceMessage = when (guidanceMessageDescription) {
            "empty" -> ""
            "whitespace string" -> "   "
            else -> guidanceMessageDescription
        }
        appointmentsSlotsFactory.generateDefaultAvailableAppointmentSlotExample(guidanceMessage = guidanceMessage)
    }

    @Given("^there are available appointment slots with different criteria for (.*) when guidance cannot be retrieved$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForEmisWhenGuidanceCannotBeRetrieved(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
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
        factory.generateExample(appointmentSlotsExample.singleSlotExample())
    }

    @Given("^there are available appointment slots for (.*) for 1 location$")
    fun thereAreAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample(appointmentSlotsExample.multipleSlotsOneLocation())
    }

    @Given("^there are appointment slots on some days other than tomorrow, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsButNotForTomorrowForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = appointmentSlotsExample.slotForDayAfterTomorrow()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days this week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysThisWeekButNotAllForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = appointmentSlotsExample.slotForEndOfToday()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days next week but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsOnSomeDaysNextWeekButNotAllForGPSystem(gpSystem: String) {
        val appointmentsSlotsFactory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val example = appointmentSlotsExample.slotForThisTimeNextWeek()
        appointmentsSlotsFactory.generateExample(example)
    }

    @Given("^there are appointment slots on some days in the next few weeks but not others, provided by (.*)$")
    fun thereAreAvailableAppointmentSlotsInTheNextFewWeeksForGPSystem(gpSystem: String) {
        thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem)
    }

    @Given("^the (.*) doesn't respond in a timely fashion for available appointment slots$")
    fun theGpSystemDoesntRespondInATimelyFashionForAvailableAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val genericExample = appointmentSlotsExample.getGenericExample()
        factory.generateExample {
            withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                    .respondWithSuccess(genericExample)
        }
    }

    @Given("^the (.*) doesn't respond in a timely fashion for available appointment slots, on the first attempt$")
    fun theGpSystemDoesntRespondInATimelyFashionForAvailableAppointmentSlotsOnTheFirstAttempt(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val genericExample = appointmentSlotsExample.getGenericExample()
        // stub to generate timeout for 1st attempt
        factory.generateExample {
            withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                    .respondWithSuccess(genericExample)
                    .inScenario(timeoutScenario)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(willSucceed)
        }
    }

    @Given("^will respond in a timely fashion on the second attempt$")
    fun theGpSystemWillSucceedForAvailableAppointmentSlotsOnTheSecondAttempt() {
        val gpSystem: String = SerenityHelpers.getValueOrNull(SupplierSpecificFactory.SerenityKey.GP_SYSTEM) ?: ""
        Assert.assertNotEquals("Cannot determine GP system being used. ", "", gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val genericExample = appointmentSlotsExample.getGenericExample()
        // stub to generate success on 2nd attempt
        factory.generateExample {
            respondWithSuccess(genericExample)
                    .inScenario(timeoutScenario)
                    .whenScenarioStateIs(willSucceed)
        }
    }

    @Given("^there are available EMIS appointment slots with different criteria " +
            "but there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        val genericExample = appointmentSlotsExample.getGenericExample()
        factory.generateExample {
            respondWithSuccess(genericExample)
                    .delayedBy(Duration.ofSeconds(1))
        }
    }

    @Given("^Appointments are disabled for VISION at a GP Practice level")
    fun appointmentsAreDisabledForVisionAtAGPLevel() {
        Serenity.setSessionVariable(gpAppointmentsDisabled).to("true")
        AppointmentsSlotsFactory.getForSupplier("VISION").generateDefaultUserData()
    }


    @Given("^(.*) is unavailable for available appointment slots$")
    fun gpSystemIUnavailableForAvailableAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithGPServiceUnavailableException()
        }
    }

    @Given("^(.*) returns corrupt data for appointment slots$")
    fun gpSystemReturnsCorruptDataForAppointmentSlots(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithCorrupted()
        }
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

    @When("^I expand the appointment slot guidance$")
    fun iExpandTheAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.assertLabel("Which type of appointment do I need?")
        availableAppointments.availableAppointmentsPage.guidance.expand()
    }

    @When("^I select a type and location that have available slots$")
    fun iFilterTypeAndLocation() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
    }

    @When("^I select time period for '(.*)'$")
    fun iFilterTimePeriod(timePeriod: String) {
        availableAppointments.availableAppointmentsPage.timePeriodFilter.selectByText(timePeriod)
    }

    @When("^I select options from the filters that don't yield any results$")
    fun iSelectAnOptionsFromTheFiltersThatDoNotYieldAnyResults() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
        availableAppointments.availableAppointmentsPage.timePeriodFilter.selectByText(
                TODAY_OPTION)
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
        availableAppointmentsFilter.availableAppointmentsPage.waitForSpinnerToDisappear()
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
        availableAppointments.availableAppointmentsPage.warning().assertVisible(
                arrayListOf("No appointments available",
                "There are currently no appointments available to book online right now. " +
                "If you need to book one now, call your GP surgery.",
                "If it's urgent and you don't know what to do, call 111 to get help near you."))
    }

    @Then("^a message is displayed indicating there are no slots for selected criteria$")
    fun aMessageIsDisplayedIndicatingThereAreNoSlotsForSelectedCriteria() {
        availableAppointments.availableAppointmentsPage.warning("No appointments available").assertVisible(
                arrayListOf(
                        "Try selecting a different date and time, or without a preferred practice member selected. " +
                                "If you can't find the appointment you need, call your GP surgery.",
                        "If it's urgent and you don't know what to do, call 111 to get help near you."))
    }

    @Then("^I only see results for days that have available slots$")
    fun onlyAvailableSlotsAreDisplayed() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots.keys
        availableAppointments.assertThatOtherDatesAreNotDisplayed(expectedDates)
    }

    @Then("^I see results for each of the remaining days for this week, " +
            "with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForThisWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(
                expectedDates,
                appointmentSlotsExample.remainingDatesForThisWeek
        )
    }

    @Then("^I see results for each of the days for next week, with an appropriate message when there are no slots$")
    fun iSeeResultsForEachOfTheRemainingDaysForNextWeek() {
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
        val expectedDates = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots.keys
        availableAppointments.assertThatRemainingDaysAreDisplayedWithAppropriateMessage(
                expectedDates,
                appointmentSlotsExample.datesForNextWeek
        )
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

    @Then("^the appointment slot guidance content is displayed$")
    fun appointmentSlotGuidanceContentIsDisplayed() {
        val expectedGuidanceContent =
                sessionVariableCalled<String>(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)

        availableAppointments.availableAppointmentsPage.guidance.assertContent(expectedGuidanceContent)
    }

    @Then("^the appointment slot guidance is collapsible$")
    fun appointmentSlotGuidanceIsCollapsible() {
        availableAppointments.availableAppointmentsPage.guidance.collapse()
        iExpandTheAppointmentSlotGuidance()
    }

    @Then("^I cannot see any appointment slot guidance$")
    fun iCannotSeeAnyAppointmentSlotGuidance() {
        availableAppointments.availableAppointmentsPage.guidance.assertNotPresent()
    }

    companion object {
        const val timeoutScenario = "timeout scenario"
        const val willSucceed = "to succeed"
    }
}
