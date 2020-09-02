package pages.sharedElements.expectedPage

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

    fun toggle(title: String, content: String): ExpectedPageStructure {
        expectedElements.add("${title}\n${content}", "label")
        expectedElements.add(content, "span")
        return this
    }

    fun details(title:String, content:List<String>) : ExpectedPageStructure {
        expectedElements.add(title.trim(), "details")
        expectedElements.add(title.trim(), "summary")
        expectedElements.add(title.trim(), "span")
        content.forEach { element -> expectedElements.add(element, "li") }
        return this
    }

    fun menuLinks(links:List<String>) : ExpectedPageStructure {
        links.forEach { element ->
            expectedElements.add(element, "li")
            expectedElements.add(element, "span")
            expectedElements.add(element, "h3") }
        return this
    }

    fun dropdown(label: String, options: List<String> ): ExpectedPageStructure {
        expectedElements.add(label, "label")
        //skip assertions of these tags, as they repeat the content in the options with whitespace
        expectedElements.add(ExpectedPageStructureElement("", "span",
                assertionOverride = { }))
        expectedElements.add(ExpectedPageStructureElement("", "select",
                assertionOverride = {}))
        options.forEach { element -> expectedElements.add(element, "option") }
        return this
    }

    override fun superType(): ExpectedPageStructure = this
}
