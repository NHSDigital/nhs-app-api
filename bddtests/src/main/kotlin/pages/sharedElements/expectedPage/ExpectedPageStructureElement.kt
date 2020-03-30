import org.junit.Assert

class ExpectedPageStructureElement(val content:String, val tag:String)
{
    fun assert(actualElement: ParsedPageElement) {
        if(content != actualElement.content || tag != actualElement.tag){
            Assert.fail("Page Element Unexpected\n" +
                    "Expected: Tag = '$tag', Content = '$content'\n" +
                    "Actual: Tag = '${actualElement.tag}', Content = '${actualElement.content}'")
        }
    }
}

fun MutableList<ExpectedPageStructureElement>.add(content: String, tag: String){
    this.add(ExpectedPageStructureElement(content, tag))
}