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

    override val titleText = "Withdraw your decision"

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
                .paragraph("Withdrawing your decision means there will be no recorded decision for you, " +
                        "and without this your family will be asked to decide for you, when you die.")
                .paragraph("If you are certain you do not want to donate your organs or tissue, " +
                        "you need to register a 'no' decision.")
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