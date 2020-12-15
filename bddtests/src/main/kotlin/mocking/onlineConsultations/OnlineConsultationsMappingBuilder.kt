package mocking.onlineConsultations

import mocking.MappingBuilder
import mocking.onlineConsultations.configurations.IQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.CarePlanConfiguration
import mocking.onlineConsultations.configurations.evaluate.ConditionsConfigurations
import mocking.onlineConsultations.configurations.evaluate.SelfOrChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.TermsAndConditionsQuestionConfigurationI
import mocking.onlineConsultations.configurations.evaluate.GenderQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.HowWeCanHelpQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.QuantityQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.EmergencyCarePlanConfiguration
import mocking.onlineConsultations.configurations.evaluate.AlcoholQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.UrgencyQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.DOBQuestionConfiguration
import mocking.onlineConsultations.configurations.evaluate.EmergencyConfiguration

import mocking.onlineConsultations.configurations.evaluate.childJourneyUnder18.YouMustBeOver18Message
import mocking.onlineConsultations.configurations.evaluate.childJourney.GenderQuestionChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.childJourney.CarePlanChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.childJourney.DOBQuestionChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.childJourney.HowWeCanHelpQuestionChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.childJourney.QuantityQuestionChildConfiguration
import mocking.onlineConsultations.configurations.evaluate.childJourney.ChildConditions

import mocking.onlineConsultations.constants.OnlineConsultationConstants

open class OnlineConsultationsMappingBuilder(method: String="POST", relativePath: String= "")
    : MappingBuilder(method, relativePath) {

    fun isValidRequest(isValid: Boolean = true) = IsValidServiceDefinitionBuilder(isValid)

    fun termsAndConditionsRequest() = setUpRequest(
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = TermsAndConditionsQuestionConfigurationI())

    fun selfOrChildRequest(hasGpSession: Boolean = true) = setUpRequest(
            hasGpSession,
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration = SelfOrChildConfiguration())

    fun conditionsRequest(hasGpSession: Boolean = true,
                          isChildCondition: Boolean = false) = setUpRequest(
            hasGpSession,
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST,
            configuration =
                if (isChildCondition) ChildConditions()
                else ConditionsConfigurations())

    fun genderRequest(hasGpSession: Boolean = true,
                      isChild: Boolean = false) = setUpRequest(
            isChild,
            hasGpSession,
            configuration =  if (isChild) GenderQuestionChildConfiguration()
            else GenderQuestionConfiguration())

    fun urgencyQuestion(hasGpSession: Boolean = true) = setUpRequest(
            hasGpSession,
            configuration = UrgencyQuestionConfiguration())

    fun emergencyQuestion() = setUpRequest(configuration = EmergencyConfiguration())

    fun emergencyEndOfConsultationConfiguration() = setUpRequest(configuration = EmergencyCarePlanConfiguration())

    fun howWeCanHelpQuestion(isChild: Boolean = false) = setUpRequest(configuration =
        if (isChild) HowWeCanHelpQuestionChildConfiguration()
        else HowWeCanHelpQuestionConfiguration())

    fun dobQuestion(isChild: Boolean = false) = setUpRequest(
            isChild,
            configuration =
            if (isChild) DOBQuestionChildConfiguration()
            else DOBQuestionConfiguration())

    fun alcoholQuestion() = setUpRequest(configuration = AlcoholQuestionConfiguration())

    fun quantityQuestion(isChild: Boolean = false) = setUpRequest(configuration =
                            if (isChild) QuantityQuestionChildConfiguration()
                            else QuantityQuestionConfiguration())

    fun youMustBeOver18() = setUpRequest(
            configuration = YouMustBeOver18Message(),
            serviceDefinitionId = OnlineConsultationConstants.CONDITION_LIST)

    fun carePlan(isChild: Boolean = false) = setUpRequest(configuration =
        if (isChild) CarePlanChildConfiguration()
        else CarePlanConfiguration())

    private fun setUpRequest(hasGpSession: Boolean = true,
                             isChild: Boolean = false,
                             serviceDefinitionId: String = OnlineConsultationConstants.BREATHING_PROBLEMS_CONDITION_ID,
                             configuration: IQuestionConfiguration): EvaluateServiceDefinitionBuilder {
        return EvaluateServiceDefinitionBuilder(hasGpSession, serviceDefinitionId, configuration, isChild)
    }
}
