/* eslint-disable import/extensions */
import myAppointments from '@/store/modules/myAppointments';

const { state } = myAppointments;
describe('state', () => {
  it('will set the appointment sessions to an empty array', () => {
    expect(state().appointmentSessions).toEqual([]);
  });

  it('will set the clinicians to an empty array', () => {
    expect(state().clinicians).toEqual([]);
  });

  it('will set the locations to an empty array', () => {
    expect(state().locations).toEqual([]);
  });

  it('will set the appointments to an empty array', () => {
    expect(state().appointments).toEqual([]);
  });
});
