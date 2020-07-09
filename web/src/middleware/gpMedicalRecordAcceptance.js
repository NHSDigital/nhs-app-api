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
  const detailsPages = [ACUTE_MEDICINES.name,
    ALLERGIESANDREACTIONS.name,
    CONSULTATIONS.name,
    CURRENT_MEDICINES.name,
    DIAGNOSIS_V2.name,
    DISCONTINUED_MEDICINES.name,
    DOCUMENT.name,
    DOCUMENTS.name,
    DOCUMENT_DETAIL.name,
    ENCOUNTERS.name,
    EVENTS.name,
    EXAMINATIONS_V2.name,
    HEALTH_CONDITIONS.name,
    IMMUNISATIONS.name,
    MEDICAL_HISTORY.name,
    MEDICINES.name,
    PROCEDURES_V2.name,
    RECALLS.name,
    REFERRALS.name,
    TESTRESULTID.name,
    TESTRESULTS.name,
    TESTRESULTSDETAIL.name];
  const matchesRoute = detailsPages.includes(route.name);
  const acceptedTerms = store.state.myRecord.hasAcceptedTerms;
  if (matchesRoute && !acceptedTerms) {
    redirect(GP_MEDICAL_RECORD.path);
  }
};
