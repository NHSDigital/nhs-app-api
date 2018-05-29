package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/more")
open class MorePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun clickOrganDonations() {
        findByXpath("//*[@id='btn_organdonation']").click()
    }
}