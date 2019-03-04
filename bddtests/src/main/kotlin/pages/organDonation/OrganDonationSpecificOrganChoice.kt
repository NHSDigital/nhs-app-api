package pages.organDonation

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertSingleElementPresent
import pages.sharedElements.RadioButtons

class OrganDonationSpecificOrganChoice(val title:String, page: HybridPageObject) {

    private val container  by lazy {
        HybridPageElement(
                "//div[h3[text()=\"$title\"]]",
                page = page,
                helpfulName = "$title div")
    }

    private val radioButtons by lazy {
        RadioButtons.create(page, "//div[h3[text()=\"$title\"]]/div/label[input][div]" )}

    private val expectedOptions by lazy {
        arrayListOf(
                Pair("Yes", ""),
                Pair("No", "")
        )
    }

    fun assertDisplayed() {
        container.assertSingleElementPresent()
        radioButtons.assertAreEqual(expectedOptions)
    }

    fun select(decision: Boolean) {
        radioButtons.button(getLabel(decision)).select()
    }

    fun assertSelection(decision: Boolean) {
        radioButtons.assertSelected(getLabel(decision))
    }

    fun assertUnselected() {
        radioButtons.assertAllUnselected()
    }

    private fun getLabel(decision: Boolean): String {
        return if (decision) "Yes" else "No"
    }
}