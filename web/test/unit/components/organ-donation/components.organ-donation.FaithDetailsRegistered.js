import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('faith details registered', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });

    wrapper = mount(FaithDetailsRegistered, {
      $store,
      propsData: { declaration: 'value' },
    });
  });

  describe('declaration text', () => {
    let declarationText;

    beforeEach(() => {
      declarationText = wrapper.find('p');
    });

    it('will exist', () => {
      expect(declarationText.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(declarationText.text()).toContain('translate_organDonation.registered.faithAndBeliefs.declaration.value');
    });
  });
});
