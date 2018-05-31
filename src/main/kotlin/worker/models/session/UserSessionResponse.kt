package worker.models.session

import javax.servlet.http.Cookie

data class UserSessionResponse(var userSessionResponseCookie: UserSessionResponseCookie, var userSessionResponseBody: UserSessionResponseBody) {

    class UserSessionResponseCookie(var cookie: Cookie)

    class UserSessionResponseBody(var givenName: String,
                                  var familyName: String,
                                  var sessionTimeout: Number)


}