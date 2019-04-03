package features.sharedSteps

import config.Config

open class PageUrl {

    fun getPage(pageName:String) : String{

        val map: HashMap<String,String> =
                hashMapOf(
                "appointment booking"          to "/appointments/booking",
                "appointment guidance"         to "/appointments/booking-guidance",
                "my appointments"              to "/appointments",
                "data sharing"                 to "/data-sharing",
                "my prescriptions"             to "/prescriptions",
                "prescription repeat courses"  to "/prescriptions/repeat-courses",
                "organ donation"               to "/organ-donation",
                "more"                         to "/more",
                "account"                      to "/account",
                "terms and conditions"         to "/terms-and-conditions"
                )
        val path = map[pageName.toLowerCase()]!!
        return "${Config.instance.url}$path"
    }
}