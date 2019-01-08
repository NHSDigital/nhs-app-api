package features.appointments.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.MyAppointmentsUISteps
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.navigation.HeaderNative

class MyAppointmentsStepDefinitions {

    @Steps
    lateinit var myAppointmentsUISteps: MyAppointmentsUISteps

    lateinit var headerNative: HeaderNative
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Then("^the Appointment Slot page is displayed$")
    fun theAppointmentSlotPageIsDisplayed() {
        headerNative.waitForPageHeaderText("Confirm appointment")
        appointmentsConfirmationSteps.checkAppointmentDetails()
    }

    @Then("^the Appointment Booking success message is displayed$")
    fun appointmentBookingSuccessMessage() {
        myAppointmentsUISteps.myAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(myAppointmentsUISteps.myAppointmentsPage.bookButton)
        myAppointmentsUISteps.checkBookingSuccessMessage()
    }

    @Then("^the booked appointment before cutoff time is correctly displayed with ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancel() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @Then("^booked appointments before and one appointment within cutoff time " +
            "are correctly displayed with relevant ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithCancelExceptOnesWithinCutoffTime() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment(1)
    }

    @Then("^the Appointment Booking success message is displayed without reference to being able to cancel$")
    fun appointmentBookingConfirmationScreenIsDisplayedWithoutReferenceToCancel() {
        myAppointmentsUISteps.checkBookingSuccessMessage(false)

    }

    @Then("^the booked appointment is correctly displayed without ability to cancel$")
    fun bookedAppointmentIsCorrectlyDisplayedWithoutCancel() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @Then("^I can book an appointment$")
    fun iCanBookAnAppointment() {
        myAppointmentsUISteps.myAppointmentsPage.bookButton.assertSingleElementPresent().assertIsVisible()
        myAppointmentsUISteps.myAppointmentsPage.bookButton.element.waitUntilPresent<WebElementFacade>()

        Assert.assertTrue("Book an appointment is not displaying",
                myAppointmentsUISteps.myAppointmentsPage.bookButton.element.isDisplayed)

        Assert.assertTrue("Book an appointment is not enabled",
                myAppointmentsUISteps.myAppointmentsPage.bookButton.element.isCurrentlyEnabled)
    }

    @Then("^the page title is \"My appointments\"$")
    fun thePageTitleIsMyAppointments() {
        myAppointmentsUISteps.checkHeaderTextIsCorrect()
    }

    @Then("^the My Appointments page is displayed$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        myAppointmentsUISteps.myAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(myAppointmentsUISteps.myAppointmentsPage.bookButton)
        iCanBookAnAppointment()
        thePageTitleIsMyAppointments()
    }

    @Then("^I am informed I have no upcoming appointments$")
    fun thenIAmInformedIHaveNoBookedAppointments() {
        myAppointmentsUISteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Then("^I am informed I have no historical appointments$")
    fun thenIAmInformedIHaveNoHistoricalAppointments() {
        myAppointmentsUISteps.checkNoHistoricalAppointmentsTextIsDisplaying()
    }

    @Then("^I am not informed I have no historical appointments$")
    fun thenIAmNotInformedIHaveNoHistoricalAppointments() {
        myAppointmentsUISteps.myAppointmentsPage.assertPastTextNotPresent()
    }

    @When("^I select \"([^\"]*)\" button$")
    fun whenISelectButton(buttonText: String) {
        myAppointmentsUISteps.myAppointmentsPage.locatorMethods.waitForNativeStepToComplete()
        myAppointmentsUISteps.myAppointmentsPage.clickOnButtonContainingText(buttonText)
    }

    @Then("^I am given the list of upcoming appointments$")
    fun thenIAmGivenTheListOfUpcomingAppointments() {
        myAppointmentsUISteps.checkUpcomingAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.checkIfUpcomingSlotsAreInCorrectOrder()
    }

    @Then("^I am given the list of historical appointments$")
    fun thenIAmGivenTheListOfHistoricalAppointments() {
        myAppointmentsUISteps.checkHistoricalAppointmentsAreCorrectlyPopulated()
        myAppointmentsUISteps.checkIfHistoricalSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        Assert.assertEquals(
                "Missing at least one cancel link. ",
                myAppointmentsUISteps.myAppointmentsPage.getWebAppointmentSlotDivs().size,
                myAppointmentsUISteps.myAppointmentsPage.getNumberOfCancelLinks()
        )
    }

    @Then("^no appointment can be cancelled$")
    fun noAppointmentCanBeCancelled() {
        myAppointmentsUISteps.verifyThatThereAreNoCancelLinks()
    }

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        myAppointmentsUISteps.myAppointmentsPage.clickFirstCancelAppointmentLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        myAppointmentsUISteps.verifyCancellationConfirmationMessage()
    }

    @Then("^I see page header indicating there is an appointment data error$")
    fun iSeePageHeaderIndicatingAppointmentDataError() {
        myAppointmentsUISteps.headerNative.waitForPageHeaderText("Appointment data error")
    }

    @Then("^I see the appropriate error messages for the appointment data error$")
    fun iSeeTheAppropriateErrorMessagesForTheAppointmentDataError() {
        myAppointmentsUISteps.checkAppointmentDataErrorMessagesAreCorrect()
    }
}
