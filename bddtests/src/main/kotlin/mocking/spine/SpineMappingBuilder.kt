package mocking.spine

import mocking.MappingBuilder
import mocking.spine.ePS.EPSMappingBuilder
import mocking.spine.ePS.EPSMappingBuilder111ItemDetail
import mocking.spine.ePS.EPSMappingBuilder111ItemSummary

open class SpineMappingBuilder(method: String, relativePath: String = "", soapAction: String = "")
    : MappingBuilder(method, "/spine$relativePath") {

    companion object {
        val configuration = SpineConfiguration("", "", "", "")
    }

    private var soapActionKey = "SoapAction"

    init {
        requestBuilder
        .andHeader(soapActionKey, soapAction)

    }

    var itemSummary = EPSMappingBuilder111ItemSummary(EPSMappingBuilder.configuration)
    var itemDescription = EPSMappingBuilder111ItemDetail(EPSMappingBuilder.configuration)
}