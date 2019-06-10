import {
  CLEAR,
  SET_QUESTION_FROM_GUIDANCE_RESPONSE,
  SET_SERVICE_DEFINITION_ID,
} from './mutation-types';
import GuidanceResponseMapper from '@/lib/online-consultations/guidance-response-mapper';

export default {
  [CLEAR](state) {
    state.serviceDefinitionId = undefined;
    state.question = undefined;
  },
  [SET_SERVICE_DEFINITION_ID](state, id) {
    state.serviceDefinitionId = id;
  },
  [SET_QUESTION_FROM_GUIDANCE_RESPONSE](state, guidanceResponse) {
    state.question = undefined;
    state.error = false;

    const dataRequirementId = GuidanceResponseMapper.getQuestionnaireId(guidanceResponse);
    const questionnaire = GuidanceResponseMapper.getQuestionnaireById(
      guidanceResponse,
      dataRequirementId,
    );
    const question = GuidanceResponseMapper.getQuestionFromQuestionnaire(questionnaire);

    if (question === undefined) {
      state.error = true;
    }
    state.question = question;
  },
};
