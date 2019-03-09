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

    val backButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(),'Back')]",
            androidLocator = null,
            page = this
    )

    fun clickBackToMyRecordButton() {
        backButton.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }
}