package pages.messages

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/messages/app-messaging/app-message")
class MessagePage : MessagesBasePage() {
    override val titleText: String = "Message from"

    fun getReplyButtonElement() : HybridPageElement {
        return  getElement("//button[@id='showKeywordReplies']")
    }

    fun assertRadioButtonOptionsExist() : HybridPageElement {
        val path = "//div[@id='radioOptions']"
        return getElement(path).assertIsVisible()
    }

    fun clickReplyButtonElement() {
        getElement("//button[@id='showKeywordReplies']").click()
    }

    fun assertResponseContainerExists() {
        val path = "//div[@id='messageReplyResponseContainer']"
        getElement(path, timeToWaitForElement = 30).assertIsVisible()
    }

    fun selectAnOption() {
        val path = "//input[@name='replyoptions']"
        getElement(path).click()
    }

    fun sendTheReply() {
        clickOnButtonContainingText("Send")
    }

}
