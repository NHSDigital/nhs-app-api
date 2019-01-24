export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const LOADED_REFERENCE_DATA = 'LOADED_REFERENCE_DATA';
export const MAKE_DECISION = 'MAKE_DECISION';
export const SET_ADDITIONAL_DETAILS = 'SET_ADDITIONAL_DETAILS';
export const SET_ACCURACY_ACCEPTANCE = 'SET_ACCURACY_ACCEPTANCE';
export const SET_ALL_ORGANS = 'SET_ALL_ORGANS';
export const SET_FAITH_DECLARATION = 'SET_FAITH_DECLARATION';
export const SET_PRIVACY_ACCEPTANCE = 'SET_PRIVACY_ACCEPTANCE';
export const SET_REGISTRATION_ID = 'SET_REGISTRATION_ID';

export const DECISION_NOT_FOUND = 'NotFound';
export const DECISION_OPT_IN = 'OptIn';
export const DECISION_OPT_OUT = 'OptOut';

export const YES = 'Yes';
export const NO = 'No';
export const NOT_STATED = 'NotStated';

export const initialState = () => ({
  additionalDetails: {
    ethnicityId: '',
    religionId: '',
  },
  referenceData: {
    ethnicities: [],
    genders: [],
    religions: [],
    titles: [],
  },
  registration: {
    identifier: '',
    nhsNumber: '',
    nameFull: '',
    name: {
      title: '',
      givenName: '',
      surname: '',
    },
    gender: '',
    dateOfBirth: '',
    addressFull: '',
    address: {
      text: '',
      postcode: '',
    },
    emailAddress: '',
    decision: DECISION_NOT_FOUND,
    decisionDetails: {
      all: '',
      choices: [],
    },
    faithDeclaration: '',
  },
  isAccuracyAccepted: false,
  isPrivacyAccepted: false,
});
