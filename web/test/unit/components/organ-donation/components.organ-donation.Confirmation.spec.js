import Confirmation from '@/components/organ-donation/Confirmation';
import { createStore, mount } from '../../helpers';

describe('confirmation', () => {
  let $store;
  let wrapper;

  const mountConfirmation = ({ submitAttempted = false } = {}) => mount(Confirmation, {
    $store,
    propsData: {
      submitAttempted,
    },
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: {
          isAccuracyAccepted: false,
          isPrivacyAccepted: false,
        },
        device: {
          isNativeApp: false,
        },
      },
    });
    wrapper = mountConfirmation();
  });

  describe('isAccuracyAccepted setter', () => {
    it('will dispatch `setAccuracyAcceptance`', () => {
      wrapper.vm.isAccuracyAccepted = true;
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setAccuracyAcceptance', true);
    });
  });

  describe('isPrivacyAccepted setter', () => {
    it('will dispatch `setPrivacyAcceptance`', () => {
      wrapper.vm.isPrivacyAccepted = false;
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setPrivacyAcceptance', false);
    });
  });

  describe('Confirmation checkbox rendering', () => {
    it('will verify an associated label for the confirmation on your decision checkbox', () => {
      expect(wrapper.find("input[type='checkbox'][id='accuracy-checkbox']")
        .exists()).toEqual(true);

      expect(wrapper.find("label[for='accuracy-checkbox']")
        .exists()).toEqual(true);
    });

    it('will verify an associated label for the privacy checkbox', () => {
      expect(wrapper.find("input[type='checkbox'][id='privacy-checkbox']")
        .exists()).toEqual(true);

      expect(wrapper.find("label[for='privacy-checkbox']")
        .exists()).toEqual(true);
    });
  });

  describe('privacy link', () => {
    const URL_EXTERNAL = 'www.foo.com';
    let link;

    beforeEach(() => {
      $store = createStore({
        state: {
          organDonation: {
            isAccuracyAccepted: false,
            isPrivacyAccepted: false,
          },
          device: {
            isNativeApp: false,
          },
        },
        $env: {
          ORGAN_DONATION_PRIVACY_URL: URL_EXTERNAL,
        },
      });
      wrapper = mountConfirmation();
      link = wrapper.find('a');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have the external privacy link, with target set', () => {
      expect(link.attributes().target).toEqual('_blank');
      expect(link.attributes().href).toEqual(URL_EXTERNAL);
    });
  });

  describe('inline errors', () => {
    const errorMessage = 'translate_organDonation.reviewYourDecision.confirmation.errors.';
    const showOrNot = bool => (bool ? 'show' : 'not show');

    const assertInlineMessage = ({ accuracy = false, privacy = false }, wrapperFn) => {
      it(`will ${showOrNot(accuracy)} inline error for accuracy`, () => {
        const span = wrapperFn().find('#accuracy-checkbox-error span');
        expect(span.exists()).toBe(accuracy);
        if (accuracy) {
          expect(span.text()).toBe(`${errorMessage}accuracy`);
        }
      });

      it(`will ${showOrNot(privacy)} inline error for privacy`, () => {
        const span = wrapperFn().find('#privacy-checkbox-error span');
        expect(span.exists()).toBe(privacy);
        if (privacy) {
          expect(span.text()).toBe(`${errorMessage}privacy`);
        }
      });
    };

    beforeEach(() => {
      wrapper = mountConfirmation({ submitAttempted: true });
    });

    describe('when both are not accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = false;
        $store.state.organDonation.isPrivacyAccepted = false;
      });

      assertInlineMessage({ accuracy: true, privacy: true }, () => wrapper);
    });

    describe('when privacy is not accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = true;
        $store.state.organDonation.isPrivacyAccepted = false;
      });

      assertInlineMessage({ accuracy: false, privacy: true }, () => wrapper);
    });

    describe('when accuracy is not accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = false;
        $store.state.organDonation.isPrivacyAccepted = true;
      });

      assertInlineMessage({ accuracy: true, privacy: false }, () => wrapper);
    });

    describe('when both are accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = true;
        $store.state.organDonation.isPrivacyAccepted = true;
      });

      assertInlineMessage({ accuracy: false, privacy: false }, () => wrapper);
    });
  });
});
