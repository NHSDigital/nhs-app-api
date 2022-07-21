package mongodb

import com.google.gson.TypeAdapter
import com.google.gson.stream.JsonReader
import com.google.gson.stream.JsonWriter
import org.bson.types.ObjectId

class ObjectIdTypeAdapter : TypeAdapter<ObjectId>() {
    override fun write(writer: JsonWriter, value: ObjectId?) {
        writer.beginObject()
            .name("$value")
            .value(value.toString())
            .endObject()
    }

    override fun read(reader: JsonReader): ObjectId {
        reader.beginObject()
        assert("\$oid" == reader.nextName())
        val objectId: String = reader.nextString()
        reader.endObject()
        return ObjectId(objectId)
    }
}
