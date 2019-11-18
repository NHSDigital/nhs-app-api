package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.HybridPageElement
import pages.sharedElements.LinksContent

class MyRecordInfoPage : HybridPageObject() {

    val pageTitle = HybridPageElement(
                    webDesktopLocator = "//h1[contains(text(),\"Your GP medical record\")]",
                    androidLocator = null,
                    page = this,
                    helpfulName = "GP Medical Record Title")

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    webDesktopLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    fun getBody(message: String): HybridPageElement {
        val noInformationText =
                HybridPageElement(
                        webDesktopLocator = "//p[contains(text(), \"$message\")]",
                        androidLocator = null,
                        page = this)
        return noInformationText
    }

    fun clickMedicalRecordSectionLink(linkText: String) {
        val linkContent = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='medical-record-menu']")
                .addLink(linkText)
        val linkElement by lazy {LinksElement(this, linkContent)}
        val link = linkElement.link(linkText)
        link.click()
    }
}
