package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/prescription-type/non-repeat/contact-surgery/")
open class ContactYourGPPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Contact your GP surgery for non-repeat prescription requests\")]",
        page = this,
        helpfulName = "Contact your GP Title"
    )

    fun assertIsDisplayed() {
        pageTitle.assertIsVisible()
    }
}
