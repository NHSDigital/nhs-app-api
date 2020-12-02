package pages.linkedProfiles.shutterPages

import pages.HybridPageObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor

class NoLinkedProfilesShutterPage: HybridPageObject() {

    fun assertText() {
        val expected = ExpectedPageStructure()
                .paragraph("You do not have any linked profiles set up on your account.")
                .paragraph("Family members and carers can access health services on behalf of someone else " +
                        "through linked profiles.")
                .paragraph("You can book appointments for them, order repeat prescriptions, and view " +
                        "their health record, where appropriate.")
                .paragraph("Find out more about linked profiles")
                .h2("Add a linked profile")
                .paragraph("Contact the GP surgery of the person you would like to set up a linked profile " +
                        "for, and ask them to register you for proxy access.")

        ExpectedPageStructureAssertor().assert(this, expected.build())
    }
}
