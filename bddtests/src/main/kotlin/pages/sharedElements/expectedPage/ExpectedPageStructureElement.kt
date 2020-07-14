package pages.sharedElements.expectedPage

import org.junit.Assert

class ExpectedPageStructureElement(val content:String,
                                   val tag:String,
                                   val assertionOverride :((ParsedPageElement)->Unit)? = null) {
    fun assert(actualElement: ParsedPageElement) {
        if (assertionOverride == null) {
            basicTextAssert(actualElement)
        } else {
            Assert.assertEquals("Expected tags to be equal for assertion override", tag, actualElement.tag)
            assertionOverride.invoke(actualElement)
        }
    }

    private fun basicTextAssert(actualElement: ParsedPageElement) {
        if (content != actualElement.content || tag != actualElement.tag) {
            Assert.fail("Page Element Unexpected\n" +
                    "Expected: Tag = '$tag', Content = '$content'\n" +
                    "Actual: Tag = '${actualElement.tag}', Content = '${actualElement.content}'")
        }
    }
}

fun MutableList<ExpectedPageStructureElement>.add(content: String, tag: String) {
    this.add(ExpectedPageStructureElement(content, tag))
}
