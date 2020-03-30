package pages.sharedElements.expectedPage

import add

class ExpectedPageStructureInset : ExpectedPageStructureBase<ExpectedPageStructureInset>(){
    init {
        expectedElements.add("Information:", "span")
    }

    override fun superType(): ExpectedPageStructureInset = this
}