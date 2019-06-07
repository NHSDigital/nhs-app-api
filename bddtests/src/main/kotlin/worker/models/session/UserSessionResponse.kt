package worker.models.session

import javax.servlet.http.Cookie

data class UserSessionResponse(var userSessionResponseCookie: UserSessionResponseCookie,
                               var userSessionResponseBody: UserSessionResponseBody) {

    class UserSessionResponseCookie(var cookie: Cookie)

    class UserSessionResponseBody(var name: String,
                                  var sessionTimeout: Number,
                                  var serviceJourneyRules: Any)
}