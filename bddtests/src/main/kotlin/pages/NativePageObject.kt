package pages

import io.appium.java_client.AppiumDriver
import io.appium.java_client.MobileElement
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.ios.IOSDriver
import org.openqa.selenium.NoSuchElementException
import org.openqa.selenium.WebDriverException


const val NATIVE_CONTEXT: String = "native"

abstract class NativePageObject : HybridPageObject() {

    enum class AppAction {
        Launch,
        Activate
    }

    private fun getNativeMobileDriver(): AppiumDriver<MobileElement> {
        return when (isAndroid()) {
            true -> {
                getSpecificDriver<AndroidDriver<MobileElement>>()
            }
            false -> {
                getSpecificDriver<IOSDriver<MobileElement>>()
            }
        }
    }

    fun switchNative() {
        val driver = getNativeMobileDriver();
        if (driver.context.contains(NATIVE_CONTEXT, ignoreCase = true)) {
            println("Already in $NATIVE_CONTEXT context: ${driver.context}")
        } else {
            for (context in driver.contextHandles) {
                if (context.contains(NATIVE_CONTEXT, true)) {
                    println("Switching context to $context... Currently on: ${driver.context}")
                    driver.context(context)
                    println("Switched context! Now on: ${driver.context}")
                    break
                }
            }
        }
        setDriver<NativePageObject>(driver)
    }

    fun switchToApp(appBundleId: String, action: AppAction) {
        try {
            when (action) {
                AppAction.Launch -> {
                    launchApp(appBundleId)
                }
                AppAction.Activate -> {
                    activateApp(appBundleId)
                }
            }
        } catch (e: WebDriverException) {
            println("app switching failed " + e)
        }
    }

    //launch an App thats not yet open
    fun launchApp(appBundleId: String) {
        val driver = getNativeMobileDriver()
        val bundleId = HashMap<String, String>()
        bundleId.put("bundleId", appBundleId)
        driver.executeScript("mobile: launchApp", bundleId)
    }

    //activating an App thats already been opened
    fun activateApp(appBundleId: String) {
        val driver = getNativeMobileDriver()
        val bundleId = HashMap<String, String>()
        bundleId.put("bundleId", appBundleId)
        driver.executeScript("mobile: activateApp", bundleId)
    }

    fun findByAccessibilityId(accessibiltyId: String): MobileElement {
        switchNative()
        val elementNative: MobileElement
        try {
            logSelectorAndSource(accessibiltyId)
            elementNative = getNativeMobileDriver().findElementByAccessibilityId(accessibiltyId)

        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return elementNative

    }

    fun findNativeByXpath(xpath: String): MobileElement {
        switchNative()

        val elementNative: MobileElement
        try {
            logSelectorAndSource(xpath)
            elementNative = getNativeMobileDriver().findElementByXPath(xpath)
        } catch (e: NoSuchElementException) {
            throw NoSuchElementException("No element found on page:\n${driver.pageSource}", e)
        }
        return elementNative
    }
}
