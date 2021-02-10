package features.authentication.stepDefinitions

import pages.navigation.WebHeader

open class NativeHeaderHelper {

   companion object {

      fun followAppointmentHeaderLink(webHeader: WebHeader) {
         webHeader.clickAppointmentsPageLink()
         webHeaderCheck("Appointments", webHeader)
      }

      fun followAdviceHeaderLink(webHeader: WebHeader) {
         webHeader.clickAdvicePageLink()
         webHeaderCheck("Advice", webHeader)
      }

      fun followPrescriptionsHeaderLink(webHeader: WebHeader) {
         webHeader.clickPrescriptionsPageLink()
         webHeaderCheck("Prescriptions", webHeader)
      }

      fun followYourHealthHeaderLink(webHeader: WebHeader) {
         webHeader.clickYourHealthPageLink()
         webHeaderCheck("Your medical record", webHeader)
      }

      fun followAccountHeaderLink(webHeader: WebHeader) {
         webHeader.clickAccount()
         webHeaderCheck("Account", webHeader)
      }

      private fun webHeaderCheck(title: String, webHeader: WebHeader) {
         webHeader.isPageTitleCorrect(title)
      }
   }
}
