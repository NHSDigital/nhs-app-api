import ViewDecision from '@/pages/organ-donation/view-decision';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import YourDecision from '@/components/organ-donation/YourDecision';
import { createStore, mount } from '../../helpers';

describe('view decision', () => {
  let $store;
  let $style;
  let state;
  let wrapper;

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(ViewDecision, { $store: store, $style });
  };

  beforeEach(() => {
    $style = {};
    state = {
      organDonation: initialState(),
    };
    wrapper = mountWrapper();
  });

  describe('YourDecision', () => {
    it('will exist', () => {
      expect(wrapper.find(YourDecision).exists()).toBe(true);
    });
  });
});
