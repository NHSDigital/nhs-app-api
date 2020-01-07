package features.loggedOut.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.isVisible
import pages.loggedOut.CookieBanner


open class CookieBannerSteps {

    private lateinit var cookieBanner: CookieBanner

    private val Boolean.int
        get() = if (this) 1 else 0
    private val Boolean.string
        get() = if (this) "is not" else "is"

    @Step
    fun iSeeCookieBanner(isVisible: Boolean) {
        Assert.assertEquals(
                "Cookie banner ${isVisible.string} displayed. ",
                isVisible.int,
                cookieBanner.cookieBanner.elements.size
        )
        Assert.assertEquals(
                "Cookie banner first paragraph ${isVisible.string} displayed. ",
                isVisible.int,
                cookieBanner.cookieBannerText1.elements.size
        )
        Assert.assertEquals(
                "Cookie banner second paragraph ${isVisible.string} displayed. ",
                isVisible.int,
                cookieBanner.cookieBannerText2.elements.size
        )
        Assert.assertEquals(
                "Link in cookie banner ${isVisible.string} displayed. ",
                isVisible.int,
                cookieBanner.cookiesInformationLink.elements.size
        )
    }

    @Step
    fun iDoNotSeeCookieBannerNoJs() {
        Assert.assertTrue(
                "Cookie banner is not visible. ",
                !cookieBanner.cookieWrapper.isVisible
        )
    }
}
