package pages

open class ServerError : ErrorPage() {

  private val serverErrorHeader = "This might be a temporary problem."
  private val serverErrorIfYouNeedToBookAnAppointment =
    "If you need to book an appointment or get a prescription now, " +
            "use your GP surgery's website or call the surgery directly."
  private val serverErrorMessage = "For urgent medical advice, go to 111.nhs.uk or call 111."

  fun assert() {
    serverErrorPageHeader.assertIsVisible()
    this.assertHeaderText(serverErrorHeader)
    this.assertIfYouNeedToBookAnAppointmentText(serverErrorIfYouNeedToBookAnAppointment)
    this.assertMessageText(serverErrorMessage)
    this.assertNoRetryButton()
  }

  private val serverErrorPageHeader = HybridPageElement(
    webDesktopLocator = "//h1[contains(text(),\"The service is unavailable\")]",
    page = this,
    helpfulName = "Server error header"
  )

}
