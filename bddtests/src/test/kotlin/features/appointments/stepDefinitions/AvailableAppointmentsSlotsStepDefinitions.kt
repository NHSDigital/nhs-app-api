package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.stepDefinitions.factories.AppointmentsBookingFactory
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.AvailableAppointmentsSteps.Companion.EXPECTED_APPOINTMENT_SESSIONS_KEY
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.BaseStepDefinition
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import pages.appointments.AvailableAppointmentsPage
import worker.models.appointments.AppointmentSlotsResponse
import javax.servlet.http.Cookie


class AvailableAppointmentsSlotsStepDefinitions : BaseStepDefinition() {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    private val expiredCookie = Cookie(
            "Set-Cookie",
            "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJLpr5jIZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuGh3Dc9Dqe9jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0iFNsJTXJm5FZBVBeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR4tU-rHpdyz0JcbStgePRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A6zxTv1y0nwu8AAXcUg7EFlSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
    )

    @Given("^there are available appointment slots for an explicit date-time range$")
    fun thereAreAvailableAppointmentSlotsForAnExplicitDateTimeRange() {
        currentProvider = ProviderTypes.valueOf(sessionVariableCalled<String>(GLOBAL_PROVIDER_TYPE))
        when (currentProvider) {
            ProviderTypes.EMIS -> {
                availableAppointments.generateEmisStubsForAppointmentSlotsForSpecificDates()
            }
            ProviderTypes.TPP -> {
                availableAppointments.generateTppStubsForAppointmentSlotsForSpecificDates()
            }
        }
    }

    @Given("^there are available appointment slots with different criteria for (.*)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        val factory = AppointmentsBookingFactory.getForSupplier(gpSystem)
        factory.generateDefaultAvailableAppointmentSlotExample()
    }

    @Given("^there are no available appointment slots for (.*)$")
    fun thereAreNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        availableAppointments.generateDefaultUserData(gpSystem)
        availableAppointments.generateNoAvailableAppointmentSlotsForGPSystem(gpSystem)
    }

    @Given("^there is 1 available appointment slot for (.*)$")
    fun thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem: String) {
        availableAppointments.generateDefaultUserData(gpSystem)
        availableAppointments.generateAvailableOneAppointmentSlotForGPSystem(gpSystem)
    }

    @Given("^there are available appointment slots for (.*) for 1 location$")
    fun thereAreAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        availableAppointments.generateDefaultUserData(gpSystem)
        availableAppointments.generateAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem)
    }

    @Given("^EMIS doesn't respond a timely fashion for available appointment slots$")
    fun emis_doesn_t_respond_a_timely_fashion_for_available_appointment_slots() {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAppointmentSlotsForNextFourWeeks(delayedInSeconds = 30)
    }

    @Given("^there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAppointmentSlotsForNextFourWeeks(delayedInSeconds = 1)
    }

    @When("^EMIS responds a timely fashion for available appointment slots$")
     fun emis_responds_a_timely_fashion_for_available_appointment_slots() {
        thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem("EMIS")
    }

    @Given("^EMIS is unavailable for available appointment slots$")
    fun emis_is_unavailable_for_available_appointment_slots() {
        availableAppointments.generateDefaultUserData()
        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWith(SC_INTERNAL_SERVER_ERROR, 0) {
                        andHtmlBody("Internal server Error")
                    }
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(patient)
                    .respondWith(SC_INTERNAL_SERVER_ERROR, 0) {
                        andHtmlBody("Internal server Error")
                    }
        }
    }

    @Given("^EMIS returns corrupt data for appointment slots$")
    fun emis_returns_corrupt_data_for_appointment_slots() {
        availableAppointments.generateDefaultUserData()
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
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAvailableSlotsGivingUnknownException()
    }

    @When("^I click on the (.*) appointment$")
    fun iClickOnXTheAppointment(position: String) {
        if (position == "same") {
            availableAppointments.clickOnASlot()
        } else {
            val regex = Regex("[a-z]+")
            val positionNumberAsString = position.split(regex)[0]
            availableAppointments.clickOnASlot(positionNumberAsString.toInt())
        }
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

    @When("^I try to progress without selecting (.*)$")
    fun iTryToProgressWithoutSelecting(optionsToAvoid: String) {
        if (!optionsToAvoid.contains("appointment type")) {
            availableAppointments.selectAnAppointmentType()
        }
        if (!optionsToAvoid.contains("location")) {
            availableAppointments.selectALocation()
        }

        availableAppointments.clickOnBookAppointmentButton()
    }

    @When("^I filter to reveal multiple slots$")
    fun iFilterToReviewMultipleSlots() {
        iSelectAnOptionFromEachOfTheFilters()
        availableSlotsAreDisplayedThatMeetTheNewCriteria()
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
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)
        val expectedAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
        for (appointmentSession in expectedAppointmentSessions) {
            expectedAppointmentSlots.addAll(appointmentSession.slots)
        }
        val actualResult = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(actualResult)
        assertNotNull(actualResult.slots)
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

    @Then("^I see an appropriate message informing that I need to select an appointment type and location$")
    fun aMessageIsDisplayedInformingThatINeedToSelectAnAppointmentTypeAndLocation() {
        availableAppointments.verifyThatValidationErrorsForMissingTypeAndLocationAreDisplayed()
    }

    @Then("^I see an appropriate message informing that I need to select an appointment type$")
    fun aMessageIsDisplayedInformingThatINeedToSelectAnAppointmentType() {
        availableAppointments.verifyThatValidationErrorsForMissingTypeIsDisplayed()
    }

    @Then("^I see an appropriate message informing that I need to select a location$")
    fun aMessageIsDisplayedInformingThatINeedToSelectALocation() {
        availableAppointments.verifyThatValidationErrorsForMissingLocationIsDisplayed()
    }

    @Then("^I see an appropriate message informing that I need to select an appointment$")
    fun aMessageIsDisplayedInformingThatINeedToSelectAnAppointment() {
        availableAppointments.verifyThatValidationErrorsForNoSelectedAppointmentIsDisplayed()
    }

    @Then("^a message is displayed indicating that the slot has already been taken$")
    fun aMessageIsDisplayedInformingTheSlotHasAlreadyBeenTaken() {
        availableAppointments.verifyThatSlotNoLongerAvailableMessageIsDisplayed()
    }

    @Then("^available slots are displayed that meet the new criteria$")
    fun availableSlotsAreDisplayedThatMeetTheNewCriteria() {

        var expectedDateHeading = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.TargetAppointmentDateKey)
        var expectedTimeSlot = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.TargetAppointmentTimeKey)
        availableAppointments.assertTimeSlotPresent(expectedDateHeading, expectedTimeSlot)

    }

    @Then("^the 2nd slot is highlighted$")
    fun theNewSlotIsHighlighted() {
        availableAppointments.verifyThatDifferentSlotIsHighlighted()
    }

    @Then("^the 1st slot is no longer highlighted$")
    fun theFirstSlotIsNotHighlighted() {
        availableAppointments.verifyThatFirstSlotIsNotHighlighted()
    }

    @Then("^the slot remains highlighted$")
    fun theSlotIsStillHighlighted() {
        availableAppointments.verifyThatSlotIsStillHighlighted()
    }
}