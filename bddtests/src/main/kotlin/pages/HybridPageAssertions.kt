package pages

import org.junit.Assert

fun HybridPageElement.assertSingleElementPresent(): HybridPageElement {
    Assert.assertEquals(
            "Expected only one matching element for $helpfulNameToUse, with xpath $webDesktopLocator",
            1,
            elements.count())
    return this
}

fun HybridPageElement.assertIsVisible(): HybridPageElement {
    Assert.assertTrue("Expected $helpfulNameToUse to be visible", element.isVisible)
    return this
}

fun HybridPageElement.assertDoesElementHaveFocus(): HybridPageElement {
    Assert.assertTrue("Expected $helpfulNameToUse to be visible", element.hasFocus())
    return this
}

fun HybridPageElement.assertIsNotVisible(): HybridPageElement {
    Assert.assertFalse("Expected $helpfulNameToUse to not be visible", element.isVisible)
    return this
}

fun HybridPageElement.assertElementNotPresent(): HybridPageElement {
    Assert.assertEquals("Expected no matching elements for $helpfulNameToUse", 0, withoutRetrying().elements.count())
    return this
}

fun HybridPageElement.withoutRetrying(): HybridPageElement {
    this.timeToWaitForElement = 0
    return this
}
