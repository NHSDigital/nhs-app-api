package features.dataSharing.steps

import config.Config
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Step
import pages.NdopPage
import java.net.URL

open class NdopSteps {

    lateinit var ndop: NdopPage
    lateinit var browser: BrowserSteps

    @Step
    fun tokenIsDisplayed(): Boolean {
        if(ndop.onMobile()){
            URL(Config.instance.dataPreferencesUrl)
        }
        return ndop.tokenIsDisplayed()
    }

}