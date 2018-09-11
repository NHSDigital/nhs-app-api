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
            browserLocator = "//div[@data-purpose='warning']",
            androidLocator = null,
            page = this
    )

    val btnAgree = HybridPageElement(
            browserLocator = "//main/div/button[contains(text(),'Agree and continue')]",
            //*[@id="mainDiv"]/main/div[2]/button[1]
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

    fun warningText(): String {
        return txtWarning.element.text
    }

    fun isWarningMsgHighlighted(): String {
        return txtWarning.element.getCssValue("background-color")
    }

    fun clickAgreeAndContinue() {
        btnAgree.element.click()
    }

    fun clickBacktoHome() {
        btnBack2Home.element.click()
    }

    fun getSensitiveList(): ArrayList<String> {
        val list = ArrayList<String>()
        val listSensitiveData = findAllByXpath("//div[@data-purpose='info']/ul/li")
        listSensitiveData.forEach { el ->
            list.add(el.text)
        }
        return list
    }
}

