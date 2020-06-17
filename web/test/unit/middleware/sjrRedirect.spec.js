import sjrRedirect from '@/middleware/sjrRedirect';
import {
  GP_AT_HAND,
  GP_APPOINTMENTS,
  INFORMATICA,
  HOSPITAL_APPOINTMENTS,
} from '@/router/routes/appointments';
import {
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  DOCUMENTS,
  DOCUMENT,
  DOCUMENT_DETAIL,
} from '@/router/routes/medical-record';
import { PRESCRIPTIONS_GP_AT_HAND, PRESCRIPTIONS } from '@/router/routes/prescriptions';
import { ACCOUNT_NOTIFICATIONS } from '@/router/routes/account';
import {
  GP_MESSAGES,
  GP_MESSAGES_URGENCY,
  GP_MESSAGES_URGENCY_CONTACT_YOUR_GP,
  GP_MESSAGES_RECIPIENTS,
  GP_MESSAGES_VIEW_DETAILS,
  GP_MESSAGES_SEND_MESSAGE,
  GP_MESSAGES_DELETE,
  GP_MESSAGES_DELETE_SUCCESS,
} from '@/router/routes/messages';
import {
  APPOINTMENTS_PATH,
  APPOINTMENT_GP_AT_HAND_PATH,
  APPOINTMENT_INFORMATICA_PATH,
  GP_APPOINTMENTS_PATH,
  GP_MEDICAL_RECORD_PATH,
  INDEX_PATH,
  GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTIONS_GP_AT_HAND_PATH,
  HEALTH_RECORDS_PATH,
} from '@/router/paths';
import * as dependency from '@/lib/utils';

