package pages.sharedElements.expectedPage

class ExpectedPageStructureContents : ExpectedPageStructureBase<ExpectedPageStructureContents>(){

    fun selectedListItem(content:String): ExpectedPageStructureContents {
        expectedElements.add(content, "li")
        expectedElements.add(content, "span")
        return this
    }

    override fun superType(): ExpectedPageStructureContents = this
}
