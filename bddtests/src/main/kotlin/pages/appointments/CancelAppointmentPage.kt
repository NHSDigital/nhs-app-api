package pages.appointments

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/appointments")
class CancelAppointmentPage : AppointmentSharedElementsPage() {

    @FindBy(xpath = "//main/div/p")
    private lateinit var checkDetailsText: WebElementFacade

    @FindBy(xpath = "//label[@for='txt_reason']")
    private lateinit var dropDownMenuLabel: WebElementFacade

    @FindBy(xpath = "//select[@id='txt_reason']")
    private lateinit var dropDownMenu: WebElementFacade


    fun getCheckDetailsText(): String {
        return checkDetailsText.text
    }

    fun getReasonDropDownLabelText(): String {
        return dropDownMenuLabel.text
    }

    fun selectReason(reason: String): Boolean {
        return try {
            dropDownMenu.selectByVisibleText<WebElementFacade>(reason)
            true
        } catch (e: NoSuchElementException) {
            false
        }
    }
}
