package pages

import org.junit.Assert

fun HybridPageElement.assertSingleElementPresent(message: String? = null): HybridPageElement {
    Assert.assertEquals(
            message ?: "Expected only one matching element for $helpfulNameToUse, with xpath $webDesktopLocator",
            1,
            elements.count())
    return this
}

fun HybridPageElement.assertIsVisible(message: String? = null): HybridPageElement {
    actOnTheElement {
        Assert.assertTrue(message ?: "Expected $helpfulNameToUse to be visible", it.isVisible)
    }
    return this
}

fun HybridPageElement.assertIsDisplayed(message:String) : HybridPageElement {
    actOnTheElement { Assert.assertTrue(message, it.isDisplayed) }
    return this
}

fun HybridPageElement.assertIsCurrentlyEnabled(message:String) : HybridPageElement {
    actOnTheElement { Assert.assertTrue(message, it.isCurrentlyEnabled) }
    return this
}

fun HybridPageElement.assertDoesElementHaveFocus(): HybridPageElement {
    actOnTheElement {
        Assert.assertTrue("Expected $helpfulNameToUse to be visible", it.hasFocus())
    }
    return this
}

fun HybridPageElement.assertIsNotVisible(): HybridPageElement {
    actOnTheElement {
        Assert.assertFalse("Expected $helpfulNameToUse to not be visible", it.isVisible)
    }
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

fun HybridPageElement.assertCurrentlyVisible() {
    actOnTheElement {
        Assert.assertTrue(
                "Expected element $helpfulNameToUse, with xpath $webDesktopLocator to be visible",
                it.isCurrentlyVisible)
    }
}
