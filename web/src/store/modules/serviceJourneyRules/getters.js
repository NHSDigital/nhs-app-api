import get from 'lodash/fp/get';
import { CDSS_ADMIN, CDSS_ADVICE, GP_AT_HAND, IM1_PROVIDER, INFORMATICA, NOMINATED_PHARMACY } from './mutation-types';


export default {
  [`${CDSS_ADMIN}Enabled`](state) {
    return !(state.rules.cdssAdmin.provider === 'none');
  },
  [`${CDSS_ADVICE}Enabled`](state) {
    return !(state.rules.cdssAdvice.provider === 'none');
  },
  [`${CDSS_ADMIN}Disabled`](state) {
    return state.rules.cdssAdmin.provider === 'none';
  },
  [`${CDSS_ADVICE}Disabled`](state) {
    return state.rules.cdssAdvice.provider === 'none';
  },
  [`${CDSS_ADMIN}Provider`](state) {
    return state.rules.cdssAdmin.provider;
  },
  [`${CDSS_ADVICE}Provider`](state) {
    return state.rules.cdssAdvice.provider;
  },
  [`${CDSS_ADMIN}Id`](state) {
    return state.rules.cdssAdmin.serviceDefinition;
  },
  [`${CDSS_ADVICE}Id`](state) {
    return state.rules.cdssAdvice.serviceDefinition;
  },
  [`${IM1_PROVIDER}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [`${INFORMATICA}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
  [`${IM1_PROVIDER}MyRecordEnabled`](state) {
    return get('rules.medicalRecord.provider')(state) === IM1_PROVIDER;
  },
  [`${GP_AT_HAND}MyRecordEnabled`](state) {
    return get('rules.medicalRecord.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
  [`${GP_AT_HAND}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [`${NOMINATED_PHARMACY}Enabled`](state) {
    return get('rules.nominatedPharmacy')(state);
  },
  [`${GP_AT_HAND}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
};
