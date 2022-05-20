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
      expect(headerTarget.text()).toBe('Ready to rebook');
    });

    it('will display the ready to book message with specialty', () => {
      const readyToBookWithSpecialty = wrapper.find('#referral-ready-to-book-1');

      expect(readyToBookWithSpecialty.exists()).toBe(true);
      expect(readyToBookWithSpecialty.text()).toBe('You need to rebook your Cardiology referral appointment as the one you had booked has been cancelled.');
    });

    it('will hide the ready to book message without specialty', () => {
      const readyToBookWithNoSpecialty = wrapper.find('#referral-ready-to-book-no-specialty-1');

      expect(readyToBookWithNoSpecialty.exists()).toBe(false);
    });

    it('will display the requested specialty', () => {
      const requestedSpecialty = wrapper.find('#requested-specialty-1');

      expect(requestedSpecialty.exists()).toBe(true);
      expect(requestedSpecialty.text()).toBe('Cardiology');
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
    const wrapper = mountReferralReadyToRebook({
      propsData: {
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
      expect(headerTarget.text()).toBe('Ready to rebook');
    });

    it('will hide the ready to book message with specialty', () => {
      const readyToBook = wrapper.find('#referral-ready-to-book-1');

      expect(readyToBook.exists()).toBe(false);
    });

    it('will display the ready to book message without specialty', () => {
      const readyToBookWithNoSpecialty = wrapper.find('#referral-ready-to-book-no-specialty-1');

      expect(readyToBookWithNoSpecialty.exists()).toBe(true);
      expect(readyToBookWithNoSpecialty.text()).toBe('You need to rebook your referral appointment as the one you had booked has been cancelled.');
    });

    it('will hide the requested specialty', () => {
      const requestedSpecialty = wrapper.find('#requested-specialty-1');

      expect(requestedSpecialty.exists()).toBe(false);
    });
  });
});
