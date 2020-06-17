import HealthRecordsPage from '@/pages/health-records';
import GpMedicalRecordPage from '@/pages/health-records/gp-medical-record';
import AcuteMedicinesPage from '@/pages/health-records/gp-medical-record/medicines/acute-medicines';
import AllergiesAndReactionsPage from '@/pages/health-records/gp-medical-record/allergies-and-reactions';
import ConsultationsPage from '@/pages/health-records/gp-medical-record/consultations';
import CurrentMedicinesPage from '@/pages/health-records/gp-medical-record/medicines/current-medicines';
import DiagnosisPage from '@/pages/health-records/gp-medical-record/diagnosis';
import DiscontinuedMedicinesPage from '@/pages/health-records/gp-medical-record/medicines/discontinued-medicines';
import DocumentDetailPage from '@/pages/health-records/gp-medical-record/documents/detail/_id';
import DocumentPage from '@/pages/health-records/gp-medical-record/documents/_id';
import DocumentsPage from '@/pages/health-records/gp-medical-record/documents';
import EncountersPage from '@/pages/health-records/gp-medical-record/encounters';
import EventsPage from '@/pages/health-records/gp-medical-record/events';
import ExaminationsPage from '@/pages/health-records/gp-medical-record/examinations';
import HealthConditionsPage from '@/pages/health-records/gp-medical-record/health-conditions';
import ImmunisationsPage from '@/pages/health-records/gp-medical-record/immunisations';
import MedicalHistoryPage from '@/pages/health-records/gp-medical-record/medical-history';
import MedicinesPage from '@/pages/health-records/gp-medical-record/medicines';
import ProceduresPage from '@/pages/health-records/gp-medical-record/procedures';
import RecallsPage from '@/pages/health-records/gp-medical-record/recalls';
import ReferralsPage from '@/pages/health-records/gp-medical-record/referrals';
import TestResultIdPage from '@/pages/health-records/gp-medical-record/testresultdetail/_testResultId';
import TestResultsDetailPage from '@/pages/health-records/gp-medical-record/test-results-detail';
import TestResultsPage from '@/pages/health-records/gp-medical-record/test-results';
import GpAtHandPage from '@/pages/health-records/gp-at-hand';
import UpliftGpMedicalRecordPage from '@/pages/uplift/gp-medical-record';

import breadcrumbs from '@/breadcrumbs/medicalRecord';

import {
  HEALTH_RECORDS_PATH,
  GP_MEDICAL_RECORD_PATH,
  ALLERGIESANDREACTIONS_PATH,
  MEDICINES_PATH,
  ACUTE_MEDICINES_PATH,
  CURRENT_MEDICINES_PATH,
  DISCONTINUED_MEDICINES_PATH,
  TESTRESULTS_PATH,
  TESTRESULTSDETAIL_PATH,
  TESTRESULTID_PATH,
  CONSULTATIONS_PATH,
  EVENTS_PATH,
  IMMUNISATIONS_PATH,
  HEALTH_CONDITIONS_PATH,
  DOCUMENTS_PATH,
  DOCUMENT_PATH,
  DOCUMENT_DETAIL_PATH,
  DIAGNOSIS_V2_PATH,
  EXAMINATIONS_V2_PATH,
  PROCEDURES_V2_PATH,
  ENCOUNTERS_PATH,
  MEDICAL_HISTORY_PATH,
  RECALLS_PATH,
  REFERRALS_PATH,
  GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  UPLIFT_GP_MEDICAL_RECORD_PATH,
  MY_RECORD_PATH,
} from '@/router/paths';
import {
  HEALTH_RECORDS_NAME,
  GP_MEDICAL_RECORD_NAME,
  ALLERGIESANDREACTIONS_NAME,
  MEDICINES_NAME,
  ACUTE_MEDICINES_NAME,
  CURRENT_MEDICINES_NAME,
  DISCONTINUED_MEDICINES_NAME,
  TESTRESULTS_NAME,
  TESTRESULTSDETAIL_NAME,
  TESTRESULTID_NAME,
  CONSULTATIONS_NAME,
  EVENTS_NAME,
  IMMUNISATIONS_NAME,
  HEALTH_CONDITIONS_NAME,
  DOCUMENTS_NAME,
  DOCUMENT_NAME,
  DOCUMENT_DETAIL_NAME,
  DIAGNOSIS_V2_NAME,
  EXAMINATIONS_V2_NAME,
  PROCEDURES_V2_NAME,
  ENCOUNTERS_NAME,
  MEDICAL_HISTORY_NAME,
  RECALLS_NAME,
  REFERRALS_NAME,
  GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  UPLIFT_GP_MEDICAL_RECORD_NAME,
  MY_RECORD_NAME,
} from '@/router/names';

import { YOUR_RECORD_MENU_ITEM } from '@/middleware/nativeNavigation';
import gpMedicalRecordAcceptance from '@/middleware/gpMedicalRecordAcceptance';
import urlResolution from '@/middleware/urlResolution';

