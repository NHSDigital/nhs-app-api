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
                    "organ donation" to "/organ-donation",
                    "more" to "/more",
                    "account" to "/account",
                    "terms and conditions" to "/terms-and-conditions",
                    "my record" to "/my-record",
                    "gp medical record" to "/gp-medical-record",
                    "gp at hand my record" to "/my-record/gp-at-hand",
                    "my gp medical record" to "/gp-medical-record",
                    "gp at hand my record" to "/my-record/gp-at-hand",
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
