package pages.patientPracticeMessaging

import mocking.patientPracticeMessaging.MessageReply
import org.junit.Assert.assertEquals
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isVisible
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
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='${id}']//p",
                androidLocator = null,
                page = this)

        return assertEquals(sentMessage.text, content)
    }

    fun assertSentSubjectCorrect(subject: String){
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='initialMessageSubject0']",
                androidLocator = null,
                page = this)

        return assertEquals(sentMessage.text, subject)
    }

    fun assertUnreadDividerIsOnSceen(){
        val pageDivider =  HybridPageElement(
                webDesktopLocator = "//*[@id='receivedMessagesDivider']",
                androidLocator = null,
                page = this)

        return assert(pageDivider.isVisible)
    }

    fun assertDateTimeCorrect(expectedSentMessageDate: String, id: String){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='${id}']",
                androidLocator = null,
                page = this)

        return assertEquals(expectedSentMessageDate, sentDateTimeMessage.text)
    }

    private fun getReceivedDateTimeMessage(index: Int, read: Boolean): HybridPageElement {
        val idPrefix = if (read) "readMessageReplyDateTime" else "unreadMessageReplyDateTime"
        return HybridPageElement(
            webDesktopLocator = "//*[@id='$idPrefix$index']",
            androidLocator = null,
            page = this)
    }

    fun assertReceivedDateTimesCorrect(expectedDates: List<String>, read: Boolean){
        expectedDates.forEachIndexed(fun (index, date) {
            assertEquals(date, getReceivedDateTimeMessage(index, read).text)
        })
    }

    private fun setReadMessageReplyId(replyId: Int) {
        baseReplyPath = "//*[@id='readMessageReplyPanel$replyId']"
    }

    private fun setUnreadMessageReplyId(replyId: Int) {
        baseReplyPath = "//*[@id='unreadMessageReplyPanel$replyId']"
    }

    private fun assertReplyTextCorrect(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseReplyPath//p",
                androidLocator = null,
                page = this)
    }

    fun clickDeleteConversation() {
        HybridPageElement(
                webDesktopLocator = "//a[@id='deleteMessage']",
                androidLocator = null,
                page= this
        ).click()
    }

}