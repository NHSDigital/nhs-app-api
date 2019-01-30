package pages.organDonation

import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.RadioButtons2

class OrganDonationSpecificOrganChoice(val title:String, page: HybridPageObject) {

    private val container  by lazy {
        HybridPageElement(
                "//div[h3[text()=\"$title\"]]",
                page = page,
                helpfulName = "$title div")
    }


    private val radioButtons by lazy {
        RadioButtons2(HybridPageElement(
                "//div[h3[text()=\"$title\"]]/div/div[input][label]",
                page = page,
                helpfulName = "$title buttons"))
    }

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

    fun assertAllUnselected() {
        radioButtons.assertAllUnselected()
    }

    fun select(decision: Boolean) {
        radioButtons.button(getLabel(decision)).select()
    }

    fun assertSelection(decision: Boolean) {
        radioButtons.assertSelected(getLabel(decision))
    }

    private fun getLabel(decision: Boolean): String {
        return if (decision) "Yes" else "No"
    }
}