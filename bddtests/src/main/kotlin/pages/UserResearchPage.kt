package pages

class UserResearchPage : HybridPageObject() {

    private val headerText = "Help improve the NHS App"
    private val mainBodyText = "We would like to contact you about taking part in user research to improve" +
            " the NHS App and connected services."

    fun assertDisplayed() {
        assertPageHeader(headerText)
        getElement("//p").withText(mainBodyText).assertIsVisible()
    }
}
