package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "ViewPatientOverviewReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ViewPatientOverviewReply(
        @XmlElementWrapper(name="DrugSensitivities")
        @field:XmlElement(name="Item")
        var drugSensitivities: MutableList<ViewPatientOverviewItem> = arrayListOf(),
        @XmlElementWrapper(name="Drugs")
        @field:XmlElement(name="Item")
        var drugs: MutableList<ViewPatientOverviewItem> = arrayListOf(),
        @XmlElementWrapper(name="PastRepeats")
        @field:XmlElement(name="Item")
        var pastRepeats: MutableList<ViewPatientOverviewItem> = arrayListOf(),
        @XmlElementWrapper(name="CurrentRepeats")
        @field:XmlElement(name="Item")
        var currentRepeats: MutableList<ViewPatientOverviewItem> = arrayListOf(),
        @XmlElementWrapper(name="Allergies")
        @field:XmlElement(name="Item")
        var allergies: MutableList<ViewPatientOverviewItem> = arrayListOf()
)