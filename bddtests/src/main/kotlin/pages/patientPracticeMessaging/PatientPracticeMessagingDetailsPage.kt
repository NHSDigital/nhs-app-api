package pages.patientPracticeMessaging

import mocking.patientPracticeMessaging.MessageReply
import org.junit.Assert.assertEquals
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.text

class PatientPracticeMessagingDetailsPage: HybridPageObject() {

    private var baseReplyPath: String = ""

    fun assertDisplayed() {
        val path = "//h1[normalize-space(text())='Messages']"
        val header = HybridPageElement(
                path,
                path,
                null,
                null,
                this,
                helpfulName = "header")
        header.waitForElement()
    }

    val attachment = getElementById("//div[@id='document']")

    fun assertReadRepliesCorrect(expectedReplies: List<MessageReply>){
        for (expectedReply in expectedReplies) {
            if (!expectedReply.isUnread!!) {
                setReadMessageReplyId(expectedReplies.indexOf(expectedReply))
                assertEquals(assertReplyTextCorrect().text, expectedReply.replyContent)
            }
        }
    }

    fun assertUnreadRepliesCorrect(expectedReplies: List<MessageReply>){
        for (expectedReply in expectedReplies) {
            if (expectedReply.isUnread!!) {
                setUnreadMessageReplyId(expectedReplies.indexOf(expectedReply))
                assertEquals(assertReplyTextCorrect().text, expectedReply.replyContent)
            }
        }
    }

    fun assertMessageCorrect(content: String, id: String){
        val sentMessage =  getElementById("//*[@id='${id}']//p")
        return assertEquals(sentMessage.text, content)
    }

    fun assertSentSubjectCorrect(subject: String){
        val sentMessage =  getElementById("//*[@id='initialMessageSubject0']")
        return assertEquals(sentMessage.text, subject)
    }

    fun assertUnreadDividerIsOnSceen(){
        val pageDivider = getElementById("//*[@id='receivedMessagesDivider']")
        pageDivider.assertIsVisible()
    }

    fun assertDateTimeCorrect(expectedSentMessageDate: String, id: String){
        val sentDateTimeMessage =  getElementById("//*[@id='${id}']")
        return assertEquals(expectedSentMessageDate, sentDateTimeMessage.text)
    }

    private fun getReceivedDateTimeMessage(index: Int, read: Boolean): HybridPageElement {
        val idPrefix = if (read) "readMessageReplyDateTime" else "unreadMessageReplyDateTime"
        return getElementById("//*[@id='$idPrefix$index']")
    }

    fun assertReceivedDateTimesCorrect(expectedDates: List<String>, read: Boolean){
        expectedDates.forEachIndexed(fun (index, date) {
            assertEquals(date, getReceivedDateTimeMessage(index, read).text)
        })
    }

    fun assertLink(linkType: String){
        getElementById("//*[@id='$linkType']").assertIsVisible()
    }

    fun clickLink(linkType: String){
        getElementById("//*[@id='$linkType']").click()
    }


    private fun setReadMessageReplyId(replyId: Int) {
        baseReplyPath = "//*[@id='readMessageReplyPanel$replyId']"
    }

    private fun setUnreadMessageReplyId(replyId: Int) {
        baseReplyPath = "//*[@id='unreadMessageReplyPanel$replyId']"
    }

    private fun assertReplyTextCorrect(): HybridPageElement{
        return getElementById("$baseReplyPath//p")
    }

    private fun getElementById(locator: String): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = locator,
                androidLocator = null,
                page = this)
    }

    fun clickDeleteConversation() {
        getElementById("//a[@id='deleteMessage']").click()
    }

}
