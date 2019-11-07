package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordMedicinesModule : HybridPageObject() {

    private val medicinesLinkText = "Medicines"
    private val medicines by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(medicinesLinkText)

    val link = medicines.link(medicinesLinkText)

}