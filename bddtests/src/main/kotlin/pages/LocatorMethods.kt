package pages

open class LocatorMethods(var page:HybridPageObject) {

    fun waitForNativeStepToComplete(milliseconds: Long = DEFAULT_MOBILE_WAIT) {
        if (page.onMobile()) {
            Thread.sleep(milliseconds)
        }
    }

    fun assertNativeElementsLoaded(elementToCheck:HybridPageElement){
        if(page.onMobile()) {
            elementToCheck.assertIsVisible()
        }
    }
}