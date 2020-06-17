import * as dependency from '@/lib/utils';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { NOMINATED_PHARMACY_INTERRUPT_PATH } from '@/router/paths';
import { create$T, createStore, createRouter, mount } from '../../helpers';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $style;
  let $router;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  const mountPage = () => mount(NoNominatedPharmacyWarning, { $store, $style, $t, $router });

  describe('warning', () => {
    let warningText;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $router = createRouter();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      warningText = wrapper.find('#warning-text');
    });

    it('will exist', () => {
      expect(warningText.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.warningText" for text', () => {
      expect(warningText.text())
        .toEqual('translate_nominatedPharmacyNotFound.warningText');
    });
  });

  describe('instruction', () => {
    let instruction;
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      instruction = wrapper.find('#instruction');
    });

    it('will exist', () => {
      expect(instruction.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.line" for text', () => {
      expect(instruction.text())
        .toEqual('translate_nominatedPharmacyNotFound.line');
    });
  });

  describe('link-to-add-nominated-pharmacy', () => {
    let link;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        link: 'link',
      };
      $router = createRouter();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      link = wrapper.find('#link-to-nominate-pharmacy');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.nominatedPharmacyLink" for text', () => {
      expect(link.text())
        .toEqual('translate_nominatedPharmacyNotFound.nominatedPharmacyLink');
    });

    it('will redirect to interrupt nominated pharmacy page', async () => {
      dependency.redirectTo = jest.fn();
      const currentPath = '/nominated-pharmacy';
      $router.currentRoute = {
        path: currentPath,
      };
      await link.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT_PATH);
    });
  });
});
