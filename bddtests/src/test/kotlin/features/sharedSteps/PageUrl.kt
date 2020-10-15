
package features.sharedSteps

open class PageUrl {

    companion object {
        val map: HashMap<String, String> =
                hashMapOf(
                        "home" to "/",
                        "appointment booking" to "/appointments/gp-appointments/booking",
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
                        "gp at hand health record" to "/health-records/gp-at-hand",
                        "gp medical record" to "/health-records/gp-medical-record",
                        "health record hub" to "/health-records",
                        "gp medical record diagnosis" to "/health-records/gp-medical-record/diagnosis",
                        "gp medical record examinations" to "/health-records/gp-medical-record/examinations",
                        "gp medical record test results detail" to
                                "/health-records/gp-medical-record/test-results-detail",
                        "gp medical record procedures" to "/health-records/gp-medical-record/procedures",
                        "gp medical record documents" to "/health-records/gp-medical-record/documents",
                        "gp medical record document information" to "/health-records/gp-medical-record/documents/1",
                        "gp medical record document detail" to "/health-records/gp-medical-record/documents/detail/1",
                        "patient practice messaging" to "/messages/gp-messages",
                        "notifications settings" to "/account/notifications",
                        "home" to "/",
                        "patient practice messaging urgency" to "/messages/gp-messages/urgency",
                        "patient practice messaging contact your gp" to
                                "/messages/gp-messages/urgency/contact-your-gp",
                        "patient practice messaging recipients" to "/messages/gp-messages/recipients",
                        "patient practice messaging view details" to "/messages/gp-messages/view-details",
                        "patient practice messaging send message" to "/messages/gp-messages/send-message",
                        "patient practice messaging delete" to "/messages/gp-messages/delete",
                        "patient practice messaging delete success" to "/messages/gp-messages/delete-success",
                        "notifications settings" to "/account/notifications",
                        "advice" to "/advice",
                        "messages hub" to "/messages"
                )

        fun getRelativePagePath(pageName: String): String {
            return map[pageName.toLowerCase()]!!
        }
    }
}
