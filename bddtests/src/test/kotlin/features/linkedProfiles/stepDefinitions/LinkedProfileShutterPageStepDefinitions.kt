package features.linkedProfiles.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Then
import mockingFacade.linkedProfiles.LinkedProfileFacade
import pages.linkedProfiles.shutterPages.AppointmentsShutterPage
import pages.linkedProfiles.shutterPages.MedicalRecordShutterComponent
import pages.linkedProfiles.shutterPages.PrescriptionsShutterPage
import pages.linkedProfiles.shutterPages.MoreShutterPage
import pages.linkedProfiles.shutterPages.AdviceShutterPage
import pages.linkedProfiles.shutterPages.MessagesShutterPage
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail

class LinkedProfileShutterPageStepDefinitions {

    private lateinit var prescriptionsShutterPage: PrescriptionsShutterPage
    private lateinit var appointmentsShutterPage: AppointmentsShutterPage
    private lateinit var moreShutterPage: MoreShutterPage
    private lateinit var adviceShutterPage: AdviceShutterPage
    private lateinit var messagesShutterPage: MessagesShutterPage
    private lateinit var medicalRecordShutterComponent: MedicalRecordShutterComponent

    @Then("^the advice shutter page is displayed$")
    fun theAdviceShutterPageIsDisplayed() {
        adviceShutterPage.isLoaded()
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            adviceShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            adviceShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the more shutter page is displayed$")
    fun theMoreShutterPageIsDisplayed() {
        moreShutterPage.isLoaded()
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            moreShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            moreShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the prescriptions shutter page is displayed$")
    fun thePrescriptionsShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            prescriptionsShutterPage.isLoaded(selectedProfile.profile.formattedFullName())
            prescriptionsShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            prescriptionsShutterPage.isLoaded(selectedProfile.profile.name.firstName)
            prescriptionsShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the appointments shutter page is displayed$")
    fun theAppointmentsShutterPageIsDisplayed() {

        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE
                .getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            appointmentsShutterPage.isLoaded(selectedProfile.profile.formattedFullName())
            appointmentsShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            appointmentsShutterPage.isLoaded(selectedProfile.profile.name.firstName)
            appointmentsShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the medical record shutter page is displayed$")
    fun theMedicalRecordShutterPageIsDisplayed() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            medicalRecordShutterComponent.assertText(selectedProfile.profile,
                    selectedProfile.profile.formattedFullName())
        } else {
            medicalRecordShutterComponent.assertText(selectedProfile.profile,
                    selectedProfile.profile.name.firstName)
        }
    }

    @Then("^the messages shutter page is displayed$")
    fun theMessagesShutterPageIsDisplayed() {
        messagesShutterPage.isLoaded()
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        val gpSystem = SerenityHelpers.getGpSupplier()
        if (gpSystem === Supplier.TPP) {
            messagesShutterPage.assertText(selectedProfile.profile.formattedFullName())
        } else {
            messagesShutterPage.assertText(selectedProfile.profile.name.firstName)
        }
    }
}
