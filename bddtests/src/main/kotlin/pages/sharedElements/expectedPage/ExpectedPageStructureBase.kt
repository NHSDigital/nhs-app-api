package pages.sharedElements.expectedPage

import ExpectedPageStructureElement
import add
import pages.HybridPageObject

abstract class ExpectedPageStructureBase<T> {

    protected val expectedElements = mutableListOf<ExpectedPageStructureElement>()

    fun paragraph(content: String): T {
        expectedElements.add(content, "p")
        return superType()
    }

    fun h2(content: String): T {
        expectedElements.add(content, "h2")
        return superType()
    }

    fun h3(content: String): T {
        expectedElements.add(content, "h3")
        return superType()
    }

    fun listItem(content: String): T {
        expectedElements.add(content, "li")
        return superType()
    }

    fun listItems(vararg content: String): T {
        content.forEach { element -> expectedElements.add(element, "li") }
        return superType()
    }

    fun build(): MutableList<ExpectedPageStructureElement> {
        return expectedElements
    }

    fun assert(page: HybridPageObject) {
        ExpectedPageStructureAssertor().assert(page, build())
    }

    protected abstract fun superType(): T
}