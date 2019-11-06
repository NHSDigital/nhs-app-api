package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordHealthConditionsModule : HybridPageObject() {

    private val healthConditionsLinkText = "Health conditions"
    private val healthConditions by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(healthConditionsLinkText)

    val link = healthConditions.link(healthConditionsLinkText)

}