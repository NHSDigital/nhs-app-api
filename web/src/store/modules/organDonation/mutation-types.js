export const CLONE_FROM_ORIGINAL = 'CLONE_FROM_ORIGINAL';
export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const LOADED_REFERENCE_DATA = 'LOADED_REFERENCE_DATA';
export const MAKE_DECISION = 'MAKE_DECISION';
export const RESET_REGISTRATION = 'RESET_REGISTRATION';
export const SET_ADDITIONAL_DETAILS = 'SET_ADDITIONAL_DETAILS';
export const SET_ACCURACY_ACCEPTANCE = 'SET_ACCURACY_ACCEPTANCE';
export const SET_AMENDING = 'SET_AMENDING';
export const SET_ALL_ORGANS = 'SET_ALL_ORGANS';
export const SET_FAITH_DECLARATION = 'SET_FAITH_DECLARATION';
export const SET_PRIVACY_ACCEPTANCE = 'SET_PRIVACY_ACCEPTANCE';
export const SET_REGISTRATION_ID = 'SET_REGISTRATION_ID';
export const SET_SOME_ORGANS = 'SET_SOME_ORGANS';
export const SET_STATE = 'SET_STATE';
export const SET_REAFFIRMING = 'SET_REAFFIRMING';
export const UPDATE_ORIGINAL_REGISTRATION = 'UPDATE_ORIGINAL_REGISTRATION';
export const DECISION_APPOINTED_REP = 'AppRep';
export const DECISION_OPT_IN = 'OptIn';
export const DECISION_OPT_OUT = 'OptOut';
export const DECISION_UNKNOWN = 'Unknown';
export const STATE_CONFLICTED = 'Conflicted';
export const STATE_NOT_FOUND = 'NotFound';
export const STATE_OK = 'Ok';
export const YES = 'Yes';
export const NO = 'No';
export const NOT_STATED = 'NotStated';

const createRegistration = () => ({
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
  decision: DECISION_UNKNOWN,
  decisionDetails: {
    all: '',
    choices: {
      heart: 'NotStated',
      lungs: 'NotStated',
      kidney: 'NotStated',
      liver: 'NotStated',
      corneas: 'NotStated',
      pancreas: 'NotStated',
      tissue: 'NotStated',
      smallBowel: 'NotStated',
    },
  },
  faithDeclaration: '',
  state: STATE_NOT_FOUND,
});

export const initialState = () => ({
  additionalDetails: {
    ethnicityId: '',
    religionId: '',
  },
  originalRegistration: createRegistration(),
  referenceData: {
    ethnicities: [],
    genders: [],
    religions: [],
    titles: [],
  },
  registration: createRegistration(),
  isAccuracyAccepted: false,
  isAmending: false,
  isPrivacyAccepted: false,
  isReaffirming: false,
});
