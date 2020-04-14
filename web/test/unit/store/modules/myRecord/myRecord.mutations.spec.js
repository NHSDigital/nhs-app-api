/* eslint-disable no-param-reassign */
import mutations from '@/store/modules/myRecord/mutations';
import {
  initialState,
  ACCEPT_TERMS,
  LOADED,
  LOADED_DETAILED_TEST_RESULT,
  TOGGLE_PATIENT_DETAIL,
  SET_RELOAD,
} from '@/store/modules/myRecord/mutation-types';
import each from 'jest-each';

const DEFAULT_DOCUMENTS = () => [{
  name: 'Document1',
  extension: 'tga',
  comments: 'You are very sick',
}, {
  name: 'Document2',
  extension: 'jpg',
  codeId: 100,
  term: 'happy times',
  eventGuid: 'e1498fe9-f59a-47d1-bffc-24dd120ca7c0',
}, {
  name: 'Document3',
  extension: 'jpg',
  codeId: 300,
  term: 'weird times',
  eventGuid: 'b46060e2-f245-4b5e-a250-cc02f29badeb',
}, {
  name: 'The one with the comments in it',
  extension: 'pdf',
  codeId: 400,
  term: 'turbo killer',
  eventGuid: 'a49060e2-f245-4b5e-a250-cc02f29badef',
  comments: 'Suckin\' diesel now so we are',
}];

describe('my record mutations', () => {
  let state;
  let data;

  function resetState(modifyData) {
    data = {
      record: {
        medications: {
          hasErrored: false,
          data: {
            acuteMedications: {},
            currentRepeatMedications: {},
            discontinuedRepeatMedications: {},
          },
        },
        documents: {
          data: DEFAULT_DOCUMENTS(),
        },
        consultations: {
          data: [{
            consultationHeaders: [{
              header: 'Allergy',
            }, {
              header: 'Document',
              comments: [
                'Do you know the muffin man?',
                'Curly wurlys are great!',
              ],
              observationsWithTerm: {
                codeId: 100,
                term: 'happy times',
                eventGuid: 'e1498fe9-f59a-47d1-bffc-24dd120ca7c0',
              },
            }, {
              header: 'Comment',
              observationsWithTerm: {
                codeId: 100,
                term: 'happy times',
                eventGuid: 'e1498fe9-f59a-47d1-bffc-24dd120ca7c0',
              },
            }, {
              header: 'Document',
              comments: [
                'Another glorious comment',
              ],
              observationsWithTerm: {
                codeId: 100,
                term: 'happy times',
                eventGuid: 'e1498fe9-f59a-47d1-bffc-24dd120ca7c0',
              },
            }, {
              header: 'Document',
              comments: [
                'If you see me this code is broken',
              ],
              observationsWithTerm: {
                codeId: 200,
                term: 'sad times',
                eventGuid: 'cd77cce5-b77c-4a90-ba0a-59602644e51a',
              },
            }],
          }, {
            consultationHeaders: [{
              header: 'Medicine',
            }],
          }],
        },
      },
      patientDetails: 'details',
    };

    if (typeof modifyData === 'function') {
      modifyData(data);
    }

    state = initialState();
  }

  beforeEach(() => resetState());

  describe('ACCEPT_TERMS', () => {
    beforeEach(() => {
      mutations[ACCEPT_TERMS](state, data);
    });

    it('will set the my record term acceptance flag ', () => {
      expect(state.hasAcceptedTerms).toEqual(true);
    });
  });

  describe('SET_RELOAD', () => {
    beforeEach(() => {
      mutations[SET_RELOAD](state, false);
    });

    it('will set reload to false', () => {
      expect(state.reload).toEqual(false);
    });
  });

  describe('LOADED', () => {
    beforeEach(() => {
      mutations[LOADED](state, data);
    });

    it('will set the patient details ', () => {
      expect(state.patientDetails).toEqual(data.patientDetails);
    });

    it('will set the patient record', () => {
      expect(state.record).toEqual(data.record);
    });

    it('will set has loaded to true', () => {
      expect(state.hasLoaded).toEqual(true);
    });

    it('will set is patient details collapsed to false', () => {
      expect(state.isPatientDetailsCollapsed).toEqual(false);
    });

    it('will set the patient documents', () => {
      resetState((d) => {
        // omit data not specific to this test
        delete d.record.consultations.data;
        d.record.documents.data.forEach(doc => delete doc.comments);
      });
      mutations[LOADED](state, data);

      const docs = DEFAULT_DOCUMENTS();

      // omit data not specific to this assertion
      docs.forEach(doc => delete doc.comments);

      expect(state.record.documents.data).toEqual(docs);
    });

    it('will set the patient documents to an empty array when no documents found in record', () => {
      resetState(d => delete d.record.documents.data);
      mutations[LOADED](state, data);

      expect(state.record.documents.data).toEqual([]);
    });

    it('will set the record count', () => {
      expect(state.record.documents.recordCount).toEqual(4);
    });

    it('will set medication error flags if record medications has errored', () => {
      resetState((d) => { d.record.medications.hasErrored = true; });
      mutations[LOADED](state, data);

      expect(state.record.medications.data.acuteMedications.hasErrored).toEqual(true);
      expect(state.record.medications.data.currentRepeatMedications.hasErrored).toEqual(true);
      expect(state.record.medications.data.discontinuedRepeatMedications.hasErrored).toEqual(true);
    });

    it('will load document comments when related consultations are present', () => {
      expect(state.record.documents.data[1].comments).toEqual([
        'Do you know the muffin man?',
        'Curly wurlys are great!',
        'Another glorious comment',
      ]);
    });

    it('will not load document comments when related consultations are not present', () => {
      expect(state.record.documents.data[2].comments).toEqual(undefined);
    });

    each([
      ['is not found in record', (d) => { delete d.record.consultations.data; }],
      ['is an empty array', (d) => { d.record.consultations.data = []; }],
    ]).it('should not load document comments if consultations %s', (_, modifier) => {
      resetState(modifier);
      mutations[LOADED](state, data);

      expect(state.record.documents.data[1].comments).toEqual(undefined);
    });

    it('will convert the comments field in document response to an array when it is present', () => {
      expect(state.record.documents.data[3].comments).toEqual(['Suckin\' diesel now so we are']);
    });
  });

  describe('LOADED_DETAILED_TEST_RESULT', () => {
    const testResults = { data: 'foobar' };

    beforeEach(() => {
      mutations[LOADED_DETAILED_TEST_RESULT](state, testResults);
    });

    it('will set the test results data to the received test results', () => {
      expect(state.detailedTestResult.data).toEqual(testResults.data);
    });

    it('will set the has loaded property of the test results to true', () => {
      expect(state.detailedTestResult.hasLoaded).toEqual(true);
    });
  });

  describe('TOGGLE_PATIENT_DETAIL', () => {
    it('will set is patient details collapsed to true if it is initially false', () => {
      state.isPatientDetailsCollapsed = false;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(true);
    });

    it('will set is patient details collapsed to false if it is initially true', () => {
      state.isPatientDetailsCollapsed = true;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(false);
    });

    it('will set is patient details collapsed to true if it is initially undefined', () => {
      state.isPatientDetailsCollapsed = undefined;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(true);
    });
  });
});
