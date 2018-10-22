package mocking.emis.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Language")
@XmlAccessorType(XmlAccessType.FIELD)
class Language {

    @field:XmlElement(name = "LanguageCode")
    var languageCode: String? = null

    @field:XmlElement(name = "Description")
    var description: String? = null
}