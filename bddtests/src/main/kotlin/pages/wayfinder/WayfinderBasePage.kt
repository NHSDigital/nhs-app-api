package pages.wayfinder

import pages.HybridPageObject

const val TIME_TO_SLEEP_IN_MILLIS = 1000L

open class WayfinderBasePage : HybridPageObject() {
    fun clickExpanderLink(linkText:String) {
        //This wait has been added to ensure race condition does not occur on wayfinder pages
        Thread.sleep(TIME_TO_SLEEP_IN_MILLIS)
        clickOnExpanderLinkContainingText(linkText)
    }
}
