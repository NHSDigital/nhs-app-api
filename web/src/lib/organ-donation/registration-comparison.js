import get from 'lodash/fp/get';
import isEqual from 'lodash/fp/isEqual';
import startsWith from 'lodash/fp/startsWith';
import { initialState } from '@/store/modules/organDonation/mutation-types';

const ORGAN_DONATION_PREFIX = 'organDonation.';
const defaultState = initialState();

const startsWithOrganDonation = startsWith(ORGAN_DONATION_PREFIX);

// eslint-disable-next-line import/prefer-default-export
export const isDefault = ({ path, state }) => {
  const fixedState = startsWithOrganDonation(path) ? { organDonation: defaultState } : defaultState;
  const getValue = get(path);
  return isEqual(getValue(state), getValue(fixedState));
};
