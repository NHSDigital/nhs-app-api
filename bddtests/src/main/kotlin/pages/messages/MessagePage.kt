package pages.messages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/messages/app-messaging/app-message")
class MessagePage : MessagesBasePage() {
    override val titleText: String = "Message from"
}
