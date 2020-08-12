package pages

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import org.openqa.selenium.WebElement

const val WAIT_FOR_NON_STALE_ELEMENT = 500L
const val UNICODE_HYPHEN = 8208.toChar().toString()

fun HybridPageElement.withNormalisedText(text: String): HybridPageElement =
        this.withText(text, exact = true, normalised = true)

val HybridPageElement.text: String
  get() {
      return textValue
  }

val HybridPageElement.value: String
    get() {
        var value = ""
        actOnTheElement { value = it.value }
        return value
    }

fun HybridPageElement.sendKeys(keysToSend: CharSequence?) {
    actOnTheElement { it.sendKeys(keysToSend) }
}

fun HybridPageElement.waitForElementToBecomeVisible() : HybridPageElement {
    actOnTheElement {
        while (true) {
            val wrappedElement = it.wrappedElement
            if (wrappedElement.size != null || it.isVisible)
                break
            Thread.sleep(WAIT_FOR_NON_STALE_ELEMENT)
        }
    }

    return this
}

fun NativePageElement.waitForNativeElementToBecomeVisible() : NativePageElement {
    actOnTheNativeElement {
        while (true) {
            val wrappedElement = it
            if (wrappedElement.size != null || it.isDisplayed)
                break
            Thread.sleep(WAIT_FOR_NON_STALE_ELEMENT)
        }
    }

    return this
}

fun HybridPageElement.waitUntilPresent() {
    actOnTheElement { it.waitUntilPresent<WebElementFacade>() }
}

fun HybridPageElement.typeTextIntoTextArea(text: String): String {
    //Each letter sent individually
    //This doesn't add a lot of time onto the test, but does help to ensure the full text is typed
    //Keys can sometimes go missing; so we return the actual text that got typed and assert that something went in
    var returnedText: String = ""
    text.toCharArray().map { letter ->
        actOnTheElement { elem ->
            elem.sendKeys(letter.toString())
        }
    }

    actOnTheElement { elem -> returnedText = elem.value }

    Assert.assertTrue("Expected some text to be output to the text area", returnedText.isNotEmpty())

    return returnedText
}

val WebElement.asciiText :String
    get() {
        return text.replace(UNICODE_HYPHEN,"-")
}
