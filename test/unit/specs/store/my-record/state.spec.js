import myRecord from '../../../../../src/store/modules/myRecord';

const { state } = myRecord;

describe('state', () => {
  it('will initialise state correctly', () => {
    expect(state().patientDemographics).toEqual(null);
    expect(state().demographicsHasLoaded).toEqual(false);
    expect(state().allergies).toEqual([]);
    expect(state().allergiesHasLoaded).toEqual(false);
  });
});
