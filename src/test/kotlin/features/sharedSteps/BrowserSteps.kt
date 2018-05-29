package features.sharedSteps

import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.LoginPage
import java.net.URL
import java.util.NoSuchElementException
import kotlin.reflect.full.findAnnotation

open class BrowserSteps {

    lateinit var loginPage: LoginPage

    @Step
    open fun goToApp() {
        loginPage.open()
    }

    @Step
    open fun browseTo(url: String) {
        loginPage.driver.get(url)
    }

    @Step
    open fun shouldHaveTitle(title: String) {
        Assert.assertEquals(title, loginPage.title)
    }

    @Step
    open fun shouldHaveUrl(url: String) {
        Assert.assertEquals(url, loginPage.driver.currentUrl)
    }

    @Step
    fun changeTab(url: URL) {
        val driver = loginPage.driver

        var allWindows: MutableList<String> = mutableListOf()

        for (window in loginPage.driver.windowHandles) {
            driver.switchTo().window(window)
            allWindows.add(0, driver.currentUrl)
            if (url.host.equals(URL(driver.currentUrl).host)) {
                return
            }
        }

        throw NoSuchElementException("No tab found with $url. All windows: ${allWindows.reversed()}")
    }

    @Step
    fun changeTabToApp() {
        changeTab(URL(loginPage::class.findAnnotation<DefaultUrl>()?.value))
    }
}