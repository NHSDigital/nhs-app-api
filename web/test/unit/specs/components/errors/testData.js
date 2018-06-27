import AppointmentsIndex from './testData/appointments/index';
import AppointmentsBooking from './testData/appointments/booking';
import AppointmentsConfirmation from './testData/appointments/confirmation';

export default {
  403: [
    [...AppointmentsIndex[403]],
    [...AppointmentsBooking[403]],
    [...AppointmentsConfirmation[403]],
  ],
  409: [
    [...AppointmentsIndex[409]],
    [...AppointmentsBooking[409]],
    [...AppointmentsConfirmation[409]],
  ],
  500: [
    [...AppointmentsIndex[500]],
    [...AppointmentsBooking[500]],
    [...AppointmentsConfirmation[500]],
  ],
  502: [
    [...AppointmentsIndex[502]],
    [...AppointmentsBooking[502]],
    [...AppointmentsConfirmation[502]],
  ],
  504: [
    [...AppointmentsIndex[504]],
    [...AppointmentsBooking[504]],
    [...AppointmentsConfirmation[504]],
  ],
};
