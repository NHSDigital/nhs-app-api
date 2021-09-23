package pages.messages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/messages/app-messaging/sender-messages")
class MessagesPage : MessagesBasePage() {
    override val titleText: String = "Messages from"

    val messages by lazy { MessageBlockElements(this) }
}
