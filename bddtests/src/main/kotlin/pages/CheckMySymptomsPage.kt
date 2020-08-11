package pages

open class CheckMySymptomsPage : HybridPageObject() {

    private val searchConditionsAndTreatments = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Search conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    private val useNhsOneOneOneOnline = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Use NHS 111 online')]",
            androidLocator = null,
            page = this
    )

    private val adviceAboutCoronavirus = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Get advice about coronavirus')]",
            androidLocator = null,
            page = this
    )

    private val askYourGpForAdvice = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Ask your GP for advice')]",
            androidLocator = null,
            page = this
    )

    fun assertSearchConditionsAndTreatmentsIsVisible() {
        searchConditionsAndTreatments.assertIsVisible()
    }

    fun clickSearchConditionsAndTreatments() {
        searchConditionsAndTreatments.click()
    }

    fun assertUseNhsOneOneOneOnlineIsVisible() {
        useNhsOneOneOneOnline.assertIsVisible()
    }

    fun clickUseNhsOneOneOneOnline(){
        useNhsOneOneOneOnline.click()
    }

    fun assertAdviceAboutCoronavirusIsVisible() {
        adviceAboutCoronavirus.assertIsVisible()
    }

    fun clickAdviceAboutCoronavirus() {
        adviceAboutCoronavirus.click()
    }

    fun clickAskYourGpForAdvice(){
        askYourGpForAdvice.click()
    }
}
