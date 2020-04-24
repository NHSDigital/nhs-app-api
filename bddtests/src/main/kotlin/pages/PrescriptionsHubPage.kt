package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/")
open class PrescriptionsHubPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"Prescriptions\")]",
            androidLocator = null,
            page = this,
            helpfulName = "Prescriptions Hub Title"
    )

    val pharmacyName = HybridPageElement(
            webDesktopLocator = "//*[@id='nominated-pharmacy']//p",
            webMobileLocator = "//*[@id='nominated-pharmacy']//p",
            page = this
    )

    val viewOrdersPanel = HybridPageElement(
                webDesktopLocator = "//*[@id='view-orders']",
                webMobileLocator = "//*[@id='view-orders']",
                page = this
    )

    val nominatedPharmacyLink = HybridPageElement(
            webDesktopLocator = "//*[@id='nominated-pharmacy']",
            webMobileLocator = "//*[@id='nominated-pharmacy']",
            page = this
    )

    val pkbMedicinesJumpOffButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_pkb_medicines']",
            webMobileLocator = "//*[@id='nominated-pharmacy']",
            helpfulName = "pkb jump off point",
            page = this
    )


    fun assertPrescriptionsHubIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun clickOrderARepeatPrescriptionButton() {
        clickOnButtonContainingText("Order a repeat prescription")
    }
}
