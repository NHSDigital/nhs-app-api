package pages

open class MorePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun clickOrganDonations() {
        findByXpath("//*[@id='btn_organdonation']").click()
    }
}