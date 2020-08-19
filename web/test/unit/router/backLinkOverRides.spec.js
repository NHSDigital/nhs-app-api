import each from 'jest-each';
import BackLinkOverRides from '@/router/backLinkOverRides';
import {
  ACCOUNT_COOKIES_NAME,
  APPOINTMENT_BOOKING_SUCCESS_NAME,
  ORGAN_DONATION_NAME,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  APPOINTMENT_CANCELLING_SUCCESS_NAME,
  ORGAN_DONATION_VIEW_DECISION_NAME,
  SWITCH_PROFILE_NAME,
  GP_MESSAGES_NAME,
  PRESCRIPTION_REPEAT_COURSES_NAME,
  PRESCRIPTIONS_VIEW_ORDERS_NAME,
} from '@/router/names';
import {
  ACCOUNT_PATH,
  APPOINTMENTS_PATH,
  MORE_PATH, MESSAGES_PATH,
  INDEX_PATH,
  PRESCRIPTIONS_PATH,
} from '@/router/paths';


describe('backLinkOverrides', () => {
  each([
    [ACCOUNT_COOKIES_NAME, ACCOUNT_PATH, true],
    [APPOINTMENT_BOOKING_SUCCESS_NAME, APPOINTMENTS_PATH, true],
    [APPOINTMENT_CANCELLING_SUCCESS_NAME, APPOINTMENTS_PATH, true],
    [LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME, APPOINTMENTS_PATH, true],
    [LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_NAME, INDEX_PATH, true],
    [PRESCRIPTION_REPEAT_COURSES_NAME, PRESCRIPTIONS_PATH, true],
    [PRESCRIPTIONS_VIEW_ORDERS_NAME, PRESCRIPTIONS_PATH, true],
    [ORGAN_DONATION_NAME, MORE_PATH, undefined],
    [ORGAN_DONATION_VIEW_DECISION_NAME, MORE_PATH, undefined],
    [GP_MESSAGES_NAME, MESSAGES_PATH, true],
    [SWITCH_PROFILE_NAME, INDEX_PATH, true]])
    .it('will go to $path from $name by default', (name, path, ignoreStore) => {
      Object.keys(BackLinkOverRides).forEach(() => {
        expect(BackLinkOverRides[name].defaultPath).toBe(path);
        expect(BackLinkOverRides[name].ignoreStore).toBe(ignoreStore);
      });
    });

  it('will have a default path in each overriding configuration', () => {
    Object.keys(BackLinkOverRides).forEach((key) => {
      expect(BackLinkOverRides[key].defaultPath).toBeDefined();
    });
  });
});
