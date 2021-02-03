package mocking.onlineConsultations

import mocking.MappingBuilder
import mocking.onlineConsultations.configurations.evaluate.ConditionsConfigurations
import mocking.onlineConsultations.configurations.evaluate.DOBQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.EmergencyConfiguration
import mocking.onlineConsultations.configurations.evaluate.EmergencyCarePlanConfiguration
import mocking.onlineConsultations.configurations.evaluate.HowWeCanHelpQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.GenderQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.SelfOrChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.AlcoholQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.CarePlanConfiguration
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.QuantityQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.TermsAndConditionsQuestionConfigurationI
import mocking.onlineConsultations.configurations.evaluate.UrgencyQuestionConfiguration
import mocking.onlineConsultations.constants.OnlineConsultationConstants

open class OnlineConsultationsMappingBuilder(method: String="POST", relativePath: String= "")
    : MappingBuilder(method, relativePath) {

    fun isValidRequest(isValid: Boolean = true) = IsValidServiceDefinitionBuilder(isValid)

    fun termsAndConditionsRequest() = setUpRequest(
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = TermsAndConditionsQuestionConfigurationI())

    fun conditionsRequest(hasGpSession: Boolean = true) = setUpRequest(
            hasGpSession,
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = ConditionsConfigurations())

    fun genderRequest(hasGpSession: Boolean = true) = setUpRequest(
            hasGpSession,
            configuration = GenderQuestionConfiguration())

    fun selfOrChildRequest() = setUpRequest(configuration = SelfOrChildConfiguration())

    fun urgencyQuestion() = setUpRequest( configuration = UrgencyQuestionConfiguration())

    fun emergencyQuestion() = setUpRequest(configuration = EmergencyConfiguration())

    fun emergencyEndOfConsultationConfiguration() = setUpRequest(configuration = EmergencyCarePlanConfiguration())

    fun howWeCanHelpQuestion() = setUpRequest(configuration = HowWeCanHelpQuestionConfiguration())

    fun dobQuestion() = setUpRequest( configuration = DOBQuestionConfiguration())

    fun alcoholQuestion() = setUpRequest(configuration = AlcoholQuestionConfiguration())

    fun quantityQuestion() = setUpRequest(configuration = QuantityQuestionConfiguration())

    fun carePlan() = setUpRequest(configuration = CarePlanConfiguration())

    private fun setUpRequest(hasGpSession: Boolean = true,
                             serviceDefinitionId: String = OnlineConsultationConstants.BREATHING_PROBLEMS_CONDITION_ID,
                             configuration: IQuestionConfiguration): EvaluateServiceDefinitionBuilder {
        return EvaluateServiceDefinitionBuilder(hasGpSession, serviceDefinitionId, configuration)
    }
}
