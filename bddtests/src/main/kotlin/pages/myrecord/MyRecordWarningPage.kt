package pages.myrecord

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By

class MyRecordWarningPage : PageObject() {

    @FindBy(xpath = "//*[@id='app']/header/h1")
    lateinit var lblHeader: WebElementFacade

    @FindBy(xpath = "//div[@class='msg warning']")
    lateinit var txtWarning: WebElementFacade

    @FindBy(xpath = "//button[contains(text(),'Agree and continue')]")
    lateinit var btnAgree: WebElementFacade

    @FindBy(xpath = "//button[contains(text(),'Back to home')]")
    lateinit var btnBack2Home: WebElementFacade


    fun isBackToHomePresent(): Boolean {
        return btnBack2Home.isCurrentlyVisible
    }

    fun isAgreePresent(): Boolean {
        return btnAgree.isCurrentlyVisible
    }

    fun getHeaderText(): String {
        return lblHeader.text
    }

    fun warningText(): String {
        return txtWarning.text
    }

    fun isWarningMsgHighlighted(): String {
        return txtWarning.getCssValue("background-color")
    }

    fun clickAgreeandContinue() {
        btnAgree.waitUntilVisible<WebElementFacade>()
        btnAgree.click()
    }

    fun clickBacktoHome() {
        evaluateJavascript("arguments[0].scrollIntoView(true);", btnBack2Home);
        btnBack2Home.click()
    }

    fun getSensitiveList(): ArrayList<String> {
        var list = ArrayList<String>()
        val listSensitiveData = findAll(By.xpath("//div[@class='info']/ul/li"))
        listSensitiveData.forEach { el ->
            list.add(el.text)
        }
        return list
    }
}

