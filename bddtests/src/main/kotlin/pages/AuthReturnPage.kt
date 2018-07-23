package pageobjects

import pages.HybridPageObject

class AuthReturnPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    val errorPageTitle = "Session error - NHS App"
    val errorH1 = "Session Error"
    val errorH2 = "Sorry, there's been a problem loading this page"
    val errorParagraph = "Please go back to the homescreen and sign in again.\nIf the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111."
    val errorCtaText = "Back to home"
}

