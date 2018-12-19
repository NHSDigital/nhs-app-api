import AppointmentsIndex from './appointments';
import AppointmentsBooking from './appointments/booking';
import AppointmentsConfirmation from './appointments/confirmation';
import AppointmentsCancelling from './appointments/cancelling';
import AuthReturn from './authReturn';
import PrescriptionsConfirmDetails from './prescriptions/confirm-prescription-details';
import Prescriptions from './prescriptions';
import PrescriptionsRepeatCourses from './prescriptions/repeat-courses';

const status = [403, 409, 500, 502, 504, 400];
const testData = {};

for (let i = 0, max = status.length; i < max; i += 1) {
  const code = status[i];
  testData[code] = [
    [...AppointmentsIndex[code]],
    [...AppointmentsBooking[code]],
    [...AppointmentsConfirmation[code]],
    [...AppointmentsCancelling[code]],
    [...PrescriptionsConfirmDetails[code]],
    [...Prescriptions[code]],
    [...PrescriptionsRepeatCourses[code]],
  ];
}

testData[460] = [[...AppointmentsConfirmation[460]]];
testData[461] = [[...AppointmentsCancelling[461]]];
testData[464] = [[...AuthReturn[464]]];
testData[465] = [[...AuthReturn[465]]];
testData[500] = [[...AuthReturn[500]]];

export default testData;
