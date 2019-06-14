import {
  CLEAR,
  SET_SERVICE_DEFINITION_ID,
  SET_QUESTION_FROM_GUIDANCE_RESPONSE,
  SET_PREVIOUS_ROUTE,
} from './mutation-types';
import InitialAdminHelpGuidanceResponse from '@/lib/online-consultations/data/guidance-responses';

export default {
  clear({ commit }) {
    commit(CLEAR);
  },
  getAdminHelpServiceDefinitionId({ commit }) {
    // todo: 5796 - get from SJR
    commit(SET_SERVICE_DEFINITION_ID, 'TEST_GEC_ADM');
  },
  evaluateServiceDefinition({ commit, state }, parameters) {
    if (state.serviceDefinitionId === undefined || parameters === undefined) return;

    // todo: 5796 - post parameters to cds api for id
    const guidanceResponse = InitialAdminHelpGuidanceResponse;

    commit(SET_QUESTION_FROM_GUIDANCE_RESPONSE, guidanceResponse);
  },
  setPreviousRoute({ commit }, previousRoute) {
    commit(SET_PREVIOUS_ROUTE, previousRoute);
  },
};
