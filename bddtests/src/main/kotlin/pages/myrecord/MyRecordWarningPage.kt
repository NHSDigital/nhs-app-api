package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject
import pages.isVisible
import pages.text

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

    private val backToHome = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'Back to home')]",
            androidLocator = "//button[contains(text(),'Back to home')]",
            page = this
    )


    fun isBackToHomePresent(): Boolean {
        return backToHome.isVisible
    }

    fun isAgreePresent(): Boolean {
        return btnAgree.isVisible
    }

    fun warningText(): String {
        return txtWarning.text
    }

    fun clickAgreeAndContinue() {
        btnAgree.click()
    }

    fun clickBacktoHome() {
        backToHome.click()
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

