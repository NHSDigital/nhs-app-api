package pages.sharedElements.expectedPage

class ExpectedPageStructurePagination {

    private val expectedElements = mutableListOf<ExpectedPageStructureElement>()

    fun next(title:String): ExpectedPageStructurePagination {
        add("Next", title)
        return this
    }

    fun previous(title:String): ExpectedPageStructurePagination {
        add("Previous", title)
        return this
    }

   private fun add(direction: String, title: String) {
        expectedElements.add("$direction\n:\n$title", "li")
        expectedElements.add(direction, "span")
        expectedElements.add(":", "span")
        expectedElements.add(title, "span")
    }

    fun build(): MutableList<ExpectedPageStructureElement> {
        return expectedElements
    }
}
