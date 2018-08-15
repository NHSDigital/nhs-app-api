package features.more.steps

import net.thucydides.core.annotations.Step
import pages.MorePage
import pages.navigation.Header
import pages.navigation.NavBar

open class MoreSteps {

    lateinit var header: Header
    lateinit var navbar: NavBar
    lateinit var more: MorePage

    @Step
    fun isDisplayed(): Boolean {
        return header.isVisible("More") && navbar.isHighlighted(NavBar.NavBarType.MORE)
    }

    @Step
    fun setOrganDonationPreferences() {
        more.clickOrganDonations()
    }

    @Step
    fun setDataSharingPreferences() {
        more.clickDataSharing()
    }

    @Step
    fun organDonationDescriptionVisible(): Boolean {
        return more.isDonationDescriptionVisible()
    }

    @Step
    fun organDonationButtonVisible(): Boolean {
        return more.isDonationButtonVisible()
    }

    @Step
    fun dataSharingDescriptionVisible(): Boolean {
        return more.isDataSharingDescriptionVisible()
    }

    @Step
    fun dataSharingButtonVisible(): Boolean {
        return more.isDataSharingButtonVisible()
    }
}
