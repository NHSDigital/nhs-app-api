import actions from '@/store/modules/myAppointments/actions';
import { ADD_ERROR, CANCELLING_JOURNEY_START, CLEAR_ERROR, LOADED } from '@/store/modules/myAppointments/mutation-types';

describe('actions', () => {
  let commit;

  beforeEach(() => {
    commit = jest.fn();
  });

  describe('clearError', () => {
    beforeEach(() => {
      actions.clearError({ commit });
    });

    it('will commit CLEAR_ERROR', () => {
      expect(commit).toBeCalledWith(CLEAR_ERROR);
    });
  });

  describe('load', () => {
    let that;

    beforeEach(() => {
      that = {
        app: {
          $http: {
            getV1PatientAppointments: null,
          },
        },
        dispatch: jest.fn(),
      };
    });

    describe('on success', () => {
      const response = 'response data';

      beforeEach(async () => {
        that.app.$http.getV1PatientAppointments = jest.fn().mockResolvedValue(response);
        await actions.load.call(that, { commit });
      });

      it('will request the patient appointments from the backend', () => {
        expect(that.app.$http.getV1PatientAppointments).toBeCalledWith({
          ignoreError: true,
        });
      });

      it('will commit LOADED', () => {
        expect(commit).toBeCalledWith(LOADED, response);
      });

      it('will dispatch device/unlockNavBar', async () => {
        expect(that.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
      });

      it('will not commit ADD_ERROR', () => {
        expect(commit).not.toBeCalledWith(ADD_ERROR, expect.anything());
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
        that.app.$http.getV1PatientAppointments = jest.fn().mockRejectedValue(error);
        await actions.load.call(that, { commit });
      });

      it('will request the patient appointments from the backend', () => {
        expect(that.app.$http.getV1PatientAppointments).toBeCalledWith({
          ignoreError: true,
        });
      });

      it('will commit ADD_ERROR', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, {
          status: 500,
          serviceDeskReference: 'xxx',
        });
      });

      it('will dispatch device/unlockNavBar', async () => {
        expect(that.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
      });

      it('will not commit LOADED', () => {
        expect(commit).not.toBeCalledWith(LOADED, expect.anything());
      });
    });
  });

  describe('cancel', () => {
    let data;
    let that;

    beforeEach(() => {
      data = { foo: 'bar' };
      that = {
        app: {
          $http: {
            deleteV1PatientAppointments: null,
          },
        },
        dispatch: jest.fn(),
      };
    });

    describe('on success', () => {
      beforeEach(async () => {
        that.app.$http.deleteV1PatientAppointments = jest.fn().mockResolvedValue();
        await actions.cancel.call(that, { commit }, data);
      });

      it('will call to delete the patients appointment from the backend', () => {
        expect(that.app.$http.deleteV1PatientAppointments)
          .toBeCalledWith({ appointmentCancelRequest: data, ignoreError: true });
      });

      it('will commit CANCELLING_JOURNEY_START', () => {
        expect(commit).toBeCalledWith(CANCELLING_JOURNEY_START);
      });

      it('will not commit ADD_ERROR', () => {
        expect(commit).not.toBeCalledWith(ADD_ERROR, expect.anything());
      });


      it('will dispatch `analytics/satelliteTrack``', () => {
        expect(that.dispatch).toBeCalledWith('analytics/satelliteTrack', 'appointment_cancelled');
      });
    });

    describe('on exception', () => {
      const errorResponse = {
        response: {
          status: 500,
          data: {
            serviceDeskReference: 'foo',
          },
        },
      };

      beforeEach(async () => {
        that.app.$http.deleteV1PatientAppointments = jest.fn().mockRejectedValue(errorResponse);
        await actions.cancel.call(that, { commit }, data);
      });

      it('will call to delete the patients appointment from the backend', () => {
        expect(that.app.$http.deleteV1PatientAppointments)
          .toBeCalledWith({ appointmentCancelRequest: data, ignoreError: true });
      });

      it('will add error', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, {
          status: 500,
          serviceDeskReference: 'foo',
        });
      });

      it('will not commit CANCELLING_JOURNEY_START', () => {
        expect(commit).not.toBeCalledWith(CANCELLING_JOURNEY_START);
      });

      it('will not dispatch `analytics/satelliteTrack`', () => {
        expect(that.dispatch).not.toBeCalled();
      });
    });
  });
});
