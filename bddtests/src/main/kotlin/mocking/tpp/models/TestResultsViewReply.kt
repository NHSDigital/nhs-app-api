package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "TestResultsViewReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class TestResultsViewReply(
        @field:XmlElement(name="Item")
        var items: MutableList<TestResultsViewReplyItem> = arrayListOf()
)
