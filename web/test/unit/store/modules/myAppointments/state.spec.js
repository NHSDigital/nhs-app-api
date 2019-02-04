/* eslint-disable import/extensions */
import myAppointments from '@/store/modules/myAppointments';

const { state } = myAppointments;
describe('state', () => {
  it('will set the upcoming appointments to an empty array', () => {
    expect(state().upcomingAppointments).toEqual([]);
  });

  it('will set the past appointments to an empty array', () => {
    expect(state().pastAppointments).toEqual([]);
  });

  it('will set the cancellation reasons to an empty array', () => {
    expect(state().cancellationReasons).toEqual([]);
  });
});
