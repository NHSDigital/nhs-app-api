package features.sharedSteps

import config.Config

open class PageUrl {

    companion object {
        val map: HashMap<String, String> =
                hashMapOf(
                        "home" to "/",
                        "appointment booking" to "/appointments/booking",
                        "appointment guidance" to "/appointments/booking-guidance",
                        "appointment hub" to "/appointments",
                        "your gp appointments" to "/appointments/gp-appointments",
                        "hospital and other appointments" to "/appointments/hospital-appointments",
                        "informatica appointments" to "/appointments/informatica",
                        "gp at hand appointments" to "/appointments/gp-at-hand",
                        "data sharing" to "/data-sharing",
                        "data sharing make your choice" to "/data-sharing/make-your-choice",
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
                        "gp medical record" to "/gp-medical-record/gp-record",
                        "health record hub" to "/gp-medical-record",
                        "gp medical record diagnosis" to "/gp-medical-record/diagnosis",
                        "gp medical record examinations" to "/gp-medical-record/examinations",
                        "gp medical record test results detail" to "/gp-medical-record/test-results-detail",
                        "gp medical record procedures" to "/gp-medical-record/procedures",
                        "gp medical record documents" to "/gp-medical-record/documents",
                        "gp medical record document information" to "/gp-medical-record/documents/1",
                        "gp medical record document detail" to "/gp-medical-record/documents/detail/1",
                        "patient practice messaging" to "/patient-practice-messaging",
                        "notifications settings" to "/account/notifications",
                        "home" to "/",
                        "patient practice messaging urgency" to "/patient-practice-messaging/urgency",
                        "patient practice messaging contact your gp" to
                                "/patient-practice-messaging/urgency/contact-your-gp",
                        "patient practice messaging recipients" to "/patient-practice-messaging/recipients",
                        "patient practice messaging view details" to "/patient-practice-messaging/view-details",
                        "patient practice messaging send message" to "/patient-practice-messaging/send-message",
                        "patient practice messaging delete" to "/patient-practice-messaging/delete",
                        "patient practice messaging delete success" to "/patient-practice-messaging/delete-success",
                        "notifications settings" to "/account/notifications",
                        "symptoms" to "/symptoms"
                )

        fun getPage(pageName: String): String {
            val path = map[pageName.toLowerCase()]!!

            return "${Config.instance.url}$path"
        }
    }
}
