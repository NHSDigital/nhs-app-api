package webdrivers

import io.appium.java_client.AppiumDriver
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.ios.IOSDriver
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.WebDriverFacade
import org.openqa.selenium.WebDriver
import java.net.URL

private const val NEW_TAB_DELAY = 1000L

fun WebDriver.isIOS(): Boolean {
    val isIOS = this is IOSDriver<*>
    val isProxyForIOS = when (this is WebDriverFacade) {
        true -> {
            (this as WebDriverFacade).isAProxyFor(IOSDriver::class.java)
        }
        false -> {
            false
        }
    }
    return isIOS.xor(isProxyForIOS)
}

fun WebDriver.isAndroid(): Boolean {
    val isAndroid = this is AndroidDriver<*>
    val isProxyForAndroid = when (this is WebDriverFacade) {
        true -> {
            (this as WebDriverFacade).isAProxyFor(AndroidDriver::class.java)
        }
        false -> {
            false
        }
    }
    return isAndroid.xor(isProxyForAndroid)
}

fun <T> WebDriver.getSpecificDriver(): T {
    val theDriver =
            if (this is WebDriverFacade) {
                this.proxiedDriver
            } else {
                this
            }
    @Suppress("UNCHECKED_CAST",
            "Cast cannot be checked as a generic type is used, " +
                    "see https://kotlinlang.org/docs/reference/typecasts.html")
    return theDriver as T
}


fun WebDriver.getMobileDriver(): AppiumDriver<WebElementFacade> {
    return when (this.isAndroid()) {
        true -> {
            this.getSpecificDriver<AndroidDriver<WebElementFacade>>()
        }
        false -> {
            this.getSpecificDriver<IOSDriver<WebElementFacade>>()
        }
    }
}

fun WebDriver.getOpenTabUrls(): List<URL> {
    // Chrome can lockup if we switch to a new tab too quickly
    // This does not throw an exception that we can catch and retry with
    // A sleep is the only way we have found to wait for the tab to load enough
    // to find the currentUrl
    Thread.sleep(NEW_TAB_DELAY)
    return windowHandles.map { windowHandle -> getTabUrl(windowHandle)}
}

private fun WebDriver.getTabUrl(windowHandle :String): URL {
    val openedTab = switchTo().window(windowHandle)
    val openedTabUrlString = openedTab.currentUrl
    return URL(openedTabUrlString)
}