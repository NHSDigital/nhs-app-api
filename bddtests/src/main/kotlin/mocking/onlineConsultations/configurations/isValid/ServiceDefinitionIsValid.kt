package mocking.onlineConsultations.configurations.isValid

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class ServiceDefinitionIsValid: IQuestionConfiguration {
    // Not required - request is matched based on JsonPath expressions
    override val request: String = ""
    override val response: String = """
        {
            "resourceType":"Parameters",
            "parameter": [{
                "name": "return",
                "valueBoolean":"{{isValid}}"
            }]
        }
    """.trimIndent()
}
