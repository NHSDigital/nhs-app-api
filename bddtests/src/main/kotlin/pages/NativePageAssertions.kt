package pages

import org.junit.Assert


fun NativePageElement.assertIsDisplayed(message: String? = null): NativePageElement {
    actOnTheNativeElement {
        Assert.assertTrue(message ?: "Expected $helpfulNameToUse to be displayed", it.isDisplayed)
    }
    return this
}

fun NativePageElement.assertElementNotPresent(): NativePageElement {
    val elementsFound = withoutRetrying().elements.count()
    Assert.assertEquals("Expected no matching elements for $helpfulNameToUse", 0, elementsFound)
    return this
}
