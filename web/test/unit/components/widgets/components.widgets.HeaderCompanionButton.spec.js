/* eslint-disable import/no-extraneous-dependencies */
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import { createStore, mount } from '../../helpers';
import { PRESCRIPTIONS, NOMINATED_PHARMACY_CHECK, PRESCRIPTION_REPEAT_COURSES } from '../../../../src/lib/routes';

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
    name: PRESCRIPTIONS.name,
  },
  $style: {
    headerCompanion: 'headerCompanion',
  },
});

describe('HeaderCompanionButton.vue', () => {
  it('will go to nominated pharmacy check when nominated pharmacy is enabled', () => {
    expect(createHeaderCompanionButton(true).vm.path).toEqual(NOMINATED_PHARMACY_CHECK.path);
  });

  it('will go to prescriptions repeat courses when nominated pharmacy is not enabled', () => {
    expect(createHeaderCompanionButton(false).vm.path).toEqual(PRESCRIPTION_REPEAT_COURSES.path);
  });
});
