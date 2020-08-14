package pages

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement

class InterruptionCard(private val path: String,private val page: HybridPageObject){

    private var found = false

    fun assertContent(expectedHeader:String, expectedContent:String, expectedLinkText:String) {
        initialiseIfNeeded()
        Assert.assertEquals("Interruption Card header", expectedHeader, header)
        Assert.assertEquals("Interruption Card body", expectedContent, body)
        Assert.assertEquals("Interruption Card link", expectedLinkText , findOutMoreLink!!.text)
    }

    fun assertContinueAndClick(continueUrl: String) {
        initialiseIfNeeded()
        element!!.actOnTheElement {
            val continueLinkForm =
                    it.findElement<WebElement>(By.xpath("//a[normalize-space(text())='Continue']"))
            val actualUrl = continueLinkForm.getAttribute("href")
            Assert.assertTrue("Interruption Card Continue Url. Expected to start with $continueUrl, but was $actualUrl",
                    actualUrl.startsWith(continueUrl))
            continueLinkForm.click()
        }
    }

    private fun initialiseIfNeeded(){
        if(!found){
            element = HybridPageElement(
                    webDesktopLocator = path,
                    page = page
            )
            element!!.actOnTheElement {
                header =
                        it.findElement<WebElement>(By.xpath("//h2")).text
                body =
                        it.findElement<WebElement>(By.xpath("//p")).text
                findOutMoreLink =
                        it.findElement(By.xpath("//p/a"))
            }
            found =true
        }
    }

    private var header: String =""
    private var body:String=""
    private var findOutMoreLink: WebElement? = null
    private var element: HybridPageElement? = null
}
