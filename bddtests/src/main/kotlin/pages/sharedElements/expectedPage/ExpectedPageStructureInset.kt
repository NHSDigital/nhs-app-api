package pages.sharedElements.expectedPage

class ExpectedPageStructureInset : ExpectedPageStructureBase<ExpectedPageStructureInset>(){
    init {
        expectedElements.add("Information:", "span")
    }

    override fun superType(): ExpectedPageStructureInset = this
}
