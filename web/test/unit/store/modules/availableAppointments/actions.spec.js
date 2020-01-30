import {
  ADD_ERROR,
  BOOKING_JOURNEY_START,
  CLEAR_ERROR,
  LOAD,
  SELECT,
  SET_BOOKING_REASON_NECESSITY,
} from '@/store/modules/availableAppointments/mutation-types';
import actions from '@/store/modules/availableAppointments/actions';

describe('actions', () => {
  describe('book', () => {
    const slot = 'foo_slot';
    let commit;
    let that;
    let response;

    beforeEach(() => {
      that = {
        app: {
          $http: {
            postV1PatientAppointments: jest.fn()
              .mockImplementation(() => response),
          },
        },
        dispatch: jest.fn(),
      };
      commit = jest.fn();
    });

    describe('on success', () => {
      beforeEach(async () => {
        response = Promise.resolve();
        await actions.book.call(that, { commit }, slot);
      });

      it('will request to book the appointment from the backend', () => {
        expect(that.app.$http.postV1PatientAppointments).toBeCalledWith({
          appointmentBookRequest: slot,
          ignoreError: true,
        });
      });

      it('will commit BOOKING_JOURNEY_START', () => {
        expect(commit).toBeCalledWith(BOOKING_JOURNEY_START);
      });

      it('will not commit ADD_ERROR', () => {
        expect(commit).not.toBeCalledWith(ADD_ERROR, jasmine.anything());
      });

      describe('on server', () => {
        beforeEach(async () => {
          that.dispatch = jest.fn();
          process.client = false;
          await actions.book.call(that, { commit });
        });

        it('will not dispatch `analytics/satelliteTrack`', () => {
          expect(that.dispatch).not.toBeCalled();
        });
      });

      describe('on client', () => {
        beforeEach(async () => {
          that.dispatch = jest.fn();
          process.client = true;
          await actions.book.call(that, { commit });
        });

        it('will dispatch `analytics/satelliteTrack`', () => {
          expect(that.dispatch).toBeCalledWith('analytics/satelliteTrack', 'appointment_booked');
        });
      });
    });

    describe('on failure', () => {
      const error = {
        response: {
          status: 500,
          data: {
            serviceDeskReference: 'xxx',
          },
        },
      };

      beforeEach(async () => {
        process.client = true;
        response = Promise.reject(error);
        await actions.book.call(that, { commit }, slot);
      });

      it('will request to book the appointment from the backend', () => {
        expect(that.app.$http.postV1PatientAppointments).toBeCalledWith({
          appointmentBookRequest: slot,
          ignoreError: true,
        });
      });

      it('will not commit BOOKING_JOURNEY_START', () => {
        expect(commit).not.toBeCalledWith(BOOKING_JOURNEY_START);
      });

      it('will not dispatch `analytics/satelliteTrack`', () => {
        expect(that.dispatch).not.toBeCalled();
      });

      it('will commit ADD_ERROR', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, {
          status: 500,
          serviceDeskReference: 'xxx',
        });
      });
    });
  });

  describe('load', () => {
    let commit;
    let that;
    let response;

    beforeEach(() => {
      commit = jest.fn();
      that = {
        app: {
          $http: {
            getV1PatientAppointmentSlots: jest.fn()
              .mockImplementation(() => response),
          },
        },
      };
    });

    describe('on success', () => {
      const expected = {
        data: { foo: 'bar' },
      };

      beforeEach(async () => {
        response = Promise.resolve(expected);
        await actions.load.call(that, { commit });
      });

      it('will request the appointment slots from the backend', () => {
        expect(that.app.$http.getV1PatientAppointmentSlots).toBeCalledWith({
          ignoreError: true,
        });
      });

      it('will commit LOAD', () => {
        expect(commit).toBeCalledWith(LOAD, expected);
      });

      it('will not commit ADD_ERROR', () => {
        expect(commit).not.toBeCalledWith(ADD_ERROR, jasmine.anything());
      });
    });

    describe('on failure', () => {
      const error = {
        response: {
          status: 500,
          data: {
            serviceDeskReference: 'yyy',
          },
        },
      };

      beforeEach(async () => {
        response = Promise.reject(error);
        await actions.load.call(that, { commit });
      });

      it('will request the appointment slots from the backend', () => {
        expect(that.app.$http.getV1PatientAppointmentSlots).toBeCalledWith({
          ignoreError: true,
        });
      });

      it('will not commit LOAD', () => {
        expect(commit).not.toBeCalledWith(LOAD, jasmine.anything());
      });

      it('will commit ADD_ERROR', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, {
          status: 500,
          serviceDeskReference: 'yyy',
        });
      });
    });
  });

  describe('select', () => {
    let commit;

    beforeEach(() => {
      commit = jest.fn();
      actions.select({ commit }, '1234');
    });

    it('will commit the received slot ID using the SELECT mutation type', () => {
      expect(commit).toBeCalledWith(SELECT, '1234');
    });
  });

  describe('set booking reason necessity', () => {
    let commit;

    beforeEach(() => {
      commit = jest.fn();
      actions.setBookingReasonNecessity({ commit }, 'NotAllowed');
    });

    it('will commit the received value using the SET_BOOKING_REASON_NECESSITY mutation type', () => {
      expect(commit).toBeCalledWith(SET_BOOKING_REASON_NECESSITY, 'NotAllowed');
    });
  });

  describe('clear error', () => {
    let commit;

    beforeEach(() => {
      commit = jest.fn();
      actions.clearError({ commit });
    });

    it('will commit CLEAR_ERROR', () => {
      expect(commit).toBeCalledWith(CLEAR_ERROR);
    });
  });
});
