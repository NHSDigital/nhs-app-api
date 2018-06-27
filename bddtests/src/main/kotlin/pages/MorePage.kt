package pages

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/more")
open class MorePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(xpath = "//a[contains(text(),'Set organ donation preferences')]")
    lateinit var btnOrganDonation: WebElementFacade

    @FindBy(xpath = "//h2[contains(text(),'Organ donation preferences')]")
    lateinit var donationHeading: WebElementFacade

    @FindBy(xpath = "//p[contains(text(),'Help us save thousands of lives in the UK every year by signing up to become an organ donor. Register your decision and choose what you want to donate on the NHS Organ Donor Register.')]")
    lateinit var donationDescription: WebElementFacade

    fun clickOrganDonations() {
        findByXpath("//*[@id='btn_organdonation']").click()
    }

    fun isDonationHeaderVisible(): Boolean {
        return donationHeading.isCurrentlyEnabled
    }

    fun isDonationDescriptionVisible(): Boolean {
        return donationDescription.isCurrentlyEnabled
    }

    fun isDonationButtonVisible(): Boolean {
        return btnOrganDonation.isCurrentlyEnabled
    }
}