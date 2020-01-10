package pages.myrecord

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.TextBlockElement
import pages.text

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

    fun assertContentWithNoWronglyDisplayedHTMLEntities(){
        val content = HybridPageElement(
                webDesktopLocator = "//div",
                page = this
        ).text

        val regex = """&(#?[0-9]+|x?[0-9A-Za-z]+);""".toRegex()
        Assert.assertFalse("HTML entity displayed incorrectly. " +
                "Regex found: '${regex.find(content)?.value}' ",
                regex.containsMatchIn(content))
    }

    fun assertContentMedicalRecordV2(){
        TextBlockElement.withoutHeader(this).assert("Test Result Detail")
    }
}