import proofLevel from '@/lib/proofLevel';
import { gpMedicalRecordHelpUrl, healthRecordsHelpUrl } from '@/router/externalLinks';
import sjrRedirectRules from '@/router/sjrRedirectRules';

export const UPLIFT_GP_MEDICAL_RECORD = {
  path: UPLIFT_GP_MEDICAL_RECORD_PATH,
  name: UPLIFT_GP_MEDICAL_RECORD_NAME,
  component: UpliftGpMedicalRecordPage,
  meta: {
    headerKey: 'pageHeaders.myRecord',
    titleKey: 'pageTitles.myRecord',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.UPLIFT_GP_MEDICAL_RECORD_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [urlResolution],
  },
};

export const HEALTH_RECORDS = {
  path: HEALTH_RECORDS_PATH,
  name: HEALTH_RECORDS_NAME,
  component: HealthRecordsPage,
  meta: {
    headerKey: 'pageHeaders.healthRecords',
    titleKey: 'pageTitles.healthRecords',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.HEALTH_RECORDS_CRUMB,
    helpUrl: healthRecordsHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
  },
};

export const GP_MEDICAL_RECORD = {
  path: GP_MEDICAL_RECORD_PATH,
  name: GP_MEDICAL_RECORD_NAME,
  component: GpMedicalRecordPage,
  meta: {
    headerKey: 'pageHeaders.myRecord',
    titleKey: 'pageTitles.myRecord',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.GP_MEDICAL_RECORD_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandMyRecordRedirect,
      sjrRedirectRules.gpAtHandMedicalRecordRedirectV2,
    ],
  },
};

