package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordTestResultDetailPage: HybridPageObject() {

    val serverErrorPageHeader = "Test result details data error"
    val serverErrorHeader = "There's been a problem getting details of your test results"
    val serverErrorMessage = "If the problem continues and you need this " +
            "information now, contact your GP surgery directly. " +
            "For urgent medical advice, call 111."

    val testResultDetailsHeader =
            HybridPageElement(
                    webDesktopLocator = "//h2[contains(text(),'Test result')]",
                    androidLocator = null,
                    page = this)

    val back = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'Back')]",
            androidLocator = "//button[contains(text(),'Back')]",
            page = this
    )

    fun clickBackToMyRecord() {
        back.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }
}