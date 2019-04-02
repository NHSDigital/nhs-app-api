/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import { $t } from '../../helpers';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_NOT_FOUND } from '../../../../src/lib/routes';

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

describe('HeaderCompanionButton.vue', () => {
  it('will go to "/nominated-pharmacy/not-found" with no nominated pharmacy', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        nominatedPharmacy: {
          pharmacy: {
          },
        },
      },
    };
    createHeaderCompanionButton($store);
    expect(component.vm.activeButton.to).toEqual(NOMINATED_PHARMACY_NOT_FOUND.path);
  });

  it('will go to "/prescriptions/repeat-courses" with nominated pharmacy', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        nominatedPharmacy: {
          pharmacy: {
            pharmacyName: 'Boots',
          },
        },
      },
    };
    createHeaderCompanionButton($store);
    expect(component.vm.activeButton.to).toEqual(PRESCRIPTION_REPEAT_COURSES.path);
  });
});