describe('middleware/sjrRedirect', () => {
  let getters;
  let next;
  let store;
  dependency.createRoutePathObject = jest.fn(x => ({ path: x.path }));

  const callSjrRedirect = (route) => {
    const to = {
      ...route,
    };
    sjrRedirect({ next, to, store });
  };

  beforeEach(() => {
    getters = {};
    store = {
      getters,
    };
    next = jest.fn();
  });

  describe('appointment gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(GP_AT_HAND);
      });

      it('will redirect to appointments', () => {
        expect(next).toBeCalledWith({ path: GP_APPOINTMENTS_PATH });
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callSjrRedirect(GP_AT_HAND);
      });

      it('will redirect to appointments informatica', () => {
        expect(next).toBeCalledWith({ path: APPOINTMENT_INFORMATICA_PATH });
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });
  });

  describe('appointments im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(GP_APPOINTMENTS);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callSjrRedirect(GP_APPOINTMENTS);
      });

      it('will redirect to appointments informatica', () => {
        expect(next).toBeCalledWith({ path: APPOINTMENT_INFORMATICA_PATH });
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(GP_APPOINTMENTS);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(next).toBeCalledWith({ path: APPOINTMENT_GP_AT_HAND_PATH });
      });
    });
  });

  describe('appointment informatica redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(INFORMATICA);
      });

      it('will redirect to appointments', () => {
        expect(next).toBeCalledWith({ path: GP_APPOINTMENTS_PATH });
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = false;
        callSjrRedirect(INFORMATICA);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(INFORMATICA);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(next).toBeCalledWith({ path: APPOINTMENT_GP_AT_HAND_PATH });
      });
    });
  });

  describe('my record version 2 redirect rules', () => {
    describe('sjr version 2 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpMedicalRecordV2Enabled'] = true;
        callSjrRedirect(GP_MEDICAL_RECORD);
      });

      it('will not redirect to my record', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1GpMedicalRecordV2Enabled'] = true;

        callSjrRedirect(GP_MEDICAL_RECORD_GP_AT_HAND);
      });

      it('will redirect to my record gp at hand', () => {
        expect(next).toBeCalledWith({ path: HEALTH_RECORDS_PATH });
      });
    });
  });

  describe('my record im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MyRecordEnabled'] = true;
        callSjrRedirect(GP_MEDICAL_RECORD);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        store = {
          getters: {
            'serviceJourneyRules/silverIntegrationEnabled': () => (false),
            'serviceJourneyRules/gpAtHandMyRecordEnabled': true,
          },
        };
        callSjrRedirect(GP_MEDICAL_RECORD);
      });
      it('will redirect to my record gp at hand', () => {
        expect(next).toBeCalledWith({ path: GP_MEDICAL_RECORD_GP_AT_HAND_PATH });
      });
    });
  });

  describe('prescriptions gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will redirect to prescriptions', () => {
        expect(next).toBeCalledWith({ path: PRESCRIPTIONS_PATH });
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });
  });

  describe('prescriptions im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS);
      });

      it('will redirect to prescriptions gp at hand', () => {
        expect(next).toBeCalledWith({ path: PRESCRIPTIONS_GP_AT_HAND_PATH });
      });
    });
  });

  describe('account notifications rules', () => {
    describe('notifications enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/notificationsEnabled'] = true;
        callSjrRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('notifications disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/notificationsEnabled'] = false;
        callSjrRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will redirect to account', () => {
        expect(next).toBeCalledWith({ path: INDEX_PATH });
      });
    });
  });

  describe('documents rules', () => {
    const documentsRoutes = [DOCUMENTS, DOCUMENT, DOCUMENT_DETAIL];

    describe('documents enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/documentsEnabled'] = true;
        documentsRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('documents disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/documentsEnabled'] = false;
        documentsRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will redirect to home', () => {
        expect(next).toBeCalledWith({ path: GP_MEDICAL_RECORD_PATH });
        expect(next).toHaveBeenCalledTimes(documentsRoutes.length);
      });
    });
  });

  /*  describe('im1Messaging rules', () => {
    const gpDeleteMessagesRoutes = [
      GP_MESSAGES_DELETE,
      GP_MESSAGES_DELETE_SUCCESS,
    ];

    describe('deleteMessageRedirect enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/deleteGpMessagesEnabled'] = true;
        gpDeleteMessagesRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('deleteMessageRedirect disabled', () => {
      beforeEach(() => {
       // getters['serviceJourneyRules/im1MessagingEnabled'] = true;
        getters['serviceJourneyRules/deleteGpMessagesEnabled'] = false;
        gpMessagesRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will redirect to GP_MESSAGES_PATH', () => {
        expect(next).toBeCalledWith({ path: GP_MESSAGES_PATH });
        expect(next).toHaveBeenCalledTimes(gpMessagesRoutes.length);
      });
    });
  });
  */


  // ADMIN_HELP

  describe('im1Messaging rules', () => {
    const gpMessagesRoutes = [
      GP_MESSAGES,
      GP_MESSAGES_URGENCY,
      GP_MESSAGES_URGENCY_CONTACT_YOUR_GP,
      GP_MESSAGES_RECIPIENTS,
      GP_MESSAGES_VIEW_DETAILS,
      GP_MESSAGES_SEND_MESSAGE,
      GP_MESSAGES_DELETE,
      GP_MESSAGES_DELETE_SUCCESS,
    ];

    describe('im1Messaging enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MessagingEnabled'] = true;
        gpMessagesRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('im1Messaging disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MessagingEnabled'] = false;
        gpMessagesRoutes.forEach(route => callSjrRedirect(route));
      });

      it('will redirect to home', () => {
        expect(next).toBeCalledWith({ path: INDEX_PATH });
        expect(next).toHaveBeenCalledTimes(gpMessagesRoutes.length);
      });
    });
  });

  describe('secondary appointments rules', () => {
    const routes = [
      HOSPITAL_APPOINTMENTS,
    ];

    describe('secondary appointments enabled', () => {
      beforeEach(() => {
        store = {
          getters: {
            'serviceJourneyRules/silverIntegrationEnabled': () => (true),
          },
        };
        routes.forEach(route => callSjrRedirect(route));
      });

      it('will not redirect', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });
    describe('secondary appointments disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/silverIntegrationAppointmentsEnabled'] = false;
        routes.forEach(route => callSjrRedirect(route));
      });

      it('will redirect to appointments', () => {
        expect(next).toBeCalledWith({ path: APPOINTMENTS_PATH });
      });
    });
  });
});
