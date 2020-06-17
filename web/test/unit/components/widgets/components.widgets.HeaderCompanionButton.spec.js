/* eslint-disable import/no-extraneous-dependencies */
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import { PRESCRIPTIONS_NAME } from '@/router/names';
import { NOMINATED_PHARMACY_CHECK_PATH, PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';
import { createStore, mount } from '../../helpers';

const createHeaderCompanionButton = isValid => mount(HeaderCompanionButton, {
  $store: createStore({
    state: {
      device: {
        source: 'web',
      },
      errors: {
        hasConnectionProblem: false,
      },
    },
    getters: {
      'nominatedPharmacy/nominatedPharmacyEnabled': isValid,
      'serviceJourneyRules/nominatedPharmacyEnabled': isValid,
    },
  }),
  $route: {
    name: PRESCRIPTIONS_NAME,
  },
  $style: {
    headerCompanion: 'headerCompanion',
  },
});

describe('HeaderCompanionButton.vue', () => {
  it('will go to nominated pharmacy check when nominated pharmacy is enabled', () => {
    expect(createHeaderCompanionButton(true).vm.path).toEqual(NOMINATED_PHARMACY_CHECK_PATH);
  });

  it('will go to prescriptions repeat courses when nominated pharmacy is not enabled', () => {
    expect(createHeaderCompanionButton(false).vm.path).toEqual(PRESCRIPTION_REPEAT_COURSES_PATH);
  });
});
