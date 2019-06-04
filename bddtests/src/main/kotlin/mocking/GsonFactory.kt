package mocking

import com.google.gson.FieldNamingPolicy
import com.google.gson.Gson
import com.google.gson.GsonBuilder

class GsonFactory {

    companion object {
        val asIs = Gson()

        val asIsSerializeNulls = GsonBuilder()
                .serializeNulls()
                .create()

        val asPascal = GsonBuilder()
            .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
            .create()

        val asPascalSerializeNulls = GsonBuilder()
                .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                .serializeNulls()
                .create()
    }
}
