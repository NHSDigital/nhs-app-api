package mocking.emis


import mocking.MappingBuilder
import mocking.onlineConsultations.EvaluateServiceDefinitionBuilder
import mocking.onlineConsultations.configurations.ConditionsConfigurations
import mocking.onlineConsultations.configurations.DOBQuestionConfiguration
import mocking.onlineConsultations.configurations.EmergencyConfiguration
import mocking.onlineConsultations.configurations.EmergencyCarePlanConfiguration
import mocking.onlineConsultations.configurations.HowWeCanHelpQuestionConfiguration
import mocking.onlineConsultations.configurations.PainOriginQuestionConfiguration
import mocking.onlineConsultations.configurations.GenderQuestionConfiguration
import mocking.onlineConsultations.configurations.SelfOrChildConfiguration
import mocking.onlineConsultations.configurations.AlcoholQuestionConfiguration
import mocking.onlineConsultations.configurations.CarePlanConfiguration
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import mocking.onlineConsultations.configurations.QuantityQuestionConfiguration
import mocking.onlineConsultations.configurations.TermsAndConditionsQuestionConfigurationI
import mocking.onlineConsultations.configurations.UrgencyQuestionConfiguration
import mocking.onlineConsultations.constants.OnlineConsultationConstants

open class OnlineConsultationsMappingBuilder(method: String = "POST", relativePath: String = "")
    : MappingBuilder(method, relativePath) {

    fun termsAndConditionsRequest() = setUpRequest(
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = TermsAndConditionsQuestionConfigurationI())

    fun conditionsRequest() = setUpRequest(
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = ConditionsConfigurations())

    fun genderRequest() = setUpRequest(configuration = GenderQuestionConfiguration())

    fun selfOrChildRequest() = setUpRequest(configuration = SelfOrChildConfiguration())

    fun urgencyQuestion() = setUpRequest( configuration = UrgencyQuestionConfiguration())

    fun emergencyQuestion() = setUpRequest(configuration = EmergencyConfiguration())

    fun emergencyEndOfConsultationConfiguration() = setUpRequest(configuration = EmergencyCarePlanConfiguration())

    fun howWeCanHelpQuestion() = setUpRequest(configuration = HowWeCanHelpQuestionConfiguration())

    fun dobQuestion() = setUpRequest( configuration = DOBQuestionConfiguration())

    fun alcoholQuestion() = setUpRequest(configuration = AlcoholQuestionConfiguration())

    fun imageQuestion() = setUpRequest(configuration = PainOriginQuestionConfiguration())

    fun quantityQuestion() = setUpRequest( configuration = QuantityQuestionConfiguration())

    fun carePlan() = setUpRequest(configuration = CarePlanConfiguration())

    private fun setUpRequest(serviceDefinitionId: String = OnlineConsultationConstants.BREATHING_PROBLEMS_CONDITION_ID,
                             configuration: IQuestionConfiguration): EvaluateServiceDefinitionBuilder {
        return EvaluateServiceDefinitionBuilder(serviceDefinitionId, configuration)
    }
}
