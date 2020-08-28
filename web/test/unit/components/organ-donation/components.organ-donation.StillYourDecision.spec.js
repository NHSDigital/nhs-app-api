import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import i18n from '@/plugins/i18n';
import ReaffirmDecisionLink from '@/components/organ-donation/ReaffirmDecisionLink';
import StillYourDecision from '@/components/organ-donation/StillYourDecision';
import { mount } from '../../helpers';

describe('organ donation still your decision', () => {
  let wrapper;
  let propsData;

  const createPropsData = ({
    isSomeOrgans = false,
    showAmend = false,
    showReaffirm = false,
  } = {}) => ({
    isSomeOrgans,
    showAmend,
    showReaffirm,
  });

  const mountStillYourDecision = () => mount(StillYourDecision, { propsData, mountOpts: { i18n } });

  describe('amend', () => {
    it('will render the AmendDecision link when `showAmend` is true', () => {
      propsData = createPropsData({ showAmend: true });
      wrapper = mountStillYourDecision();
      expect(wrapper.find(AmendDecisionLink).exists()).toBe(true);
    });

    it('will not display the AmendDecision link when `showAmend` false', () => {
      propsData = createPropsData({ showAmend: false });
      wrapper = mountStillYourDecision();
      expect(wrapper.find(AmendDecisionLink).exists()).toBe(false);
    });
  });

  describe('preamble', () => {
    beforeEach(() => {
      propsData = createPropsData();
      wrapper = mountStillYourDecision();
    });

    it('will display the organ donation subheader', () => {
      expect(wrapper.find('h3').text()).toEqual('Is this still your decision?');
    });

    it('will display the organ donation paragraph', () => {
      expect(wrapper.find('p').text()).toEqual('Keeping your registration up to date will help your family, should organ donation be possible.');
    });
  });

  describe('reaffirm', () => {
    it('will display the ReaffirmDecision link when `showReaffirm` is true', () => {
      propsData = createPropsData({ showReaffirm: true });
      wrapper = mountStillYourDecision();
      expect(wrapper.find(ReaffirmDecisionLink).exists()).toBe(true);
    });

    it('will not display the ReaffirmDecision link when false', () => {
      propsData = createPropsData({ showReaffirm: false });
      wrapper = mountStillYourDecision();
      expect(wrapper.find(ReaffirmDecisionLink).exists()).toBe(false);
    });

    it('will pass the value of `isSomeOrgans` to ReaffirmDecision', () => {
      propsData = createPropsData({ showReaffirm: true, isSomeOrgans: true });
      wrapper = mountStillYourDecision();
      expect(wrapper.find(ReaffirmDecisionLink).props().isSomeOrgans).toBe(true);
    });
  });
});
