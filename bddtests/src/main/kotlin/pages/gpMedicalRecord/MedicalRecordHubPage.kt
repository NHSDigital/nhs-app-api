package pages.gpMedicalRecord

import pages.HybridPageElement
import pages.HybridPageObject

class MedicalRecordHubPage : HybridPageObject() {

    fun pageTitleGpMedicalRecord(): HybridPageElement {
        val webDesktopLocator = "//h1[contains(text(),\"Your GP health record\")]"
        return HybridPageElement(
            webDesktopLocator = webDesktopLocator,
            page = this,
            helpfulName = "Your GP health record"
        )
    }

    fun pageTitleYourHealth(): HybridPageElement {
        val webDesktopLocator = "//h1[contains(text(),\"Your health\")]"
        return HybridPageElement(
            webDesktopLocator = webDesktopLocator,
            page = this,
            helpfulName = "Your health"
        )
    }

    fun getHeaderElement(title: String): HybridPageElement {
        val locator = "//h2[contains(text(),\"$title\")]"
        return HybridPageElement(
            webDesktopLocator = locator,
            page = this,
            helpfulName = title
        )
    }

    val gpMedicalRecordPanel = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_gp_medical_record']",
            page = this
    )

    val ndopLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),'Choose if data from your health records is shared')]",
        page = this
    )
}
