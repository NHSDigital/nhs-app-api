package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordWarningPage : HybridPageObject() {

    private val txtWarning = HybridPageElement(
            browserLocator = "//div[@data-purpose='warning']",
            androidLocator = null,
            page = this
    )

    private val btnAgree = HybridPageElement(
            browserLocator = "//main/div/button[contains(text(),'Agree and continue')]",
            androidLocator = null,
            page = this
    )

    private val btnBack2Home = HybridPageElement(
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

