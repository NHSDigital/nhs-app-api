package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordWarningPage : HybridPageObject() {

    private val txtWarning = HybridPageElement(
            webDesktopLocator = "//div[@data-purpose='warning']",
            androidLocator = null,
            page = this
    )

    private val btnAgree = HybridPageElement(
            webDesktopLocator = "//button[contains(text(),'Agree and continue')]",
            androidLocator = null,
            page = this
    )

    private val btnBack2Home = HybridPageElement(
            webDesktopLocator = "//button[contains(text(),'Back to home')]",
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

    fun clickAgreeAndContinue() {
        btnAgree.click()
    }

    fun clickBacktoHome() {
        btnBack2Home.click()
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

