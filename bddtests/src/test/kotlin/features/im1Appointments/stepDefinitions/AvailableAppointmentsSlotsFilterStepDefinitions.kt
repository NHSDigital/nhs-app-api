package features.im1Appointments.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.im1Appointments.steps.AvailableAppointmentFilterSteps
import features.im1Appointments.steps.AvailableAppointmentFilterSteps.Companion.TODAY_OPTION
import features.im1Appointments.steps.AvailableAppointmentsSteps
import net.thucydides.core.annotations.Steps

class AvailableAppointmentsSlotsFilterStepDefinitions {

    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps
    @Steps
    lateinit var availableAppointmentsFilter: AvailableAppointmentFilterSteps

    @Given("^I have filtered such that there is one time displayed that represents multiple slots$")
    fun iSelectAOptionsFromTheFiltersThatIncludeTimesWhenMultipleSlotsExist() {
        availableAppointmentsFilter.selectOptionsToRevealSlots()
    }

    @When("^I select a type and location that have available slots$")
    fun iFilterTypeAndLocation() {
        availableAppointmentsFilter.selectFilterOptionsToRevealSlots()
    }

    @When("^I select a particular slot type and location$")
    fun iFilterType() {
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

    @Then("^I am able to filter on available slots$")
    fun iAmAbleToFilterOnSlots() {
        thereIsAFilterForAppointmentTypes()
        thereIsAFilterForLocations()
        thereIsAFilterForDoctorsNurses()
        thereIsAFilterForTimePeriod()
        availableAppointments.verifyThatNoSlotsAreDisplayed()
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
}
