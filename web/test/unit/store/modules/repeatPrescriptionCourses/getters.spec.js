import getters from '@/store/modules/repeatPrescriptionCourses/getters';

describe('getters', () => {
  const { selectedPrescriptions, isValid, selectedIds, specialRequestValid } = getters;
  let currentState;

  describe('selectedPrescriptions', () => {
    describe('selectedPrescriptions filters correctly', () => {
      it('will only return selected prescriptions', () => {
        // arrange
        currentState = {
          repeatPrescriptionCourses: [{
            id: 'prescription-1',
            selected: true,
          },
          {
            id: 'prescription-2',
            selected: false,
          },
          {
            id: 'prescription-3',
            selected: true,
          }],
        };

        // act
        const result = selectedPrescriptions(currentState);

        // assert
        expect(result.length).toBe(2);
        expect(result[0].id).toBe('prescription-1');
        expect(result[1].id).toBe('prescription-3');
      });
    });
  });

  describe('isValid', () => {
    it('returns true when there is at least one selected prescription', () => {
      // arrange
      currentState = {};

      const stubGetters = {
        selectedPrescriptions: [{
          id: 'prescription-1',
          selected: true,
        }],
      };

      // act
      const result = isValid(currentState, stubGetters);

      // assert
      expect(result).toBe(true);
    });

    it('returns false when there are no selected prescriptions', () => {
      // arrange
      currentState = {};

      const stubGetters = {
        selectedPrescriptions: [],
      };

      // act
      const result = isValid(currentState, stubGetters);

      // assert
      expect(result).toBe(false);
    });
  });

  describe('selectedIds', () => {
    it('returns selected prescription course ids', () => {
      // arrange
      currentState = {};

      const stubGetters = {
        selectedPrescriptions: [{
          id: 'prescription-1',
          selected: true,
        }],
      };

      // act
      const result = selectedIds(currentState, stubGetters);

      // assert
      expect(result.length).toBe(1);
      expect(result).toEqual(['prescription-1']);
    });

    it('returns empty array when no prescriptions are selected', () => {
      // arrange
      currentState = {};

      const stubGetters = {
        selectedPrescriptions: [],
      };

      // act
      const result = selectedIds(currentState, stubGetters);

      // assert
      expect(result).toEqual([]);
    });
  });

  describe('specialRequestValid', () => {
    it('should be true when special request value is entered and value is mandatory', () => {
      // arrange
      currentState = {
        specialRequest: 'Please be quick.',
        specialRequestNecessity: 'Mandatory',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(true);
    });

    it('should be false when special request value is empty and value is mandatory', () => {
      // arrange
      currentState = {
        specialRequest: '',
        specialRequestNecessity: 'Mandatory',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(false);
    });

    it('should be false when special request value is whitespace and value is mandatory', () => {
      // arrange
      currentState = {
        specialRequest: '  ',
        specialRequestNecessity: 'Mandatory',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(false);
    });

    it('should be true when special request value is entered and value is optional', () => {
      // arrange
      currentState = {
        specialRequest: 'Please be quick.',
        specialRequestNecessity: 'Optional',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(true);
    });

    it('should be true when special request value is empty and value is optional', () => {
      // arrange
      currentState = {
        specialRequest: '',
        specialRequestNecessity: 'Optional',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(true);
    });

    it('should be true when special request value is whitespace and value is optional', () => {
      // arrange
      currentState = {
        specialRequest: '   ',
        specialRequestNecessity: 'Optional',
      };

      // act
      const result = specialRequestValid(currentState);

      // assert
      expect(result).toBe(true);
    });
  });
});
