package pages

import io.appium.java_client.AppiumDriver
import io.appium.java_client.MobileElement
import io.appium.java_client.android.AndroidDriver
import io.appium.java_client.ios.IOSDriver
import org.openqa.selenium.NoSuchElementException

const val NATIVE_CONTEXT: String = "native"

abstract class NativePageObject : HybridPageObject() {

    private fun getNativeMobileDriver(): AppiumDriver<MobileElement> {
         return when(isAndroid()) {
            true -> {
                getSpecificDriver<AndroidDriver<MobileElement>>()
            }
            false -> {
                getSpecificDriver<IOSDriver<MobileElement> >()
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