import { INDEX_CRUMB } from '@/breadcrumbs/general';
import sjrIf from '@/lib/sjrIf';
import sjrRedirectRules from '@/router/sjrRedirectRules';

import { HEALTH_RECORDS_NAME } from '@/router/names';

const UPLIFT_GP_MEDICAL_RECORD_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  nativeDisabled: true,
};

export const HEALTH_RECORDS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB],
  nativeDisabled: true,
  i18nKey: 'yourHealth',
  name: HEALTH_RECORDS_NAME,
};

const GP_MEDICAL_RECORD_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
  i18nKey: 'myRecord',
  name: HEALTH_RECORDS_NAME,
  nativeDisabled({ $store }) {
    const rule = sjrRedirectRules.silverIntegrationsHealthRecordHubCarePlansEnabledRedirect;
    return sjrIf({
      $store,
      journey: rule.journey,
      disabled: true,
    });
  },
};

const GP_MEDICAL_RECORD_GP_AT_HAND_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const ALLERGIES_AND_REACTIONS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const ACUTE_MEDICINES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const CURRENT_MEDICINES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const DISCONTINUED_MEDICINES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const CONSULTATIONS_AND_EVENTS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const TEST_RESULTS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const TEST_RESULTS_DETAIL_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const TEST_RESULTS_ID_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const IMMUNISATIONS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const DIAGNOSIS_V2_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const EXAMINATIONS_V2_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const PROCEDURES_V2_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const MEDICINES_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const DOCUMENTS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const DOCUMENT_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const DOCUMENT_DETAIL_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};

const HEALTH_CONDITIONS_CRUMB = {
  defaultCrumb: [INDEX_CRUMB, HEALTH_RECORDS_CRUMB],
};


export default {
  HEALTH_RECORDS_CRUMB,
  GP_MEDICAL_RECORD_CRUMB,
  ALLERGIES_AND_REACTIONS_CRUMB,
  ACUTE_MEDICINES_CRUMB,
  CURRENT_MEDICINES_CRUMB,
  DISCONTINUED_MEDICINES_CRUMB,
  CONSULTATIONS_AND_EVENTS_CRUMB,
  TEST_RESULTS_CRUMB,
  TEST_RESULTS_DETAIL_CRUMB,
  TEST_RESULTS_ID_CRUMB,
  IMMUNISATIONS_CRUMB,
  DIAGNOSIS_V2_CRUMB,
  EXAMINATIONS_V2_CRUMB,
  PROCEDURES_V2_CRUMB,
  MEDICINES_CRUMB,
  DOCUMENTS_CRUMB,
  DOCUMENT_CRUMB,
  DOCUMENT_DETAIL_CRUMB,
  HEALTH_CONDITIONS_CRUMB,
  UPLIFT_GP_MEDICAL_RECORD_CRUMB,
  GP_MEDICAL_RECORD_GP_AT_HAND_CRUMB,
};
