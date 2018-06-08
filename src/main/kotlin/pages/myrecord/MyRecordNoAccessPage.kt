package pages.myrecord

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.PageObject
import net.serenitybdd.core.pages.WebElementFacade

class MyRecordNoAccessPage : PageObject() {

    @FindBy(id = "")
    lateinit var txtNoAccess: WebElementFacade

    @FindBy(id = "")
    lateinit var txtGPTxt: WebElementFacade

    fun isOnNoAccessPage(): Boolean {
        return txtNoAccess.isCurrentlyVisible
    }

    fun getNoAccessText(): String {
        return txtNoAccess.text
    }

    fun getContactGPText(): String {
        return txtGPTxt.text
    }
}
