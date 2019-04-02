import NextSteps from '@/components/organ-donation/NextSteps';
import { createStore, mount } from '../../helpers';

describe('next steps component', () => {
  let wrapper;
  let $store;
  const TELL_FAMILY_URL = 'www.foo.com';

  describe('opt in', () => {
    const SHARE_DECISION_URL = 'www.boo.com';
    beforeEach(() => {
      $store = createStore({
        $env: {
          ORGAN_DONATION_SHARE_DECISION_URL: SHARE_DECISION_URL,
          ORGAN_DONATION_TELL_FAMILY_URL: TELL_FAMILY_URL,
        },
      });
      wrapper = mount(NextSteps, {
        $store,
        propsData: { isOptInDecision: true },
      });
    });

    describe('share decision link', () => {
      let shareDecisionLink;

      beforeEach(() => {
        shareDecisionLink = wrapper.find('#btn_shareDecision');
      });

      it('will exist', () => {
        expect(shareDecisionLink.exists()).toBe(true);
      });

      it('will have target set to blank', () => {
        expect(shareDecisionLink.attributes().target).toEqual('_blank');
      });

      it('will go to share decision external url', () => {
        expect(shareDecisionLink.attributes().href).toEqual(SHARE_DECISION_URL);
      });
    });

    describe('tell family link', () => {
      let tellFamilyLink;

      beforeEach(() => {
        tellFamilyLink = wrapper.find('#btn_tellFamily');
      });

      it('will exist', () => {
        expect(tellFamilyLink.exists()).toBe(true);
      });

      it('will have target set to blank', () => {
        expect(tellFamilyLink.attributes().target).toEqual('_blank');
      });

      it('will go to share decision external url', () => {
        expect(tellFamilyLink.attributes().href).toEqual(TELL_FAMILY_URL);
      });
    });
  });

  describe('opt out', () => {
    beforeEach(() => {
      $store = createStore({
        $env: {
          ORGAN_DONATION_TELL_FAMILY_URL: TELL_FAMILY_URL,
        },
      });
      wrapper = mount(NextSteps, {
        $store,
        propsData: { isOptInDecision: false },
      });
    });

    it('share decision link will not exist', () => {
      expect(wrapper.find('#btn_shareDecision').exists()).toBe(false);
    });

    describe('tell family link', () => {
      let tellFamilyLink;

      beforeEach(() => {
        tellFamilyLink = wrapper.find('#btn_tellFamily');
      });

      it('will exist', () => {
        expect(tellFamilyLink.exists()).toBe(true);
      });

      it('will have target set to blank', () => {
        expect(tellFamilyLink.attributes().target).toEqual('_blank');
      });

      it('will go to share decision external url', () => {
        expect(tellFamilyLink.attributes().href).toEqual(TELL_FAMILY_URL);
      });
    });
  });
});
