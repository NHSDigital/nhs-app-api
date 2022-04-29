import ReferralBookable from '@/components/wayfinder/referrals/ReferralBookableCard';
import { mount } from '../../../helpers';

const mountReferralBookable = ({ propsData = {} }) => mount(
  ReferralBookable,
  {
    propsData,
  },
);

describe('Referral Bookable Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralBookable({
      propsData: {
        bookingReference: '608119956620',
        deepLinkUrl: 'default',
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        requestedSpecialty: 'Cardiology',
        referralId: '1',
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Ready to book');
    });

    it('will display the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(true);
      expect(bookingreferenceTarget.text()).toBe('Cardiology');
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

    it('will display a button', () => {
      const button = wrapper.find('#bookOrManageReferral-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Book or manage this referral');
    });
  });

  describe('Requested specialty is not set', () => {
    const wrapper = mountReferralBookable({
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
      expect(headerTarget.text()).toBe('Ready to book');
    });

    it('will hide the ready to book message', () => {
      const bookingreferenceTarget = wrapper.find('#referral-ready-to-book-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });

    it('will hide the requested specialty', () => {
      const bookingreferenceTarget = wrapper.find('#requested-specialty-1');

      expect(bookingreferenceTarget.exists()).toBe(false);
    });
  });
});
