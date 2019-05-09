package pages.nominatedPharmacy

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/nominated-pharmacy/check")
open class NominatedPharmacyCheckPage : HybridPageObject() {

    private val continueButtonLocator = "//button[contains(text(), " +
                                                        "'Continue')]"

    fun clickContinueToRepeatCoursesButton()
    {
        val continueButton = findByXpath(continueButtonLocator)
        continueButton.click()
    }
}
