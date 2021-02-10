package pages

import org.junit.Assert

open class P5UpliftPage : HybridPageObject() {
    private val heading = "Prove your identity to get full access"
    private val genericDescription = "You'll need to prove who you are before you can access this part of the NHS App."
    private val healthRecordsDescription =
            "You'll need to prove who you are before you can view your health records in the NHS App."
    private val descriptionMap = mapOf(
            "default" to "You'll need to prove who you are before you can book appointments, " +
                    "order repeat prescriptions, and view your NHS number and health records in the NHS App.",
            "appointment hub" to "You'll need to prove who you are before you can book appointments in the NHS App.",
            "gp medical record"
                    to healthRecordsDescription,
            "organ donation" to healthRecordsDescription,
            "your prescriptions" to "You'll need to prove who you are before you can order repeat prescriptions " +
                    "in the NHS App.",
            "silver integration feature" to genericDescription
    )

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
        )
    }

    fun assertUpliftBanner(page: String = "default") {
        headerElement.assertSingleElementPresent()
        descriptionMap[page]?.let {
            descriptionElement.withText(it).assertSingleElementPresent()
        } ?: Assert.fail("Test setup error: cannot find the uplift banner description for the '$page' page")
    }
}
