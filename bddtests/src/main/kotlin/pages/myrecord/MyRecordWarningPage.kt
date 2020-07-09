package pages.myrecord

import pages.HybridPageObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.sharedElements.expectedPage.ParsedPage

class MyRecordWarningPage : HybridPageObject() {

    fun assertContent(){
        val fullContent =
                ExpectedPageStructure()
                        .h2("Important")
                        .paragraph("Your record may contain sensitive information. " +
                                "If someone is pressuring you for this information, " +
                                "contact your GP surgery immediately.")
                        .paragraph("You have a legal right to access the information in your record.")
                        .paragraph("Your record shows personal data, such as your details, " +
                                "allergies and medications.")
                        .paragraph("Depending on what your GP surgery shares, you may also see:")
        .listItems("your medical history, including problems and consultation notes",
                "test results that you may not have discussed with your doctor")
                        .button("Continue")
                        .paragraph("Back to home")
        val parsedPage = ParsedPage.parse(this,"//div[@id='mainDiv']")

        ExpectedPageStructureAssertor().assert(parsedPage, fullContent.build())
    }
}
