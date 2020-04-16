package pages

open class P5UpliftPage : HybridPageObject() {
    private val heading = "Finish setting up your NHS login"
    private val description = "You'll need to connect to your GP surgery before you can book appointments, order " +
            "prescriptions and view your health records."

    private val headerElement by lazy {
        HybridPageElement(
                webDesktopLocator = "//h2",
                androidLocator = null,
                page = this
        ).withText(heading)
    }

    private val descriptionElement by lazy {
        HybridPageElement(
                webDesktopLocator = "//p",
                androidLocator = null,
                page = this
        ).withText(description)
    }

    fun assertUpliftBanner() {
        headerElement.assertSingleElementPresent()
        descriptionElement.assertSingleElementPresent()
    }
}
