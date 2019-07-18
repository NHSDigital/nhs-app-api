package features.authentication.stepDefinitions

import pages.assertIsVisible
import pages.navigation.WebHeader
import utils.ISerenityHelperEnums

enum class AuthenticationSerenityHelpers : ISerenityHelperEnums {
   IM1_CONNECTION_REQUEST,
   IM1_CONNECTION_RESPONSE,
   USER_SESSION_RESPONSE
}


class NativeHeaderHelper {

   companion object WebHeaderChecker {
      fun followAppointmentHeaderLink(webHeader: WebHeader) {
         webHeader.clickAppointmentsPageLink()
         webHeaderCheck("Appointments", webHeader)
      }

      fun followSymptomsHeaderLink(webHeader: WebHeader) {
         webHeader.clickSymptomsPageLink()
         webHeaderCheck("Symptoms", webHeader)
      }

      fun followPrescriptionsHeaderLink(webHeader: WebHeader) {
         webHeader.clickPrescriptionsPageLink()
         webHeaderCheck("Repeat prescriptions", webHeader)
      }

      fun followMedicalRecordHeaderLink(webHeader: WebHeader) {
         webHeader.clickMyRecordPageLink()
         webHeaderCheck("My medical record", webHeader)
      }

      fun followMoreHeaderLink(webHeader: WebHeader) {
         webHeader.clickMorePageLink()
         webHeaderCheck("More", webHeader)
      }

      fun followAccountHeaderLink(webHeader: WebHeader) {
         webHeader.clickAccount()
         webHeaderCheck("Account", webHeader)
      }

      private fun webHeaderCheck(title:String, webHeader: WebHeader) {
         webHeader.isPageTitleCorrect(title)
         webHeader.getAllBreadCrumb().assertIsVisible()
      }
   }
}