package mongodb

import org.bson.json.Converter
import org.bson.json.StrictJsonWriter
import java.util.*
import java.time.ZoneId
import java.time.format.DateTimeFormatter



class JsonDateTimeConverter: Converter<Long> {
    override fun convert(value: Long?, writer: StrictJsonWriter?) {
        if (value != null) {
            val instant = Date(value).toInstant()
            val s = DATE_TIME_FORMATTER.format(instant)
            writer!!.writeString(s)
        }
    }

    companion object {
        private val DATE_TIME_FORMATTER = DateTimeFormatter.ISO_INSTANT
            .withZone(ZoneId.of("UTC"))
    }
}
