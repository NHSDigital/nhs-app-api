import ReferralReadyToRebook from '@/components/wayfinder/referrals/ReferralReadyToRebookCard';
import { mount } from '../../../helpers';

const mountReferralReadyToRebook = ({ propsData = {} }) => mount(
  ReferralReadyToRebook,
  {
    propsData,
  },
);

describe('Referral Ready To Rebook Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralReadyToRebook({
      propsData: {
        bookingReference: '608119956620',
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        requestedSpecialty: 'Cardiology',
        referralId: '1',
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Ready to rebook');
    });

    it('will display the ready to book message with specialty', () => {
      const bookingreferenceTarget = wrapper.find('#referral-ready-to-book-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('You need to rebook your Cardiology referral appointment as the one you had booked has been cancelled.');
    });

    it('will hide the ready to book message without specialty', () => {
      const bookingreferenceTarget = wrapper.find('#referral-ready-to-book-no-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });

    it('will display the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('Cardiology');
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

    it('will display the referred date value', () => {
      const referredDateTarget = wrapper.find('#referral-date-1');

      expect(referredDateTarget.exists()).toBe(true);
      expect(referredDateTarget.text()).toBe('Date you were referred: 10 April 2022');
    });

    it('will display a button', () => {
      const button = wrapper.find('#bookOrManageReferral-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Book or manage this referral');
    });
  });

  describe('Requested specialty is not set', () => {
    const wrapper = mountReferralReadyToRebook({
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
      expect(headerTarget.text()).toBe('Ready to rebook');
    });

    it('will hide the ready to book message with specialty', () => {
      const bookingreferenceTarget = wrapper.find('#referral-ready-to-book-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });

    it('will display the ready to book message without specialty', () => {
      const bookingreferenceTarget = wrapper.find('#referral-ready-to-book-no-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('You need to rebook your referral appointment as the one you had booked has been cancelled.');
    });

    it('will hide the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });
  });
});
