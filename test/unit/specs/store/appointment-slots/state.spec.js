import appointmentSlots from '../../../../../src/store/modules/appointmentSlots';

const { state } = appointmentSlots;
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

  it('will set the slots to an empty array', () => {
    expect(state().slots).toEqual([]);
  });
});
