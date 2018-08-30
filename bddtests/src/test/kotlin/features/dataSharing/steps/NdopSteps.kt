package features.dataSharing.steps

import net.thucydides.core.annotations.Step
import pages.NdopPage

open class NdopSteps {

    lateinit var ndop: NdopPage

    @Step
    fun tokenIsDisplayed(): Boolean {
        return ndop.tokenIsDisplayed()
    }

}