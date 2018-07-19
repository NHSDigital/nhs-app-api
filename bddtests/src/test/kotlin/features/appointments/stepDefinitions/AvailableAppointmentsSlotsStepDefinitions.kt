package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.*
import org.junit.Assert.*
import worker.models.appointments.AppointmentSlotsResponse
import javax.servlet.http.Cookie


class AvailableAppointmentsSlotsStepDefinitions {

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
        availableAppointments.generateStubsForAppointmentSlotsForSpecificDates()
    }

    @Given("^there are available appointment slots$")
    fun thereAreAvailableAppointmentSlots() {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAppointmentSlotsForNextFourWeeks()
    }

    @Given("^there are available appointment slots with different criteria for (.*)$")
    fun thereAreAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(
                if (gpSystem == "any GP System") "EMIS" else gpSystem
        )
    }

    @Given("^there are no available appointment slots for (.*)$")
    fun thereAreNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateNoAvailableAppointmentSlotsForGPSystem(
                if (gpSystem == "any GP System") "EMIS" else gpSystem
        )
    }

    @Given("^there is 1 available appointment slot for (.*)$")
    fun thereIsOneAvailableAppointmentSlotForGPSystem(gpSystem: String) {
        availableAppointments.generateDefaultUserData(gpSystem)
        availableAppointments.generateAvailableOneAppointmentSlotForGPSystem(
                if (gpSystem == "any GP System") "EMIS" else gpSystem
        )
    }

    @Given("^there are available appointment slots for (.*) for 1 location$")
    fun thereAreAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateAvailableAppointmentSlotsForGPSystemForOneLocation(
                if (gpSystem == "any GP System") "EMIS" else gpSystem
        )
    }

    @Given("^GP system doesn't respond a timely fashion for available appointment slots$")
    fun gp_system_doesn_t_respond_a_timely_fashion_for_available_appointment_slots() {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAppointmentSlotsForNextFourWeeks(delayedInSeconds = 30)
    }

    @Given("^there is a slight delay in retrieving them$")
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        availableAppointments.generateDefaultUserData()
        availableAppointments.generateEmisStubsForAppointmentSlotsForNextFourWeeks(delayedInSeconds = 1)
    }

    @When("^GP system responds a timely fashion for available appointment slots$")
    fun gp_system_responds_a_timely_fashion_for_available_appointment_slots() {
        availableAppointments.generateDefaultUserData()
        thereAreAvailableAppointmentSlots()
    }

    @Given("^GP system is unavailable for available appointment slots$")
    fun gp_system_is_unavailable_for_available_appointment_slots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWith(SC_INTERNAL_SERVER_ERROR, 0, {
                                andHtmlBody("Internal server Error")
                            })
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWith(SC_INTERNAL_SERVER_ERROR, 0, {
                                andHtmlBody("Internal server Error")
                            })
                }
    }

    @Given("^GP system returns corrupt data for appointment slots$")
    fun gp_system_returns_corrupt_data_for_appointment_slots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWith(SC_OK, 0, {
                                andHtmlBody("appointment slots metadata")
                            })
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWith(SC_OK, 0, {
                                andHtmlBody("appointment slots")
                            })
                }
    }


    @Given("^there are available appointment slots, but session has expired$")
    fun thereAreAvailableAppointmentSlotsButExpiredSession() {
        thereAreAvailableAppointmentSlots()
        Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
    }

    @Given("^the practice does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots() {
        availableAppointments.generateEmisStubsForAvailableSlotsWhenUnavailableToPatient()
    }

    @Given("^an unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {
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
//        availableSlotsAreDisplayedThatMeetTheNewCriteria()
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
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertEquals(2, result.slots.size)
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

    @Then("^I don't see a time-out error$")
    fun iDoNotSeeATimeOutError() {
        availableAppointments.checkTimeoutErrorMessage(false)
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
        availableAppointments.verifyThatAppropriateDateHeadingIsDisplayed()
        availableAppointments.verifyThatAppropriateTimeSlotIsDisplayed()
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