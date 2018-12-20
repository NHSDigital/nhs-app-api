export const LOADED = 'LOADED';
export const LOADED_REFERENCE_DATA = 'LOADED_REFERENCE_DATA';
export const MAKE_DECISION = 'MAKE_DECISION';

export const DECISION_NOT_FOUND = 'NotFound';
export const DECISION_OPT_IN = 'OptIn';
export const DECISION_OPT_OUT = 'OptOut';

export const initialState = () => ({
  registration: {
    identifier: '',
    nhsNumber: '',
    name: {
      title: '',
      givenName: '',
      surname: '',
    },
    gender: '',
    dateOfBirth: '',
    address: {
      text: '',
      postcode: '',
    },
    emailAddress: '',
    decision: 'NotFound',
    decisionDetails: {
      all: false,
      choices: [],
    },
    faithDeclaration: 'NotStated',
    selectedEthnicity: undefined,
    selectedReligion: undefined,
  },
  referenceData: {
    ethnicities: [],
    genders: [],
    religions: [],
    titles: [],
  },
});