export const ALLERGIESANDREACTIONS = {
  path: ALLERGIESANDREACTIONS_PATH,
  name: ALLERGIESANDREACTIONS_NAME,
  component: AllergiesAndReactionsPage,
  meta: {
    headerKey: 'pageHeaders.allergiesAndReactions',
    titleKey: 'pageTitles.allergiesAndReactions',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.ALLERGIES_AND_REACTIONS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const MEDICINES = {
  path: MEDICINES_PATH,
  name: MEDICINES_NAME,
  component: MedicinesPage,
  meta: {
    headerKey: 'pageHeaders.medicines',
    titleKey: 'pageTitles.medicines',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.MEDICINES_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const ACUTE_MEDICINES = {
  path: ACUTE_MEDICINES_PATH,
  name: ACUTE_MEDICINES_NAME,
  component: AcuteMedicinesPage,
  meta: {
    headerKey: 'pageHeaders.acuteMedicines',
    titleKey: 'pageTitles.acuteMedicines',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.ACUTE_MEDICINES_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const CURRENT_MEDICINES = {
  path: CURRENT_MEDICINES_PATH,
  name: CURRENT_MEDICINES_NAME,
  component: CurrentMedicinesPage,
  meta: {
    headerKey: 'pageHeaders.currentMedicines',
    titleKey: 'pageTitles.currentMedicines',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.CURRENT_MEDICINES_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const DISCONTINUED_MEDICINES = {
  path: DISCONTINUED_MEDICINES_PATH,
  name: DISCONTINUED_MEDICINES_NAME,
  component: DiscontinuedMedicinesPage,
  meta: {
    headerKey: 'pageHeaders.discontinuedMedicines',
    titleKey: 'pageTitles.discontinuedMedicines',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DISCONTINUED_MEDICINES_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const TESTRESULTS = {
  path: TESTRESULTS_PATH,
  name: TESTRESULTS_NAME,
  component: TestResultsPage,
  meta: {
    headerKey: 'pageHeaders.testResults',
    titleKey: 'pageTitles.testResults',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const TESTRESULTSDETAIL = {
  path: TESTRESULTSDETAIL_PATH,
  name: TESTRESULTSDETAIL_NAME,
  component: TestResultsDetailPage,
  meta: {
    headerKey: 'pageHeaders.testResults',
    titleKey: 'pageTitles.testResults',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_DETAIL_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const TESTRESULTID = {
  path: TESTRESULTID_PATH,
  name: TESTRESULTID_NAME,
  component: TestResultIdPage,
  meta: {
    headerKey: 'pageHeaders.testResult',
    titleKey: 'pageTitles.testResult',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_ID_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const CONSULTATIONS = {
  path: CONSULTATIONS_PATH,
  name: CONSULTATIONS_NAME,
  component: ConsultationsPage,
  meta: {
    headerKey: 'pageHeaders.consultationsAndEvents',
    titleKey: 'pageTitles.consultationsAndEvents',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.CONSULTATIONS_AND_EVENTS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const EVENTS = {
  path: EVENTS_PATH,
  name: EVENTS_NAME,
  component: EventsPage,
  meta: {
    headerKey: 'pageHeaders.consultationsAndEvents',
    titleKey: 'pageTitles.consultationsAndEvents',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.CONSULTATIONS_AND_EVENTS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const IMMUNISATIONS = {
  path: IMMUNISATIONS_PATH,
  name: IMMUNISATIONS_NAME,
  component: ImmunisationsPage,
  meta: {
    headerKey: 'pageHeaders.immunisations',
    titleKey: 'pageTitles.immunisations',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.IMMUNISATIONS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const HEALTH_CONDITIONS = {
  path: HEALTH_CONDITIONS_PATH,
  name: HEALTH_CONDITIONS_NAME,
  component: HealthConditionsPage,
  meta: {
    headerKey: 'pageHeaders.healthConditions',
    titleKey: 'pageTitles.healthConditions',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.HEALTH_CONDITIONS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const DOCUMENTS = {
  path: DOCUMENTS_PATH,
  name: DOCUMENTS_NAME,
  component: DocumentsPage,
  meta: {
    headerKey: 'pageHeaders.myRecordDocuments',
    titleKey: 'pageTitles.myRecordDocuments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DOCUMENTS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const DOCUMENT = {
  path: DOCUMENT_PATH,
  name: DOCUMENT_NAME,
  component: DocumentPage,
  meta: {
    headerKey: 'pageHeaders.myRecordDocuments',
    titleKey: 'pageTitles.myRecordDocuments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DOCUMENT_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const DOCUMENT_DETAIL = {
  path: DOCUMENT_DETAIL_PATH,
  name: DOCUMENT_DETAIL_NAME,
  component: DocumentDetailPage,
  meta: {
    titleKey: 'pageTitles.myRecordDocuments',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DOCUMENT_DETAIL_CRUMB,
    shouldShowContentHeader: false,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const DIAGNOSIS_V2 = {
  path: DIAGNOSIS_V2_PATH,
  name: DIAGNOSIS_V2_NAME,
  component: DiagnosisPage,
  meta: {
    headerKey: 'pageHeaders.diagnosisV2',
    titleKey: 'pageTitles.diagnosisV2',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DIAGNOSIS_V2_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const EXAMINATIONS_V2 = {
  path: EXAMINATIONS_V2_PATH,
  name: EXAMINATIONS_V2_NAME,
  component: ExaminationsPage,
  meta: {
    headerKey: 'pageHeaders.examinationsV2',
    titleKey: 'pageTitles.examinationsV2',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.EXAMINATIONS_V2_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const PROCEDURES_V2 = {
  path: PROCEDURES_V2_PATH,
  name: PROCEDURES_V2_NAME,
  component: ProceduresPage,
  meta: {
    headerKey: 'pageHeaders.proceduresV2',
    titleKey: 'pageTitles.proceduresV2',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.PROCEDURES_V2_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const MEDICAL_HISTORY = {
  path: MEDICAL_HISTORY_PATH,
  name: MEDICAL_HISTORY_NAME,
  component: MedicalHistoryPage,
  meta: {
    headerKey: 'pageHeaders.medicalHistory',
    titleKey: 'pageTitles.medicalHistory',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.MEDICAL_HISTORY_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const ENCOUNTERS = {
  path: ENCOUNTERS_PATH,
  name: ENCOUNTERS_NAME,
  component: EncountersPage,
  meta: {
    headerKey: 'pageHeaders.encounters',
    titleKey: 'pageTitles.encounters',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.ENCOUNTERS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const RECALLS = {
  path: RECALLS_PATH,
  name: RECALLS_NAME,
  component: RecallsPage,
  meta: {
    headerKey: 'pageHeaders.recalls',
    titleKey: 'pageTitles.recalls',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.RECALLS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const REFERRALS = {
  path: REFERRALS_PATH,
  name: REFERRALS_NAME,
  component: ReferralsPage,
  meta: {
    headerKey: 'pageHeaders.referrals',
    titleKey: 'pageTitles.referrals',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.REFERRALS_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
  },
};

export const GP_MEDICAL_RECORD_GP_AT_HAND = {
  path: GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  name: GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  component: GpAtHandPage,
  meta: {
    headerKey: 'pageHeaders.serviceUnavailable',
    titleKey: 'pageTitles.serviceUnavailable',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.GP_MEDICAL_RECORD_GP_AT_HAND_CRUMB,
    helpUrl: gpMedicalRecordHelpUrl,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.im1GpMedicalRecordRedirectV2],
  },
};

export const MY_RECORD = {
  path: MY_RECORD_PATH,
  name: MY_RECORD_NAME,
  redirect: HEALTH_RECORDS.path,
};

export default [
  MY_RECORD,
  HEALTH_RECORDS,
  GP_MEDICAL_RECORD,
  ALLERGIESANDREACTIONS,
  MEDICINES,
  ACUTE_MEDICINES,
  CURRENT_MEDICINES,
  DISCONTINUED_MEDICINES,
  TESTRESULTS,
  TESTRESULTSDETAIL,
  TESTRESULTID,
  CONSULTATIONS,
  EVENTS,
  IMMUNISATIONS,
  HEALTH_CONDITIONS,
  DOCUMENTS,
  DOCUMENT,
  DOCUMENT_DETAIL,
  DIAGNOSIS_V2,
  EXAMINATIONS_V2,
  PROCEDURES_V2,
  ENCOUNTERS,
  MEDICAL_HISTORY,
  RECALLS,
  REFERRALS,
  UPLIFT_GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
];
