package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "TestResultsViewReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class TestResultsViewReply(
        @field:XmlElement(name="Item")
        var items: MutableList<TestResultsViewReplyItem> = arrayListOf()
)