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
import EventsPage from '@/pages/health-records/gp-medical-record/events';
import ExaminationsPage from '@/pages/health-records/gp-medical-record/examinations';
import HealthConditionsPage from '@/pages/health-records/gp-medical-record/health-conditions';
import ImmunisationsPage from '@/pages/health-records/gp-medical-record/immunisations';
import MedicinesPage from '@/pages/health-records/gp-medical-record/medicines';
import ProceduresPage from '@/pages/health-records/gp-medical-record/procedures';
import TestResultIdPage from '@/pages/health-records/gp-medical-record/testresultdetail/_testResultId';
import TestResultsDetailPage from '@/pages/health-records/gp-medical-record/test-results-detail';
import TestResultsPage from '@/pages/health-records/gp-medical-record/test-results';
import TestResultsV2Page from '@/pages/health-records/gp-medical-record/test-results-v2';
import TestResultsForYearPage from '@/pages/health-records/gp-medical-record/test-results-for-year';
import ChooseTestResultYearPage from '@/pages/health-records/gp-medical-record/choose-test-result-year';
import GpAtHandPage from '@/pages/health-records/gp-at-hand';
import UpliftGpMedicalRecordPage from '@/pages/uplift/gp-medical-record';

import breadcrumbs from '@/breadcrumbs/medicalRecord';

import {
  ACUTE_MEDICINES_PATH,
  ALLERGIESANDREACTIONS_PATH,
  CONSULTATIONS_PATH,
  CURRENT_MEDICINES_PATH,
  DIAGNOSIS_V2_PATH,
  DISCONTINUED_MEDICINES_PATH,
  DOCUMENT_DETAIL_PATH,
  DOCUMENT_PATH,
  DOCUMENTS_PATH,
  EVENTS_PATH,
  EXAMINATIONS_V2_PATH,
  GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  GP_MEDICAL_RECORD_PATH,
  HEALTH_CONDITIONS_PATH,
  HEALTH_RECORDS_PATH,
  IMMUNISATIONS_PATH,
  MEDICINES_PATH,
  PROCEDURES_V2_PATH,
  TESTRESULTID_PATH,
  TESTRESULTS_PATH,
  TESTRESULTS_V2_PATH,
  CHOOSE_TEST_RESULT_YEAR_PATH,
  TEST_RESULTS_FOR_YEAR_PATH,
  TESTRESULTSDETAIL_PATH,
  UPLIFT_GP_MEDICAL_RECORD_PATH,
} from '@/router/paths';
import {
  ACUTE_MEDICINES_NAME,
  ALLERGIESANDREACTIONS_NAME,
  CONSULTATIONS_NAME,
  CURRENT_MEDICINES_NAME,
  DIAGNOSIS_V2_NAME,
  DISCONTINUED_MEDICINES_NAME,
  DOCUMENT_DETAIL_NAME,
  DOCUMENT_NAME,
  DOCUMENTS_NAME,
  EVENTS_NAME,
  EXAMINATIONS_V2_NAME,
  GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  GP_MEDICAL_RECORD_NAME,
  HEALTH_CONDITIONS_NAME,
  HEALTH_RECORDS_NAME,
  IMMUNISATIONS_NAME,
  MEDICINES_NAME,
  PROCEDURES_V2_NAME,
  TESTRESULTID_NAME,
  TESTRESULTS_NAME,
  TESTRESULTS_V2_NAME,
  CHOOSE_TEST_RESULT_YEAR_NAME,
  TEST_RESULTS_FOR_YEAR_NAME,
  TESTRESULTSDETAIL_NAME,
  UPLIFT_GP_MEDICAL_RECORD_NAME,
  GP_HEALTH_RECORD_JOURNEY_NAME,
} from '@/router/names';

import { YOUR_RECORD_MENU_ITEM } from '@/middleware/nativeNavigation';
import gpMedicalRecordAcceptance from '@/middleware/gpMedicalRecordAcceptance';

import proofLevel from '@/lib/proofLevel';
import { GP_MEDICAL_RECORD_HELP_PATH, HEALTH_RECORDS_HELP_PATH } from '@/router/externalLinks';
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export const HEALTH_RECORDS = {
  path: HEALTH_RECORDS_PATH,
  name: HEALTH_RECORDS_NAME,
  component: HealthRecordsPage,
  meta: {
    headerKey: 'navigation.pages.headers.yourHealth',
    titleKey: 'navigation.pages.titles.yourHealth',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.HEALTH_RECORDS_CRUMB,
    helpPath: HEALTH_RECORDS_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [
      sjrRedirectRules.gpAtHandMyRecordRedirect,
      sjrRedirectRules.gpAtHandMedicalRecordRedirectV2,
    ],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export const TESTRESULTSV2 = {
  path: TESTRESULTS_V2_PATH,
  name: TESTRESULTS_V2_NAME,
  component: TestResultsV2Page,
  meta: {
    headerKey: 'navigation.pages.headers.testResults',
    titleKey: 'navigation.pages.titles.testResults',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_CRUMB,
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export const CHOOSE_TEST_RESULT_YEAR = {
  path: CHOOSE_TEST_RESULT_YEAR_PATH,
  name: CHOOSE_TEST_RESULT_YEAR_NAME,
  component: ChooseTestResultYearPage,
  meta: {
    titleKey: 'navigation.pages.titles.testResults',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_CRUMB,
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export const TEST_RESULTS_FOR_YEAR = {
  path: TEST_RESULTS_FOR_YEAR_PATH,
  name: TEST_RESULTS_FOR_YEAR_NAME,
  component: TestResultsForYearPage,
  meta: {
    titleKey: 'navigation.pages.titles.testResults',
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.TEST_RESULTS_CRUMB,
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export const DOCUMENT_DETAIL = {
  path: DOCUMENT_DETAIL_PATH,
  name: DOCUMENT_DETAIL_NAME,
  component: DocumentDetailPage,
  meta: {
    proofLevel: proofLevel.P9,
    upliftRoute: UPLIFT_GP_MEDICAL_RECORD,
    crumb: breadcrumbs.DOCUMENT_DETAIL_CRUMB,
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.documentsDisabledRedirect],
    middleware: [gpMedicalRecordAcceptance],
    headerKey: store => store.state.documents.viewDocumentTitle,
    titleKey: store => store.state.documents.viewDocumentTitle,
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    middleware: [gpMedicalRecordAcceptance],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
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
    helpPath: GP_MEDICAL_RECORD_HELP_PATH,
    nativeNavigation: YOUR_RECORD_MENU_ITEM,
    sjrRedirectRules: [sjrRedirectRules.im1GpMedicalRecordRedirectV2],
    gpSessionOnDemand: {
      journey: GP_HEALTH_RECORD_JOURNEY_NAME,
    },
  },
};

export default [
  HEALTH_RECORDS,
  GP_MEDICAL_RECORD,
  ALLERGIESANDREACTIONS,
  MEDICINES,
  ACUTE_MEDICINES,
  CURRENT_MEDICINES,
  DISCONTINUED_MEDICINES,
  TESTRESULTS,
  TESTRESULTSV2,
  CHOOSE_TEST_RESULT_YEAR,
  TEST_RESULTS_FOR_YEAR,
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
  UPLIFT_GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
];
