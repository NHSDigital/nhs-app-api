package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.TextBlockElement

class MyRecordTestResultDetailPage: HybridPageObject() {

    val serverErrorPageHeader = "Test result details data error"
    val serverErrorHeader = "There's been a problem getting details of your test results"
    val serverErrorMessage = "If the problem continues and you need this " +
            "information now, contact your GP surgery directly. " +
            "For urgent medical advice, call 111."

    val back = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='main-back-button']",
            androidLocator = "//button[@data-purpose='main-back-button']",
            page = this
    ).withText("Back", false)

    fun assertContent(){
        TextBlockElement.withH2Header("Test result", this).assert("Test Result Detail")
    }

    fun clickBackToMyRecord() {
        back.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }
}
