package webdrivers

import io.appium.java_client.AppiumDriver
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.ios.IOSDriver
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.webdriver.WebDriverFacade
import org.openqa.selenium.MutableCapabilities
import org.openqa.selenium.WebDriver
import pages.HybridPageElement
import pages.LocatorStrategy
import webdrivers.options.OptionManager
import webdrivers.options.device.DeviceWebMobile

fun WebDriver.isIOS(): Boolean {
    val isIOS = this is IOSDriver<*>
    val isProxyForIOS = when (this is WebDriverFacade) {
        true -> {
            (this).isAProxyFor(IOSDriver::class.java)
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
            (this).isAProxyFor(AndroidDriver::class.java)
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

fun WebDriver.getLocatorStrategy(element: HybridPageElement): LocatorStrategy {
    return if (this.isAndroid() && element.androidLocator != null) {
        LocatorStrategy.ANDROID
    } else if (this.isIOS() && element.iOSLocator != null) {
        LocatorStrategy.IOS
    } else if (!this.isIOS() &&
            !this.isAndroid() &&
            OptionManager.instance().isEnabled(DeviceWebMobile::class)) {
        LocatorStrategy.BROWSER_MOBILE
    } else {
        LocatorStrategy.BROWSER_DESKTOP
    }
}

fun MutableCapabilities.setCapabilityIfNotNull(name: String, value: String?) {
    if (value != null) {
        this.setCapability(name, value)
    }
}
