package features

import io.cucumber.java.DefaultDataTableCellTransformer
import io.cucumber.java.DefaultDataTableEntryTransformer
import io.cucumber.java.DefaultParameterTransformer
import wiremock.com.fasterxml.jackson.databind.JavaType
import wiremock.com.fasterxml.jackson.databind.ObjectMapper
import java.lang.reflect.Type

class DataTableSteps {
    private val objectMapper: ObjectMapper = ObjectMapper()

    @DefaultParameterTransformer
    @DefaultDataTableEntryTransformer
    @DefaultDataTableCellTransformer
    fun defaultTransformer(fromValue: Any?, toValueType: Type?): Any {
        val javaType: JavaType = objectMapper.constructType(toValueType)
        return objectMapper.convertValue(fromValue, javaType)
    }
}
