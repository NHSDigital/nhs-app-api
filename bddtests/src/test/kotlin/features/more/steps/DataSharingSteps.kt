package features.more.steps

import net.thucydides.core.annotations.Step
import pages.DataSharingPage

class DataSharingSteps {

    lateinit var datasharing: DataSharingPage

    @Step
    fun dataSharingCompleteButtonVisible(): Boolean {
        return datasharing.isStartNowVisible()
    }

    @Step
    fun clickCompleteDataShardTerms() {
        return datasharing.clickStartNow()
    }

    @Step
    fun isNdopTestTextIsVisible(): Boolean {
        return datasharing.isNdopTestTextIsVisible()
    }
}