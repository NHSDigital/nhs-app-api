package pages.sharedElements.expectedPage

import add

class ExpectedPageStructure : ExpectedPageStructureBase<ExpectedPageStructure>(){

    fun contents(contents: (ExpectedPageStructureContents.() -> ExpectedPageStructureContents)): ExpectedPageStructure {
        val expectedContentsElementsBuilder = ExpectedPageStructureContents()
        contents.invoke(expectedContentsElementsBuilder)
        expectedElements.addAll(expectedContentsElementsBuilder.build())
        return this
    }

    fun inset(contents: (ExpectedPageStructureInset.() -> ExpectedPageStructureInset)): ExpectedPageStructure {
        val expectedInsetElementBuilder = ExpectedPageStructureInset()
        contents.invoke(expectedInsetElementBuilder)
        expectedElements.addAll(expectedInsetElementBuilder.build())
        return this
    }

    fun button(content: String): ExpectedPageStructure {
        expectedElements.add(content, "button")
        return this
    }

    fun pagination(contents: (ExpectedPageStructurePagination.() -> ExpectedPageStructurePagination))
            : ExpectedPageStructure {
        val builder = ExpectedPageStructurePagination()
        contents.invoke(builder)
        expectedElements.addAll(builder.build())
        return this
    }

    override fun superType(): ExpectedPageStructure = this
}

