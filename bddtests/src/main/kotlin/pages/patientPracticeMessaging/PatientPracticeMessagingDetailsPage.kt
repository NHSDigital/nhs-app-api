package pages.patientPracticeMessaging

import org.junit.Assert.assertEquals
import mocking.emis.patientPracticeMessaging.MessageReply
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
            if (!expectedReply.isUnread) {
                setReadMessageReplyId(expectedReplies.indexOf(expectedReply))
                assertEquals(assertReplyTextCorrect().text, expectedReply.replyContent)
            }
        }
    }

    fun assertUnreadRepliesCorrect(expectedReplies: List<MessageReply>){
        for (expectedReply in expectedReplies) {
            if (expectedReply.isUnread) {
                setUnreadMessageReplyId(expectedReplies.indexOf(expectedReply))
                assertEquals(assertReplyTextCorrect().text, expectedReply.replyContent)
            }
        }
    }

    fun assertSentMessageCorrect(content: String){
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSentPanel']//p",
                androidLocator = null,
                page = this)

        return assertEquals(sentMessage.text, content)
    }

    fun assertSentSubjectCorrect(subject: String){
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSubject']",
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

    fun assertSentDateTimeCorrect(){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSentDateTime']",
                androidLocator = null,
                page = this)

        return assertEquals(sentDateTimeMessage.text, "Sent 05 December 2019 at 1:39pm")
    }

    fun assertReadReceivedDateTimeCorrect(){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='readMessageReplyDateTime0']",
                androidLocator = null,
                page = this)

        return assertEquals(sentDateTimeMessage.text, "Sent 05 December 2019 at 1:39pm")
    }

    fun assertUnreadReceivedDateTimeCorrect(){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='unreadMessageReplyDateTime0']",
                androidLocator = null,
                page = this)

        return assertEquals(sentDateTimeMessage.text, "Sent 05 December 2019 at 1:39pm")
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

}