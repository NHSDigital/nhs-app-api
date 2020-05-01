package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.DropdownElement
import pages.sharedElements.expectedPage.ExpectedPageStructure
import utils.getOrFail

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationWithdrawDecisionPage : OrganDonationBasePage()  {

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertBody()
    }

    override val titleText = "Withdraw your previous organ donation decision"

    val withdrawalReasonList = DropdownElement(
            "Reason for withdrawing",
            "withdrawal Reason Dropdown",
            this
    )

    val defaultDropDownValue = "Select reason"

    private fun assertBody() {
        val expectedDropDownContent = arrayListOf(defaultDropDownValue)
        expectedDropDownContent.addAll(OrganDonationSerenityHelpers.REFERENCE_WITHDRAWAL_REASONS
                .getOrFail<ArrayList<String>>())
        val expected = ExpectedPageStructure().h2(titleText)
                .paragraph("Withdrawing from the NHS Organ Donor Register is different " +
                        "from recording a decision not to donate (opting out). " +
                        "If you withdraw, we will not know your decision.")
                .paragraph("In line with changes to the law around organ donation, " +
                        "you are considered to have agreed to be an organ donor, unless:")
                .listItems("you have recorded a decision not to donate",
                        "you are in an excluded group")
                .paragraph("Find out more about the law and excluded groups.")
                .paragraph("If you do not want to be an organ donor, " +
                        "the best way to tell us is to update your decision. " +
                        "You can change your decision at any time.")
                .paragraph("Whatever you decide, please make sure your family know your decision.")
                .dropdown("Reason for withdrawing", expectedDropDownContent)
                .button("Continue")
        expected.assert(this)
    }

    fun assertWithdrawalListValidationError() {
        val problem = "There's a problem"
        val withdrawalListValidationMessage = "Give a reason for withdrawing your decision"

        validationBanner.assertVisible(arrayListOf(problem, withdrawalListValidationMessage))
        withdrawalReasonList.assertInlineErrorContent(withdrawalListValidationMessage)
    }
}