package features.sharedSteps

import config.Config

open class PageUrl {

    val map: HashMap<String, String> =
            hashMapOf(
                    "appointment booking" to "/appointments/booking",
                    "appointment guidance" to "/appointments/booking-guidance",
                    "your appointments" to "/appointments",
                    "informatica appointments" to "/appointments/informatica",
                    "gp at hand appointments" to "/appointments/gp-at-hand",
                    "data sharing" to "/data-sharing",
                    "your prescriptions" to "/prescriptions",
                    "prescription repeat courses" to "/prescriptions/repeat-courses",
                    "gp at hand prescriptions" to "/prescriptions/gp-at-hand",
                    "partial success" to "/prescriptions/repeat-partial-success",
                    "prescriptions success" to "/prescriptions/order-success",
                    "organ donation" to "/organ-donation",
                    "more" to "/more",
                    "account" to "/account",
                    "manage cookies" to "/account/cookies",
                    "notifications settings" to "/account/notifications",
                    "terms and conditions" to "/terms-and-conditions",
                    "my record" to "/my-record",
                    "gp at hand my record" to "/my-record/gp-at-hand",
                    "gp at hand my record" to "/my-record/gp-at-hand",
                    "gp medical record" to "/gp-medical-record",
                    "my gp medical record" to "/gp-medical-record",
                    "gp medical record diagnosis" to "/gp-medical-record/diagnosis",
                    "gp medical record examinations" to "/gp-medical-record/examinations",
                    "gp medical record test results detail" to "/gp-medical-record/test-results-detail",
                    "gp medical record procedures" to "/gp-medical-record/procedures",
                    "patient practice messaging" to "/patient-practice-messaging",
                    "notifications settings" to "/account/notifications"
            )

    private val mobileOverrides =
            arrayOf("organ donation",
                    "more")

    fun getPage(pageName: String): String {
        val path = map[pageName.toLowerCase()]!!

        return "${Config.instance.url}$path"
    }
}
