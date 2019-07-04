package features.sharedSteps

import config.Config

open class PageUrl {

    fun getPage(pageName:String, isOnMobile: Boolean) : String{
        val map: HashMap<String,String> =
                hashMapOf(
                "appointment booking"          to "/appointments/booking",
                "appointment guidance"         to "/appointments/booking-guidance",
                "my appointments"              to "/appointments",
                "informatica appointments"     to "/appointments/informatica",
                "gp at hand appointments"      to "/appointments/gp-at-hand",
                "data sharing"                 to "/data-sharing",
                "my prescriptions"             to "/prescriptions",
                "prescription repeat courses"  to "/prescriptions/repeat-courses",
                "gp at hand prescriptions"     to "/prescriptions/gp-at-hand",
                "organ donation"               to getPageUrl(isOnMobile, "/organ-donation"),
                "more"                         to getPageUrl(isOnMobile, "/more"),
                "account"                      to "/account",
                "terms and conditions"         to "/terms-and-conditions",
                "my record"                    to "/my-record",
                "gp at hand my record"         to "/my-record/gp-at-hand"
                )
        val path = map[pageName.toLowerCase()]!!
        return "${Config.instance.url}$path"
    }

    private fun getPageUrl(isOnMobile: Boolean, urlString: String): String {
        val urlForWeb = "$urlString?source=ios"
        val suffix = Config.instance.nativeUrlSuffix
        return when (isOnMobile) {
            true -> "$urlString$suffix"
            false -> urlForWeb
        }
    }
}