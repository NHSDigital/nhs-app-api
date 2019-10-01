package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordInfoPage : HybridPageObject() {

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    webDesktopLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    val allergies = MyRecordAllergiesModule(this)

    fun getBody(message: String): HybridPageElement {
        val noInformationText =
                HybridPageElement(
                        webDesktopLocator = "//p[contains(text(), \"$message\")]",
                        androidLocator = null,
                        page = this)
        return noInformationText
    }
}
