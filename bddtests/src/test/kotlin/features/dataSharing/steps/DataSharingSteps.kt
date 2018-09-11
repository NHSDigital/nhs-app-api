package features.dataSharing.steps

import features.appointments.data.AppointmentsBookingData.Companion.mockingClient
import net.thucydides.core.annotations.Step
import pages.DataSharingPage
import pages.navigation.Header
import pages.navigation.NavBar

open class DataSharingSteps {

    lateinit var header: Header
    lateinit var navbar: NavBar
    lateinit var datasharing: DataSharingPage

    @Step
    fun isDisplayed(): Boolean {
        return header.isVisible("Sharing health data preferences") && navbar.isHighlighted(NavBar.NavBarType.MORE)
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
