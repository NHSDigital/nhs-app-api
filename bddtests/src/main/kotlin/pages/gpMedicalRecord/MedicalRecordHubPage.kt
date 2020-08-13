package pages.gpMedicalRecord

import pages.HybridPageElement
import pages.HybridPageObject

class MedicalRecordHubPage : HybridPageObject() {

    fun pageTitleGpMedicalRecord(): HybridPageElement {
        val webDesktopLocator = "//h1[contains(text(),\"Your GP health record\")]"
        return HybridPageElement(
                webDesktopLocator = webDesktopLocator,
                androidLocator = webDesktopLocator,
                iOSLocator = webDesktopLocator,
                page = this,
                helpfulName = "Your GP health record"
        )
    }

    fun pageTitleHealthRecords(): HybridPageElement {
        val webDesktopLocator = "//h1[contains(text(),\"Health records\")]"
        return HybridPageElement(
                webDesktopLocator = webDesktopLocator,
                androidLocator = webDesktopLocator,
                iOSLocator = webDesktopLocator,
                page = this,
                helpfulName = "Health Records"
        )
    }

    fun getHeaderElement(title: String): HybridPageElement {
        val locator = "//h2[contains(text(),\"$title\")]"
        return HybridPageElement(
                webDesktopLocator = locator,
                androidLocator = locator,
                iOSLocator = locator,
                page = this,
                helpfulName = title
        )
    }

    val gpMedicalRecordPanel = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_gp_medical_record']",
            page = this
    )
}
