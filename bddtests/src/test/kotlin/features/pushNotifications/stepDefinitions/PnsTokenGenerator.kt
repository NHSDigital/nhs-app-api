package features.pushNotifications.stepDefinitions

import java.time.Instant
import java.time.ZoneOffset
import java.time.format.DateTimeFormatter
import java.util.*

class PnsTokenGenerator{
    companion object {
        fun generate(): String {
            val timeStamp = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss")
                    .withZone(ZoneOffset.UTC)
                    .format(Instant.now())
            return UUID.randomUUID().toString() + "-" + timeStamp
        }
    }
}