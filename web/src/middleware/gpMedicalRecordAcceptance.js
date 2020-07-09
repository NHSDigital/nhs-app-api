import {
  ACUTE_MEDICINES,
  ALLERGIESANDREACTIONS,
  CONSULTATIONS,
  CURRENT_MEDICINES,
  DIAGNOSIS_V2,
  DISCONTINUED_MEDICINES,
  DOCUMENT,
  DOCUMENTS,
  DOCUMENT_DETAIL,
  ENCOUNTERS,
  EVENTS,
  EXAMINATIONS_V2,
  GP_MEDICAL_RECORD,
  HEALTH_CONDITIONS,
  IMMUNISATIONS,
  MEDICAL_HISTORY,
  MEDICINES,
  PROCEDURES_V2,
  RECALLS,
  REFERRALS,
  TESTRESULTID,
  TESTRESULTS,
  TESTRESULTSDETAIL,
} from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const matchesRoute = (route.name === ACUTE_MEDICINES.name ||
    route.name === ALLERGIESANDREACTIONS.name ||
    route.name === CONSULTATIONS.name ||
    route.name === CURRENT_MEDICINES.name ||
    route.name === DIAGNOSIS_V2.name ||
    route.name === DISCONTINUED_MEDICINES.name ||
    route.name === DOCUMENT.name ||
    route.name === DOCUMENTS.name ||
    route.name === DOCUMENT_DETAIL.name ||
    route.name === ENCOUNTERS.name ||
    route.name === EVENTS.name ||
    route.name === EXAMINATIONS_V2.name ||
    route.name === HEALTH_CONDITIONS.name ||
    route.name === IMMUNISATIONS.name ||
    route.name === MEDICAL_HISTORY.name ||
    route.name === MEDICINES.name ||
    route.name === PROCEDURES_V2.name ||
    route.name === RECALLS.name ||
    route.name === REFERRALS.name ||
    route.name === TESTRESULTID.name ||
    route.name === TESTRESULTS.name ||
    route.name === TESTRESULTSDETAIL.name);
  const acceptedTerms = store.state.myRecord.hasAcceptedTerms;
  if (matchesRoute && !acceptedTerms) {
    redirect(GP_MEDICAL_RECORD.path);
  }
};
