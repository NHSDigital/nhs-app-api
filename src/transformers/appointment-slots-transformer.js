// import { find } from 'lodash/fp';
//
// const findById = id => find(item => item.Id === id);
// const mapByIds = (ids, items) => ids.map(id => findById(id)(items));
// const mapClinicians = (ids, clinicians) =>
//   mapByIds(ids, clinicians).map(clinician => clinician.DisplayName);
// const mapDisplayName = (id, items) => findById(id)(items).DisplayName
//
// export default (data) => {
//   const slots = data.Slots.map((slotData) => {
//     const slot = {};
//     Object.keys(slotData).forEach((key) => {
//       switch (key) {
//         case 'AppointmentSessionId':
//           slot.appointmentSession =
//             mapDisplayName(slotData.AppointmentSessionId, data.AppointmentSessions);
//           break;
//         case 'ClinicianIds':
//           slot.clinicians = mapClinicians(slotData.ClinicianIds, data.Clinicians);
//           break;
//         case 'LocationId':
//           slot.location = mapDisplayName(slotData.LocationId, data.Locations);
//           break;
//         default:
//           slot[key] = slotData[key];
//           break;
//       };
//     });
//     return slot;
//   });
//
//   alert(JSON.stringify(slots, null, 2))
//   return slots;
// };
