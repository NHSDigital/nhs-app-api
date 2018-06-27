import getters from '../../../../../src/store/modules/appointmentSlots/getters';

const { slots } = getters;
describe('getters', () => {
  describe('slots', () => {
    it('will return slots', () => {
      const state = {
        slots: [{ id: 1 }],
      };

      const result = slots(state);

      expect(result).toEqual([{ id: 1 }]);
    });
  });
});
