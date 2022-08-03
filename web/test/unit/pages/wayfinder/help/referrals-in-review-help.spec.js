import ReferralsInReviewHelpPage from '@/pages/wayfinder/help/referrals-in-review-help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('Missing, incorrect or cancelled referrals in review link clicked from index page', () => {
  let $store;
  let wrapper;
  let somethingIsMissingTitle;
  let cancellationNotShowingTitle;
  let somethingIsMissingText1;
  let cancellationNotShowingText;
  let backButton;

  const createIndexPage = () => mount(ReferralsInReviewHelpPage, {
    $store,
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
  });

  describe('if page loaded', () => {
    it('something is missing title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      somethingIsMissingTitle = wrapper.find('#somethingIsMissingTitle');

      expect(somethingIsMissingTitle.exists()).toBe(true);
      expect(somethingIsMissingTitle.text()).toEqual('If something is missing');
    });

    it('something is missing text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      somethingIsMissingText1 = wrapper.find('#somethingIsMissingText1');

      expect(somethingIsMissingText1.exists()).toBe(true);
      expect(somethingIsMissingText1.text()).toEqual('You may have referrals being reviewed by a clinic that are not shown but are in other services. Contact the organisation that referred you.');
    });

    it('cancellation not showing title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      cancellationNotShowingTitle = wrapper.find('#cancellationNotShowingTitle');

      expect(cancellationNotShowingTitle.exists()).toBe(true);
      expect(cancellationNotShowingTitle.text()).toEqual('If a cancellation is not showing');
    });

    it('cancellation not showing text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      cancellationNotShowingText = wrapper.find('#cancellationNotShowingText');

      expect(cancellationNotShowingText.exists()).toBe(true);
      expect(cancellationNotShowingText.text()).toEqual('If you have cancelled a referral that’s being reviewed and it’s still showing you need to contact the healthcare provider that referred you.');
    });

    it('back button exists', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
