package pages.myrecord

import pages.HybridPageObject

class MyRecordTestResultDetailPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val serverErrorPageTitle = "Test result details data error - NHS App"
    val serverErrorPageHeader = "Test result details data error"
    val serverErrorHeader = "There's been a problem getting details of your test results"
    val serverErrorSubHeader = ""
    val serverErrorMessage = "If the problem continues and you need this " +
                             "information now, contact your GP surgery directly. " +
                             "For urgent medical advice, call 111."
}