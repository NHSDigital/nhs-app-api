import { parseSelectedRepeatCourses } from '@/lib/noJs';
import { INDEX, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import { initialState as repeatPrescriptionCoursesInitialState } from '@/store/modules/repeatPrescriptionCourses/mutation-types';

const getDefaultExpectedParseResult = () => ({
  shouldRedirect: true,
  redirectPath: INDEX.path,
  redirectQuery: {},
  state: undefined,
});

describe('noJs lib', () => {
  describe('parse form data to state', () => {
    describe('parseSelectedRepeatCourses', () => {
      let expectedResult;
      beforeEach(() => {
        expectedResult = getDefaultExpectedParseResult();
        expectedResult.redirectPath = PRESCRIPTION_REPEAT_COURSES.path;
      });

      it('will return default result if data is an empty object', () => {
        const data = {};

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });

      it('will return default result if data is undefined', () => {
        const data = undefined;

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });

      it('will return result with redirect for none selected if no prescriptions are selected', () => {
        const data = {
          prescription: undefined,
        };
        expectedResult.redirectQuery = {
          ...expectedResult.redirectQuery,
          noneSelected: 1,
        };

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });

      it('will return result with redirect for missing special request if no prescriptions are selected', () => {
        const data = {
          prescription: 'test-prescription-id',
          'test-prescription-id': '{ "name": "name", "details": "details" }',
          specialRequestNecessity: 'Mandatory',
          specialRequest: undefined,
        };
        expectedResult.redirectQuery = {
          ...expectedResult.redirectQuery,
          missingSpecialRequest: 1,
        };

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });

      it('will return successful result with parsed repeat prescription state and redirect false when data.prescription is a string', () => {
        const data = {
          prescription: 'test-prescription-id',
          'test-prescription-id': '{ "name": "name", "details": "details" }',
          specialRequestNecessity: 'Optional',
          specialRequest: 'Special request',
        };
        const state = {
          repeatPrescriptionCourses: {
            ...repeatPrescriptionCoursesInitialState(),
            ...{
              repeatPrescriptionCourses: [{
                id: 'test-prescription-id',
                name: 'name',
                details: 'details',
                selected: true,
              }],
              specialRequestNecessity: 'Optional',
              specialRequest: 'Special request',
            },
          },
        };
        expectedResult = {
          ...expectedResult,
          shouldRedirect: false,
          state,
        };

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });

      it('will return successful result with parsed repeat prescription state and redirect false when data.prescription is an array', () => {
        const data = {
          prescription: [
            'test-prescription-id-one',
            'test-prescription-id-two',
          ],
          'test-prescription-id-one': '{ "name": "one", "details": "details" }',
          'test-prescription-id-two': '{ "name": "two", "details": "details" }',
          specialRequestNecessity: 'Mandatory',
          specialRequest: 'Special request',
        };
        const state = {
          repeatPrescriptionCourses: {
            ...repeatPrescriptionCoursesInitialState(),
            ...{
              repeatPrescriptionCourses: [{
                id: 'test-prescription-id-one',
                name: 'one',
                details: 'details',
                selected: true,
              }, {
                id: 'test-prescription-id-two',
                name: 'two',
                details: 'details',
                selected: true,
              }],
              specialRequestNecessity: 'Mandatory',
              specialRequest: 'Special request',
            },
          },
        };
        expectedResult = {
          ...expectedResult,
          shouldRedirect: false,
          state,
        };

        const actualResult = parseSelectedRepeatCourses({ data });

        expect(actualResult).toMatchObject(expectedResult);
      });
    });
  });
});
