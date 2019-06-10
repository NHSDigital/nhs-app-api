import first from 'lodash/fp/first';
import {
  QUESTIONNAIRE,
  QUESTIONNAIRE_RESPONSE,
} from '@/lib/online-consultations/resource-types';
import {
  CHOICE,
} from '@/lib/online-consultations/question-types';

export default {
  getQuestionnaireId(guidanceResponse) {
    if (guidanceResponse === undefined ||
        !Array.isArray(guidanceResponse.dataRequirement)) {
      return undefined;
    }

    const questionnaireResponseDataRequirements = guidanceResponse.dataRequirement
      .filter(d => d.type === QUESTIONNAIRE_RESPONSE);

    if (!Array.isArray(questionnaireResponseDataRequirements) ||
        questionnaireResponseDataRequirements[0] === undefined) {
      return undefined;
    }

    return first(questionnaireResponseDataRequirements).id;
  },
  getQuestionnaireById(guidanceResponse, dataRequirementId) {
    if (guidanceResponse === undefined ||
        !Array.isArray(guidanceResponse.contained) ||
        dataRequirementId === undefined) {
      return undefined;
    }

    return guidanceResponse.contained
      .filter(c => c.resourceType === QUESTIONNAIRE
                   && c.id === dataRequirementId
                   && c.status === 'active')[0];
  },
  getQuestionFromQuestionnaire(questionnaire) {
    if (questionnaire === undefined ||
        !Array.isArray(questionnaire.item)) {
      return undefined;
    }

    const item = questionnaire.item[0];

    if (item === undefined) {
      return undefined;
    }

    return this.getQuestionFromQuestionnaireItem(item);
  },
  getQuestionFromQuestionnaireItem(item) {
    const question = {
      type: item.type,
      text: item.text,
      id: item.linkId,
      required: item.required,
      tag: undefined,
      isLegend: false,
    };

    if (question.type === CHOICE) {
      question.tag = 'p';
      question.isLegend = true;
      question.options = item.option.map(o => ({
        code: o.valueCoding.code,
        label: o.valueCoding.display,
      }));
      question.name = `${item.linkId}-${CHOICE}`;
    }

    return question;
  },
};
