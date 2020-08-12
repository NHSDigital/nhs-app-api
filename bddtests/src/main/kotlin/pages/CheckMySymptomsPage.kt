package pages

open class CheckMySymptomsPage : HybridPageObject() {

    val searchConditionsAndTreatments = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Search conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    val useNhsOneOneOneOnline = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Use NHS 111 online')]",
            androidLocator = null,
            page = this
    )

    val adviceAboutCoronavirus = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Get advice about coronavirus')]",
            androidLocator = null,
            page = this
    )

    val askYourGpForAdvice = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(),'Ask your GP for advice')]",
            androidLocator = null,
            page = this
    )

    fun assertPageDisplayed(){
        searchConditionsAndTreatments.assertSingleElementPresent()
        useNhsOneOneOneOnline.assertSingleElementPresent()
        adviceAboutCoronavirus.assertSingleElementPresent()
    }
}
