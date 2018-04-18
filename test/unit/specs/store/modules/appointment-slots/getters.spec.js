import { slots } from '@/store/modules/appointment-slots/getters';

describe('getters', () => {
  describe('slots', () => {
    it('will set the location on the slots from the state`s locations', () => {
      const state = {
        locations: [
          { id: 'a' },
          { id: 'b' },
          { id: 'c' },
        ],
        slots: [
          { id: 'x', locationId: 'b' },
          { id: 'y', locationId: 'c' },
          { id: 'z', locationId: 'a' },
        ],
      };

      const result = slots(state);

      expect(result[0].location).toEqual(state.locations[1]);
      expect(result[1].location).toEqual(state.locations[2]);
      expect(result[2].location).toEqual(state.locations[0]);
    });
  });

  it('will not set the location for slots without a location ID', () => {
    const state = {
      locations: [
        { id: 'a' },
        { id: 'b' },
        { id: 'c' },
      ],
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].location).toEqual(undefined);
  });

  it('will not set the location when there are no locations', () => {
    const state = {
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].location).toEqual(undefined);
  });

  it('will set the appointment session on the slots from the state`s appointment sessions', () => {
    const state = {
      appointmentSessions: [
        { id: 'a' },
        { id: 'b' },
        { id: 'c' },
      ],
      slots: [
        { id: 'x', appointmentSessionId: 'a' },
        { id: 'y', appointmentSessionId: 'b' },
        { id: 'z', appointmentSessionId: 'c' },
      ],
    };

    const result = slots(state);

    expect(result[0].appointmentSession).toEqual(state.appointmentSessions[0]);
    expect(result[1].appointmentSession).toEqual(state.appointmentSessions[1]);
    expect(result[2].appointmentSession).toEqual(state.appointmentSessions[2]);
  });

  it('will not set the appointment session for slots without an appointment session ID', () => {
    const state = {
      appointmentSessions: [
        { id: 'a' },
        { id: 'b' },
        { id: 'c' },
      ],
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].appointmentSession).toEqual(undefined);
  });

  it('will not set the appointment session when there are no appointment sessions', () => {
    const state = {
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].appointmentSession).toEqual(undefined);
  });

  it('will the set clinicians on the slots from the state`s clinican IDs', () => {
    const state = {
      clinicians: [
        { id: 'a' },
        { id: 'b' },
        { id: 'c' },
      ],
      slots: [
        { id: 'x', clinicianIds: ['a', 'b'] },
        { id: 'y', clinicianIds: ['b'] },
        { id: 'z', clinicianIds: ['c'] },
      ],
    };

    const result = slots(state);

    expect(result[0].clinicians).toEqual([state.clinicians[0], state.clinicians[1]]);
    expect(result[1].clinicians).toEqual([state.clinicians[1]]);
    expect(result[2].clinicians).toEqual([state.clinicians[2]]);
  });

  it('will have empty clinicians for slots without clinicial IDs', () => {
    const state = {
      clinicians: [
        { id: 'a' },
        { id: 'b' },
        { id: 'c' },
      ],
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].clinicians).toEqual([]);
  });

  it('will have empty clinicians when there are no clinicians', () => {
    const state = {
      slots: [
        { id: 'x' },
      ],
    };

    const result = slots(state);

    expect(result[0].clinicians).toEqual([]);
  });
});
