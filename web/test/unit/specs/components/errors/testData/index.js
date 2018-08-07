import AppointmentsIndex from './appointments/index';
import AppointmentsBooking from './appointments/booking';
import AppointmentsConfirmation from './appointments/confirmation';
import AppointmentsCancel from './appointments/cancel';
import AuthReturn from './authReturn';
import PrescriptionsConfirmDetails from './prescriptions/confirm-prescription-details';
import Prescriptions from './prescriptions/index';
import PrescriptionsRepeatCourses from './prescriptions/repeat-courses';

const status = [403, 409, 500, 502, 504, 400];
const testData = {};

for (let i = 0, max = status.length; i < max; i += 1) {
  const code = status[i];
  testData[code] = [
    [...AppointmentsIndex[code]],
    [...AppointmentsBooking[code]],
    [...AppointmentsConfirmation[code]],
    [...AppointmentsCancel[code]],
    [...PrescriptionsConfirmDetails[code]],
    [...Prescriptions[code]],
    [...PrescriptionsRepeatCourses[code]],
  ];
}

testData[460] = [[...AppointmentsConfirmation[460]]];
testData[461] = [[...AppointmentsCancel[461]]];
testData[464] = [[...AuthReturn[464]]];
testData[465] = [[...AuthReturn[465]]];

export default testData;
