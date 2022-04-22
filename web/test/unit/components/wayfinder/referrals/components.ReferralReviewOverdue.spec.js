import ReferralReviewOverdue from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import { mount } from '../../../helpers';

const mountReferralReviewOverdue = ({ propsData = {} }) => mount(
  ReferralReviewOverdue,
  {
    propsData,
  },
);

describe('Referral Ready Overdue Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralReviewOverdue({
      propsData: {
        bookingReference: '608119956620',
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
      expect(headerTarget.text()).toBe('Review by clinic is overdue');
    });

    it('will display the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('Cardiology');
    });

    it('will display the contact message with requested specialty ', () => {
      const bookingreferenceTarget = wrapper.find('#contact-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('You need to contact Cardiology as a review of this referral is overdue.');
    });

    it('will display the booking reference', () => {
      const bookingreferenceTarget = wrapper.find('#booking-reference-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('Booking reference: 608119956620');
    });

    it('will display the referred by value', () => {
      const referredByTarget = wrapper.find('#referred-by-1');

      expect(referredByTarget.exists()).toBe(true);
      expect(referredByTarget.text()).toBe('Referred by: Mahogany GP Surgery');
    });

    it('will display the review date value', () => {
      const referredByTarget = wrapper.find('#review-date-1');

      expect(referredByTarget.exists()).toBe(true);
      expect(referredByTarget.text()).toBe('Due to be reviewed by: 18 April 2022');
    });

    it('will display the referred date value', () => {
      const referredDateTarget = wrapper.find('#referral-date-1');

      expect(referredDateTarget.exists()).toBe(true);
      expect(referredDateTarget.text()).toBe('Date you were referred: 10 April 2022');
    });

    it('will display a button', () => {
      const button = wrapper.find('#manageInReviewReferral-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Contact the clinic');
    });
  });

  describe('Requested specialty is not set', () => {
    const wrapper = mountReferralReviewOverdue({
      propsData: {
        bookingReference: '608119956620',
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        requestedSpecialty: null,
        referralId: '1',
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Review by clinic is overdue');
    });

    it('will hide the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });

    it('will hide the contact message with requested specialty ', () => {
      const bookingreferenceTarget = wrapper.find('#contact-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });
  });
});
