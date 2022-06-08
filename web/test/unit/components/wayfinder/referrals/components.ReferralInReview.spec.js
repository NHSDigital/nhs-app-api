import ReferralInReview from '@/components/wayfinder/referrals/ReferralInReviewCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    onClick: jest.fn(),
  },
}));

const mountReferralInReview = ({ propsData = {} }) => mount(
  ReferralInReview,
  { propsData },
);

describe('Referral In Review Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralInReview({
      propsData: {
        deepLinkUrl: 'default',
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        reviewDate: '2022-04-18T10:00:00',
        requestedSpecialty: 'Cardiology',
        referralId: '1',
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Referral request in review');
    });

    it('will display the requested specialty', () => {
      const requestedSpecialtyTarget = wrapper.find('#requested-specialty-1');

      expect(requestedSpecialtyTarget.exists()).toBe(true);
      expect(requestedSpecialtyTarget.text()).toBe('Cardiology');
    });

    it('will display the requested specialty message', () => {
      const requestedSpecialtyTarget = wrapper.find('#healthcare-requested-specialty-1');

      expect(requestedSpecialtyTarget.exists()).toBe(true);
      expect(requestedSpecialtyTarget.text()).toBe('Your healthcare provider has requested for you to be referred to Cardiology. This request is being reviewed. You do not need to do anything.');
    });

    it('will display the referred by value', () => {
      const referredByHeaderTarget = wrapper.find('#referred-by-header-1');
      const referredByTextTarget = wrapper.find('#referred-by-text-1');

      expect(referredByHeaderTarget.exists()).toBe(true);
      expect(referredByTextTarget.exists()).toBe(true);

      expect(referredByHeaderTarget.text()).toBe('Referred by:');
      expect(referredByTextTarget.text()).toBe('Mahogany GP Surgery');
    });

    it('will display the referred date value', () => {
      const referredDateHeaderTarget = wrapper.find('#referral-date-header-1');
      const referredDateTextTarget = wrapper.find('#referral-date-text-1');

      expect(referredDateHeaderTarget.exists()).toBe(true);
      expect(referredDateTextTarget.exists()).toBe(true);

      expect(referredDateHeaderTarget.text()).toBe('Date you were referred:');
      expect(referredDateTextTarget.text()).toBe('10 April 2022');
    });

    it('will display the review date value', () => {
      const reviewDateHeaderTarget = wrapper.find('#review-date-header-1');
      const reviewDateTextTarget = wrapper.find('#review-date-text-1');

      expect(reviewDateHeaderTarget.exists()).toBe(true);
      expect(reviewDateTextTarget.exists()).toBe(true);

      expect(reviewDateHeaderTarget.text()).toBe('Due to be reviewed by:');
      expect(reviewDateTextTarget.text()).toBe('18 April 2022');
    });

    it('will hide the no requested specialty message', () => {
      const noRequestedSpecialtyTarget = wrapper.find('#no-requested-specialty-1');

      expect(noRequestedSpecialtyTarget.exists()).toBe(false);
    });

    it('will display a deep link', () => {
      const deepLink = wrapper.find('#manageInReviewReferral-1');

      expect(deepLink.exists()).toBe(true);
      expect(deepLink.text()).toBe('View or manage this referral');
    });
  });

  describe('Requested specialty is empty', () => {
    const wrapper = mountReferralInReview({
      propsData: {
        deepLinkUrl: 'default',
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        reviewDate: '2022-04-18T10:00:00',
        requestedSpecialty: null,
        referralId: '1',
      },
    });

    it('will hide the requested specialty value', () => {
      const noRequestedSpecialtyTarget = wrapper.find('#requested-specialty-1');

      expect(noRequestedSpecialtyTarget.exists()).toBe(false);
    });

    it('will hide the healthcare requested specialty message', () => {
      const noRequestedSpecialtyTarget = wrapper.find('#healthcare-requested-specialty-1');

      expect(noRequestedSpecialtyTarget.exists()).toBe(false);
    });

    it('will display the no requested specialty message', () => {
      const noRequestedSpecialtyTarget = wrapper.find('#no-requested-specialty-1');

      expect(noRequestedSpecialtyTarget.exists()).toBe(true);
      expect(noRequestedSpecialtyTarget.text()).toBe('Your healthcare provider has requested for you to be referred. This request is being reviewed. You do not need to do anything.');
    });

    it('will display a deep link', () => {
      const deepLink = wrapper.find('#manageInReviewReferral-1');

      expect(deepLink.exists()).toBe(true);
      expect(deepLink.text()).toBe('View or manage this referral');
    });

    it('will call onClick when the view or manage link is clicked', () => {
      const deepLink = wrapper.find('#manageInReviewReferral-1 a');

      deepLink.trigger('click');

      expect(RedirectorMixin.methods.onClick).toHaveBeenCalled();
    });
  });
});
