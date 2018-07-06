package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement
import pages.navigation.Header

class MyRecordWarningPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val lblHeader = HybridPageElement(
            browserLocator = "//*[@id='app']/header/h1",
            androidLocator = null,
            page = this
    )

    val txtWarning = HybridPageElement(
            browserLocator = "//div[@class='msg warning']",
            androidLocator = null,
            page = this
    )

    val btnAgree = HybridPageElement(
            browserLocator = "//button[contains(text(),'Agree and continue')]",
            androidLocator = null,
            page = this
    )

    val btnBack2Home = HybridPageElement(
            browserLocator = "//button[contains(text(),'Back to home')]",
            androidLocator = null,
            page = this
    )


    fun isBackToHomePresent(): Boolean {
        return btnBack2Home.element.isVisible
    }

    fun isAgreePresent(): Boolean {
        return btnAgree.element.isVisible
    }

    fun getHeaderText(): String {
        return switchToPage(Header::class.java).getPageHeaderText()
    }

    fun warningText(): String {
        return txtWarning.element.text
    }

    fun isWarningMsgHighlighted(): String {
        return txtWarning.element.getCssValue("background-color")
    }

    fun clickAgreeandContinue() {
        btnAgree.element.click()
    }

    fun clickBacktoHome() {
        btnBack2Home.element.click()
    }

    fun getSensitiveList(): ArrayList<String> {
        var list = ArrayList<String>()
        val listSensitiveData = findAllByXpath("//div[@class='info']/ul/li")
        listSensitiveData.forEach { el ->
            list.add(el.text)
        }
        return list
    }
}

