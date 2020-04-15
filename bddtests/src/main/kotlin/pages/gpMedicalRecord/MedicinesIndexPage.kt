package pages.gpMedicalRecord

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.withNormalisedText

class MedicinesIndexPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            page = this
    ).withNormalisedText("Medicines")

    private val acuteMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a//h2",
            page = this
    ).withText("Acute (short-term) medicines")

    private val currentMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a//h2",
            page = this
    ).withText("Repeat medicines: current")

    private val discontinuedMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a//h2",
            page = this
    ).withText("Repeat medicines: discontinued")

    fun assertIsVisible() {
        pageTitle.assertIsVisible()
        acuteMedicinesLink.assertIsVisible()
        currentMedicinesLink.assertIsVisible()
        discontinuedMedicinesLink.assertIsVisible()
    }

    fun clickAcuteMedicinesLink() {
        acuteMedicinesLink.click()
    }

    fun clickCurrentMedicinesLink() {
        currentMedicinesLink.click()
    }

    fun clickDiscontinuedMedicinesLink() {
        discontinuedMedicinesLink.click()
    }
}
