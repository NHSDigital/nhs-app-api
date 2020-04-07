package pages.myrecord

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent
import pages.text
import pages.withNormalisedText

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
        val title = "Test result"
        val body = "Test Result Detail"
        HybridPageElement(
                webDesktopLocator = "//div[h2[normalize-space(text())='$title']]//p[normalize-space(text())='$body']",
                page = this
        ).assertSingleElementPresent()
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
        HybridPageElement(
                webDesktopLocator = "//div/span/p",
                androidLocator = "//div/span/p",
                page = this).withNormalisedText("Test Result Detail").assertSingleElementPresent()
    }
}
