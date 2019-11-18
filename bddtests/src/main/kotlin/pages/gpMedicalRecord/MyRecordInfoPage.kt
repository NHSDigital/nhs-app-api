package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.HybridPageElement

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

    val allergies = MyRecordAllergiesModule(this)

    val immunisations = MyRecordImmunisationsModule()

    val testResults = MyRecordTestResultsModule(this)

    fun getBody(message: String): HybridPageElement {
        val noInformationText =
                HybridPageElement(
                        webDesktopLocator = "//p[contains(text(), \"$message\")]",
                        androidLocator = null,
                        page = this)
        return noInformationText
    }
}
