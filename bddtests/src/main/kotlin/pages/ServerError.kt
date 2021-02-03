package pages

open class ServerError : ErrorPage() {

  private val serverErrorHeader = "We're experiencing technical difficulties"
  private val serverErrorMessage = "Try again later. " +
          "If the problem continues and you need to book an appointment or get " +
          "a prescription now, contact your GP surgery directly. " +
          "For urgent medical advice, go to 111.nhs.uk or call 111."

  fun assert() {
    serverErrorPageHeader.assertIsVisible()
    this.assertHeaderText(serverErrorHeader)
    this.assertMessageText(serverErrorMessage)
    this.assertNoRetryButton()
  }

  private val serverErrorPageHeader = HybridPageElement(
    webDesktopLocator = "//h1[contains(text(),\"Server error\")]",
    androidLocator = null,
    page = this,
    helpfulName = "Server error header"
  )

}
