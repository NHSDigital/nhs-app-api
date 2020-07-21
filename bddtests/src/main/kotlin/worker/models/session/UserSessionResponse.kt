package worker.models.session

import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse
import javax.servlet.http.Cookie

data class UserSessionResponse(var userSessionResponseCookie: UserSessionResponseCookie,
                               var userSessionResponseBody: UserSessionResponseBody) {

    class UserSessionResponseCookie(var cookie: Cookie)

    class UserSessionResponseBody(var name: String,
                                  var sessionTimeout: Number,
                                  var serviceJourneyRules: ServiceJourneyRulesResponse,
                                  var userSessionCreateReferenceCode: String?)
}
