package pages.throttling

import pages.HybridPageObject
import pages.HybridPageElement
import pages.sharedElements.TextBlockElement

class WaitingListJoinedPage : HybridPageObject() {

    val waitingListResultsHeader = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(), 'Next steps')]",
            webMobileLocator = "//h1[contains(text(), 'Next steps')]",
            androidLocator = null,
            page = this
    )


    val homeButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Go to home screen')]",
            webMobileLocator = "//button[contains(text(), 'Go to home screen')]",
            androidLocator = null,
            page = this
    )

    fun assertJoined() {
        TextBlockElement.withH2Header("What happens next", this)
                .assert("We've just sent you an email. You need to confirm you want to be updated by us " +
                        "about your GP surgery. You do this by following a link in that email. It may be in " +
                        "your junk folder.")
                .assert("When your GP surgery can use all the features of the app, we'll email you. " +
                        "You'll then be able to create an NHS login.")
    }

    fun assertNotJoined(){
        TextBlockElement.withH2Header("What happens next", this)
                .assert("Check in with your GP surgery to find out when they'll be " +
                        "using all the features of the app. When they are, they will help you set up an NHS login.")
    }

    fun assertWhatToDoUntilThen() {
        TextBlockElement.withH2Header("What you can do until then", this)
                .assert("You can check your symptoms in the app. 'Check if you need urgent help' will " +
                        "direct you to medical help, if you need it.",
                        "You can also record your organ donation decision.")
    }
}
