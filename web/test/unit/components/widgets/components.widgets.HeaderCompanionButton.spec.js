/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import { $t, createStore as newStore } from '../../helpers';
import { PRESCRIPTIONS, NOMINATED_PHARMACY_CHECK, PRESCRIPTION_REPEAT_COURSES } from '../../../../src/lib/routes';

let component;

const createHeaderCompanionButton = ($store) => {
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const $route = {
    name: PRESCRIPTIONS.name,
  };

  const $style = {
    headerCompanion: 'headerCompanion',
  };

  component = mount(HeaderCompanionButton, {
    localVue,
    mocks: {
      $store,
      $route,
      $style,
      $t,
      showTemplate: () => true,
    },
  });
};

const createState = isValid => ({
  device: {
    source: 'web',
  },
  nominatedPharmacy: {
    pharmacy: {
      pharmacyName: undefined,
    },
    nominatedPharmacyEnabled: isValid,
  },
});

describe('HeaderCompanionButton.vue', () => {
  it('will go to "/nominated-pharmacy/check" with no nominated pharmacy', () => {
    const $store = newStore(
      { state: createState(true) },
    );
    createHeaderCompanionButton($store);
    expect(component.vm.path).toEqual(NOMINATED_PHARMACY_CHECK.path);
  });

  it('will go to "/nominated-pharmacy/check" with nominated pharmacy', () => {
    const $store = newStore(
      { state: createState(true) },
    );
    createHeaderCompanionButton($store);
    expect(component.vm.path).toEqual(NOMINATED_PHARMACY_CHECK.path);
  });

  it('will go to "/prescriptions/repeat-courses" when nominated pharmacy should not be shown', () => {
    const $store = newStore(
      { state: createState(false) },
    );
    createHeaderCompanionButton($store);
    expect(component.vm.path).toEqual(PRESCRIPTION_REPEAT_COURSES.path);
  });
});
