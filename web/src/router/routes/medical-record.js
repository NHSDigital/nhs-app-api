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
    headerKey: 'navigation.pages.headers.myRecord',
    titleKey: 'navigation.pages.titles.myRecord',
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
    headerKey: 'navigation.pages.headers.healthRecords',
    titleKey: 'navigation.pages.titles.healthRecords',
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
    headerKey: 'navigation.pages.headers.myRecord',
    titleKey: 'navigation.pages.titles.myRecord',
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
    headerKey: 'navigation.pages.headers.allergiesAndReactions',
    titleKey: 'navigation.pages.titles.allergiesAndReactions',
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
    headerKey: 'navigation.pages.headers.medicines',
    titleKey: 'navigation.pages.titles.medicines',
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
    headerKey: 'navigation.pages.headers.acuteMedicines',
    titleKey: 'navigation.pages.titles.acuteMedicines',
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
    headerKey: 'navigation.pages.headers.currentMedicines',
    titleKey: 'navigation.pages.titles.currentMedicines',
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
    headerKey: 'navigation.pages.headers.discontinuedMedicines',
    titleKey: 'navigation.pages.titles.discontinuedMedicines',
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
    headerKey: 'navigation.pages.headers.testResults',
    titleKey: 'navigation.pages.titles.testResults',
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
    headerKey: 'navigation.pages.headers.testResults',
    titleKey: 'navigation.pages.titles.testResults',
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
    headerKey: 'navigation.pages.headers.testResult',
    titleKey: 'navigation.pages.titles.testResult',
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
    headerKey: 'navigation.pages.headers.consultationsAndEvents',
    titleKey: 'navigation.pages.titles.consultationsAndEvents',
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
    headerKey: 'navigation.pages.headers.consultationsAndEvents',
    titleKey: 'navigation.pages.titles.consultationsAndEvents',
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
    headerKey: 'navigation.pages.headers.immunisations',
    titleKey: 'navigation.pages.titles.immunisations',
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
    headerKey: 'navigation.pages.headers.healthConditions',
    titleKey: 'navigation.pages.titles.healthConditions',
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
    headerKey: 'navigation.pages.headers.myRecordDocuments',
    titleKey: 'navigation.pages.titles.myRecordDocuments',
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
    headerKey: 'navigation.pages.headers.myRecordDocuments',
    titleKey: 'navigation.pages.titles.myRecordDocuments',
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
    titleKey: 'navigation.pages.titles.myRecordDocuments',
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
    headerKey: 'navigation.pages.headers.diagnosisV2',
    titleKey: 'navigation.pages.titles.diagnosisV2',
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
    headerKey: 'navigation.pages.headers.examinationsV2',
    titleKey: 'navigation.pages.titles.examinationsV2',
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
    headerKey: 'navigation.pages.headers.proceduresV2',
    titleKey: 'navigation.pages.titles.proceduresV2',
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
    headerKey: 'navigation.pages.headers.medicalHistory',
    titleKey: 'navigation.pages.titles.medicalHistory',
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
    headerKey: 'navigation.pages.headers.encounters',
    titleKey: 'navigation.pages.titles.encounters',
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
    headerKey: 'navigation.pages.headers.recalls',
    titleKey: 'navigation.pages.titles.recalls',
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
    headerKey: 'navigation.pages.headers.referrals',
    titleKey: 'navigation.pages.titles.referrals',
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
    headerKey: 'navigation.pages.headers.serviceUnavailable',
    titleKey: 'navigation.pages.titles.serviceUnavailable',
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
