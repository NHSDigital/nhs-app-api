package features.dataSharing.steps

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
    fun clickOverviewLink() {
        datasharing.clickOverview()
    }

    @Step
    fun clickManageYourChoiceLink() {
        datasharing.clickManageYourChoice()
    }

    @Step
    fun dataSharingCompleteButtonVisible(): Boolean {
        return datasharing.isStartNowVisible()
    }

    @Step
    fun clickStartNowButton() {
        return datasharing.clickStartNow()
    }

    @Step
    fun isOverviewTitleVisible(): Boolean {
        return datasharing.onOverviewPage()
    }

    @Step
    fun isManageYourChoiceTitleVisible(): Boolean {
        return datasharing.onManageYourChoicePage()
    }

    @Step
    fun isNdopTestTextIsVisible(): Boolean {
        return datasharing.isNdopTestTextIsVisible()
    }
}
