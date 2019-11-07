package pages.gpMedicalRecord

import pages.HybridPageElement
import pages.HybridPageObject

class MedicinesIndexPage: HybridPageObject() {

    private val acuteMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a[@id='acute-medicines']",
            page = this
    )
    private val currentMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a[@id='current-medicines']",
            page = this
    )
    private val discontinuedMedicinesLink = HybridPageElement(
            webDesktopLocator = "//a[@id='discontinued-medicines']",
            page = this
    )

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
