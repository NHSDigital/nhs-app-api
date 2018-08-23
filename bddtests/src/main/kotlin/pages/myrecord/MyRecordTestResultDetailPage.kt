package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordTestResultDetailPage: HybridPageObject() {

    val serverErrorPageHeader = "Test result details data error"
    val serverErrorHeader = "There's been a problem getting details of your test results"
    val serverErrorMessage = "If the problem continues and you need this " +
            "information now, contact your GP surgery directly. " +
            "For urgent medical advice, call 111."

    val testResultDetailsHeader =
            HybridPageElement(
                    browserLocator = "//h2[contains(text(),'Test result')]",
                    androidLocator = null,
                    page = this)

    val backButton = HybridPageElement(
            browserLocator = "//button[contains(text(),'Back')]",
            androidLocator = null,
            page = this
    )

    fun clickBackToMyRecordButton() {
        backButton.element.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }
}