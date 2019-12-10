package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.DropdownElement
import pages.sharedElements.TextBlockElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationWithdrawDecisionPage : OrganDonationBasePage()  {

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertBody()
    }

    override val titleText = "Withdraw your decision"

    val withdrawalReasonList = DropdownElement(
            "Reason for withdrawing",
            "withdrawal Reason Dropdown",
            this
    )

    val defaultDropDownValue = "Select reason"

    private fun assertBody() {
        TextBlockElement.withH2Header(titleText, this)
                .assert("Withdrawing your decision means there will be no recorded decision for you, " +
                        "and without this your family will be asked to decide for you, when you die.",
                        "If you are certain you do not want to donate your organs or tissue, " +
                                "you need to register a 'no' decision.",
                        "Whatever you decide, please make sure your family know your decision.")
    }

    fun assertWithdrawalListValidationError() {
        val problem = "There's a problem"
        val withdrawalListValidationMessage = "Give a reason for withdrawing your decision"

        validationBanner.assertVisible(
                arrayListOf(problem, withdrawalListValidationMessage))
        withdrawalReasonList.assertInlineErrorContent(withdrawalListValidationMessage)
    }
}