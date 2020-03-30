import org.openqa.selenium.WebElement

class ParsedPageElement(val element: WebElement)
{
    val tag = element.tagName
    val content = element.text.trim()
}