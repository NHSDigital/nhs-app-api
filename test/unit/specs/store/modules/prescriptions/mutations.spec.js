import mutations from '@/store/modules/prescriptions/mutations';
import { PRESCRIPTIONS_LOADED } from '@/store/modules/prescriptions/mutation-types';

describe('PRESCRIPTIONS_LOADED', () => {
  it('will set the prescriptions on the state from the received prescription data', () => {
    const state = {};
    const receivedData = {
      coursesAndRepeatPrescriptions: [{}],
    };

    mutations[PRESCRIPTIONS_LOADED](state, receivedData);

    expect(state.coursesAndRepeatPrescriptions).toEqual(receivedData.coursesAndRepeatPrescriptions);
  });
});
