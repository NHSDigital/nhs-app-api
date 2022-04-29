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

    it('will display the referred by value', () => {
      const referredByHeaderTarget = wrapper.find('#referred-by-header-1');
      const referredByTextTarget = wrapper.find('#referred-by-text-1');

      expect(referredByHeaderTarget.exists()).toBe(true);
      expect(referredByTextTarget.exists()).toBe(true);

      expect(referredByHeaderTarget.text()).toBe('Referred by:');
      expect(referredByTextTarget.text()).toBe('Mahogany GP Surgery');
    });

    it('will display the review date value', () => {
      const referredByHeaderTarget = wrapper.find('#review-date-header-1');
      const referredByTextTarget = wrapper.find('#review-date-text-1');

      expect(referredByHeaderTarget.exists()).toBe(true);
      expect(referredByTextTarget.exists()).toBe(true);

      expect(referredByHeaderTarget.text()).toBe('Due to be reviewed by:');
      expect(referredByTextTarget.text()).toBe('18 April 2022');
    });

    it('will display the referred date value', () => {
      const referredDateHeaderTarget = wrapper.find('#referral-date-header-1');
      const referredDateTextTarget = wrapper.find('#referral-date-text-1');

      expect(referredDateHeaderTarget.exists()).toBe(true);
      expect(referredDateTextTarget.exists()).toBe(true);

      expect(referredDateHeaderTarget.text()).toBe('Date you were referred:');
      expect(referredDateTextTarget.text()).toBe('10 April 2022');
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
        deepLinkUrl: 'default',
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
