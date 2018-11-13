package features.appointments.stepDefinitions

import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import javax.servlet.http.Cookie


class AppointmentsSharedStepDefinitions {



    companion object {

        @Step
        fun SetAppointmentsSessionCookieToExpired() {
            Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
        }

        val expiredCookie = Cookie(
                "Set-Cookie",
                "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJL" +
                        "pr5jIZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuG" +
                        "h3Dc9Dqe9jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0i" +
                        "FNsJTXJm5FZBVBeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR" +
                        "4tU-rHpdyz0JcbStgePRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A" +
                        "6zxTv1y0nwu8AAXcUg7EFlSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
        )
    }


}
