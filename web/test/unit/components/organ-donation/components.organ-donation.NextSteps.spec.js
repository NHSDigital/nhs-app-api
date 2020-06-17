import NextSteps from '@/components/organ-donation/NextSteps';
import {
  ORGAN_DONATION_SHARE_DECISION_URL,
  ORGAN_DONATION_TELL_FAMILY_URL,
} from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

describe('next steps component', () => {
  let wrapper;
  let $store;

  describe('opt in', () => {
    beforeEach(() => {
      $store = createStore();
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
        expect(shareDecisionLink.attributes().href).toEqual(ORGAN_DONATION_SHARE_DECISION_URL);
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
        expect(tellFamilyLink.attributes().href).toEqual(ORGAN_DONATION_TELL_FAMILY_URL);
      });
    });
  });

  describe('opt out', () => {
    beforeEach(() => {
      $store = createStore();
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
        expect(tellFamilyLink.attributes().href).toEqual(ORGAN_DONATION_TELL_FAMILY_URL);
      });
    });
  });
});
