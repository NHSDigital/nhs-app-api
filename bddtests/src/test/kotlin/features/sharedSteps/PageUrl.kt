package features.sharedSteps

open class PageUrl {
    companion object {
        val map: HashMap<String, String> =
            hashMapOf(
                "account and settings" to "/more/account-and-settings",
                "advice" to "/advice",
                "appointment booking" to "/appointments/gp-appointments/booking",
                "appointment hub" to "/appointments",
                "data sharing make your choice" to "/data-sharing/make-your-choice",
                "data sharing" to "/data-sharing",
                "gp at hand appointments" to "/appointments/gp-at-hand",
                "gp at hand health record" to "/health-records/gp-at-hand",
                "gp at hand prescriptions" to "/prescriptions/gp-at-hand",
                "gp medical record diagnosis" to "/health-records/gp-medical-record/diagnosis",
                "gp medical record document detail" to "/health-records/gp-medical-record/documents/detail/1",
                "gp medical record document information" to "/health-records/gp-medical-record/documents/1",
                "gp medical record documents" to "/health-records/gp-medical-record/documents",
                "gp medical record examinations" to "/health-records/gp-medical-record/examinations",
                "gp medical record procedures" to "/health-records/gp-medical-record/procedures",
                "gp medical record test results detail" to "/health-records/gp-medical-record/test-results-detail",
                "gp medical record" to "/health-records/gp-medical-record",
                "health record hub" to "/health-records",
                "home" to "/",
                "hospital and other appointments" to "/appointments/hospital-appointments",
                "informatica appointments" to "/appointments/informatica",
                "legal and cookies" to "/more/account-and-settings/legal-and-cookies/",
                "legal and cookies manage cookies" to "/more/account-and-settings/legal-and-cookies/manage-cookies",
                "linked profiles" to "/linked-profiles",
                "messages hub" to "/messages",
                "more" to "/more",
                "notifications settings" to "/more/account-and-settings/manage-notifications",
                "organ donation" to "/organ-donation",
                "partial success" to "/prescriptions/repeat-partial-success",
                "patient practice messaging contact your gp" to "/messages/gp-messages/urgency/contact-your-gp",
                "patient practice messaging delete success" to "/messages/gp-messages/delete-success",
                "patient practice messaging delete" to "/messages/gp-messages/delete",
                "patient practice messaging recipients" to "/messages/gp-messages/recipients",
                "patient practice messaging send message" to "/messages/gp-messages/send-message",
                "patient practice messaging urgency" to "/messages/gp-messages/urgency",
                "patient practice messaging view details" to "/messages/gp-messages/view-details",
                "patient practice messaging" to "/messages/gp-messages",
                "prescription repeat courses" to "/prescriptions/repeat-courses",
                "prescriptions success" to "/prescriptions/order-success",
                "terms and conditions" to "/terms-and-conditions",
                "wayfinder" to "/wayfinder",
                "your gp appointments" to "/appointments/gp-appointments",
                "your prescriptions" to "/prescriptions",
                "choose test results" to "/health-records/gp-medical-record/choose-test-result-year",
                "test results for year" to "/health-records/gp-medical-record/test-results-for-year"
            )

        fun getRelativePagePath(pageName: String): String {
            return map[pageName.toLowerCase()]!!
        }
    }
}
