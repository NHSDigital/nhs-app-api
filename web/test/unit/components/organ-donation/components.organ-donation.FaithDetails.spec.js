import FaithDetails from '@/components/organ-donation/FaithDetails';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('faith details', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });

    wrapper = mount(FaithDetails, {
      $store,
      propsData: { declaration: 'value' },
    });
  });

  describe('declaration text', () => {
    let declarationText;

    beforeEach(() => {
      declarationText = wrapper.find('span');
    });

    it('will exist', () => {
      expect(declarationText.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(declarationText.text()).toContain('translate_organDonation.reviewYourDecision.faith.declaration.value');
    });
  });
});
