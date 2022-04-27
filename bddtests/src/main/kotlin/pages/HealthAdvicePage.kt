package pages

open class HealthAdvicePage : HybridPageObject() {

    val searchConditionsAndTreatments = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),'Search conditions and treatments')]",
        page = this
    )

    val useNhsOneOneOneOnline = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),'Use NHS 111 online')]",
        page = this
    )

    val adviceAboutCoronavirus = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),'Get advice about coronavirus')]",
        page = this
    )

    val askYourGpForAdvice = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_gpAdvice']//h2[contains(text(),'Ask your GP for advice')]",
        page = this
    )

    val engageMedicalAdvice = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_engage_medical_advice']//h2[contains(text(),'Ask your GP for advice')]",
        page = this
    )

    val accuRxMedicalAdvice = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_accurx_medical_advice']//h2[contains(text(),'Ask your GP for advice')]",
        page = this
    )
}
