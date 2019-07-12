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

    private val txtShownInformation = HybridPageElement(
            webDesktopLocator = "//div[@data-purpose='info']/p",
            androidLocator = null,
            page = this
    )

    private val btnContinue = HybridPageElement(
            webDesktopLocator = "//button[contains(text(),'Continue')]",
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

    fun isContinuePresent(): Boolean {
        return btnContinue.isVisible
    }

    fun warningText(): String {
        return txtWarning.text
    }

    fun clickWarningContinue() {
        btnContinue.click()
    }

    fun clickBacktoHome() {
        backToHome.click()
    }

    fun getShownInformationDescription(): String {
        return txtShownInformation.text
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

