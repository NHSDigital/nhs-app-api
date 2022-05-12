import ReferralInReview from '@/components/wayfinder/referrals/ReferralInReviewCard';
import { mount } from '../../../helpers';

const mountReferralInReview = ({ propsData = {} }) => mount(
  ReferralInReview,
  {
    propsData,
  },
);

describe('Referral In Review Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralInReview({
      propsData: {
        bookingReference: '6081 1995 6620',
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
      expect(headerTarget.text()).toBe('Your referral request is being reviewed');
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

    it('will display the booking reference', () => {
      const bookingreferenceTarget = wrapper.find('#booking-reference-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('Booking reference: 6081 1995 6620');
    });

    it('will display the referred by value', () => {
      const referredByTarget = wrapper.find('#referred-by-1');

      expect(referredByTarget.exists()).toBe(true);
      expect(referredByTarget.text()).toBe('Referred by: Mahogany GP Surgery');
    });

    it('will display the referred date value', () => {
      const referredDateTarget = wrapper.find('#referral-date-1');

      expect(referredDateTarget.exists()).toBe(true);
      expect(referredDateTarget.text()).toBe('Date you were referred: 10 April 2022');
    });

    it('will display the review date value', () => {
      const reviewDateTarget = wrapper.find('#review-date-1');

      expect(reviewDateTarget.exists()).toBe(true);
      expect(reviewDateTarget.text()).toBe('Due to be reviewed by: 18 April 2022');
    });

    it('will hide the no requested specialty message', () => {
      const noRequestedSpecialtyTarget = wrapper.find('#no-requested-specialty-1');

      expect(noRequestedSpecialtyTarget.exists()).toBe(false);
    });

    it('will display a button', () => {
      const button = wrapper.find('#manageInReviewReferral-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Manage this referral');
    });
  });

  describe('Requested specialty is empty', () => {
    const wrapper = mountReferralInReview({
      propsData: {
        bookingReference: '6081 1995 6620',
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

    it('will display a button', () => {
      const button = wrapper.find('#manageInReviewReferral-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Manage this referral');
    });
  });
});
