import ReviewYourDecision from '@/pages/organ-donation/review-your-decision';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

describe('review your decision', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = () => {
    const state = {
      organDonation: initialState(),
    };

    return state;
  };

  const mountPage = () => mount(ReviewYourDecision, { $store, $style, $t });

  beforeEach(() => {
    $store = createStore({ state: createState() });
    $style = {};
    wrapper = mountPage();
  });

  describe('back button', () => {
    let backButton;

    beforeEach(() => {
      backButton = wrapper.find('#back-button');
    });

    it('will exist', () => {
      expect(backButton.exists()).toBe(true);
    });
  });
});
