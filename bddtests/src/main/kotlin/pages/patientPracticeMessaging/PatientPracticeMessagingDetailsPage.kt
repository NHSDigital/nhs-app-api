package pages.patientPracticeMessaging

import org.junit.Assert.assertEquals
import mocking.emis.patientPracticeMessaging.MessageDetails
import mocking.emis.patientPracticeMessaging.MessageReply
import pages.HybridPageElement
import pages.HybridPageObject
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

    fun assertRepliesCorrect(expectedReplies: List<MessageReply>){
        for (expectedReply in expectedReplies) {
            setMessageReplyId(expectedReplies.indexOf(expectedReply))
            assertEquals(assertReplyTextCorrect().text, expectedReply.replyContent)
        }
    }

    fun assertSentMessageCorrect(message: MessageDetails){
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSentPanel']//p",
                androidLocator = null,
                page = this)

        return assertEquals(sentMessage.text, message.content)
    }

    fun assertSentSubjectCorrect(message: MessageDetails){
        val sentMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSubject']",
                androidLocator = null,
                page = this)

        return assertEquals(sentMessage.text, message.subject)
    }

    fun assertSentDateTimeCorrect(){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageSentDateTime']",
                androidLocator = null,
                page = this)

        return assertEquals(sentDateTimeMessage.text, "Sent 05 December 2019 at 1:39pm")
    }

    fun assertReceivedDateTimeCorrect(){
        val sentDateTimeMessage =  HybridPageElement(
                webDesktopLocator = "//*[@id='messageReplyDateTime0']",
                androidLocator = null,
                page = this)

        return assertEquals(sentDateTimeMessage.text, "Sent 05 December 2019 at 1:39pm")
    }

    private fun setMessageReplyId(replyId: Int) {
        baseReplyPath = "//*[@id='messageReplyPanel$replyId']"
    }

    private fun assertReplyTextCorrect(): HybridPageElement{
        return HybridPageElement(
                webDesktopLocator = "$baseReplyPath//p",
                androidLocator = null,
                page = this)
    }

}