package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationSpecificOrganChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Your choice"

    private val expectedOptions = arrayListOf(
            "Heart",
            "Lungs",
            "Kidney",
            "Liver",
            "Corneas",
            "Pancreas",
            "Tissue",
            "Small bowel"
    )

    val organOptions by lazy {
        expectedOptions.map { organ -> OrganDonationSpecificOrganChoice(organ, this) }
    }

    fun chooseOption(organ: String, decision: Boolean) {
        organOptions.single { organOption -> organOption.title == organ }.select(decision)
    }

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        organOptions.forEach { option -> option.assertDisplayed() }
    }

    fun assertOrganOption(organ:String, selected: Boolean){
        OrganDonationSpecificOrganChoice(organ, this).assertSelection(selected)
    }

    fun assertAllOptionsUnselected(){
        RadioButtons.assertAllOnPageUnselected(this)
    }

}
