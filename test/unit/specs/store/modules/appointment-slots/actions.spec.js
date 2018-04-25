import Vue from 'vue';
import { load, select } from '@/store/modules/appointment-slots/actions';
import { SLOT_SELECTED, SLOTS_LOADED } from '@/store/modules/appointment-slots/mutation-types';

const API_HOST = 'http://unit.test';

let original$http;

beforeEach(() => {
  original$http = Vue.$http;
});

afterEach(() => {
  Vue.$http = original$http;
});


describe('load', () => {
  it('will request the appointment slots from the backend', () => {
    Vue.$http = {
      getV1PatientAppointmentslots: jest.fn().mockResolvedValue(),
    };

    return load({ commit: jest.fn() }, { API_HOST })
      .then(() => {
        expect(Vue.$http.getV1PatientAppointmentslots).toBeCalled();
      });
  });

  it('will call commit with the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    Vue.$http = {
      getV1PatientAppointmentslots: () => Promise.resolve(expected),
    };

    const commit = jest.fn();

    return load({ commit }, { API_HOST })
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
