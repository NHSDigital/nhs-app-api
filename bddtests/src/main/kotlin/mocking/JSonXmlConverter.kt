package mocking

import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

object JSonXmlConverter {
    private val gson by lazy { GsonFactory.asIs }
    private val gsonPascal by lazy { GsonFactory.asPascal }

    inline fun <reified T : Any> toXML(xmlAttributedSrc: T, excludeRootXmlTag: Boolean = false): String {
        val jaxbContext = JAXBContext.newInstance(T::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)
        marshaller.setProperty(Marshaller.JAXB_FRAGMENT, excludeRootXmlTag)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(xmlAttributedSrc, stringWriter)
        }

        return stringWriter.toString()
    }

    fun <T : Any> toJson(source: T): String {
        return gson.toJson(source)
    }

    fun <T : Any> toJsonWithUpperCamelCase(source: T): String {
        return gsonPascal.toJson(source)
    }

    fun wrapAroundXmlTag(tagName: String, value: String):String {
        return "<$tagName>$value</$tagName>"
    }
}
