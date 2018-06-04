import {
  SLOT_SELECTED,
  SLOTS_LOADED,
} from '../../../../../src/store/modules/appointmentSlots/mutation-types';
import actions from '../../../../../src/store/modules/appointmentSlots/actions';

const API_HOST = 'http://unit.test';

const { load, select } = actions;

describe('load', () => {
  it('will request the appointment slots from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientAppointmentSlots: jest.fn().mockResolvedValue(),
        },
      },
    };
    return load.call(that, { commit: jest.fn() }, { API_HOST }).then(() => {
      expect(that.app.$http.getV1PatientAppointmentSlots).toBeCalled();
    });
  });

  it('will call commit with the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    const that = {
      app: {
        $http: {
          getV1PatientAppointmentSlots: () => Promise.resolve(expected),
        },
      },
    };

    const commit = jest.fn();

    return load
      .call(that, { commit }, { API_HOST })
      .then(() => expect(commit).toBeCalledWith(SLOTS_LOADED, expected));
  });
});

describe('select', () => {
  it('will commit the received slot ID using the SLOT_SELECTED mutation type', () => {
    const slotId = '1234';
    const commit = jest.fn();

    select({ commit }, slotId);

    expect(commit).toBeCalledWith(SLOT_SELECTED, slotId);
  });
});
