package features.dataSharing.steps

import features.appointments.data.AppointmentsBookingData.Companion.mockingClient
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.DataSharingPage
import pages.navigation.Header
import pages.navigation.NavBar

open class DataSharingSteps {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    lateinit var header: Header
    lateinit var datasharing: DataSharingPage

    @Step
    fun assertIsDisplayed() {
        header.assertIsVisible("Sharing health data preferences")
        navbarSteps.assertSelectedTab("More")
    }

    @Step
    fun clickNextButton() {
        datasharing.clickNext()
    }

    @Step
    fun clickPreviousButton() {
        datasharing.clickPrevious()
    }

    @Step
    fun clickManageYourChoiceLink() {
        datasharing.clickManageYourChoice()
    }

    @Step
    fun clickDataSharingMoreInfoLink() {
        datasharing.clickDataSharingMoreInfoLink()
    }

    @Step
    fun clickStartNowButton() {
        mockingClient.forNdop {
            postTokenToNdop()
                    .respondWithNdopMockPage()
        }
        return datasharing.clickStartNow()
    }

    @Step
    fun isOverviewTitleVisible(): Boolean {
        return datasharing.onOverviewPage()
    }

    @Step
    fun isBenefitsTitleVisible(): Boolean {
        return datasharing.onBenefitsPage()
    }

    @Step
    fun isDataUseTitleVisible(): Boolean {
        return datasharing.onDataUsePage()
    }

    @Step
    fun isWhereOptOutDoesntApplyTitleVisible(): Boolean {
        return datasharing.onWhereOptOutDoesntApplyPage()
    }

    @Step
    fun isManageYourChoiceTitleVisible(): Boolean {
        return datasharing.onManageYourChoicePage()
    }

}
