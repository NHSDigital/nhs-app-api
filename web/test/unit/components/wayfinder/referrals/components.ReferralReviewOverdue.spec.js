import ReferralReviewOverdue from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountReferralReviewOverdue = ({ propsData = {} }) => mount(
  ReferralReviewOverdue,
  { propsData },
);

describe('Referral Ready Overdue Card', () => {
  describe('Requested specialty is set', () => {
    const wrapper = mountReferralReviewOverdue({
      propsData: {
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        reviewDate: '2022-04-18T10:00:00',
        requestedSpecialty: 'Cardiology',
        referralId: '1',
        deepLinkUrl,
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Review by clinic overdue');
    });

    it('will display the requested specialty', () => {
      const requestedSpecialty = wrapper.find('#requested-specialty-1');

      expect(requestedSpecialty.exists()).toBe(true);
      expect(requestedSpecialty.text()).toBe('Cardiology');
    });

    it('will display the contact message with requested specialty ', () => {
      const contactWithSpecialty = wrapper.find('#contact-specialty-1');

      expect(contactWithSpecialty.exists()).toBe(true);
      expect(contactWithSpecialty.text()).toBe('Your healthcare provider has requested for you to be referred to Cardiology. A review of this request is overdue. You need to contact the clinic.');
    });

    it('will hide the contact message without requested specialty ', () => {
      const contactWithNoSpecialty = wrapper.find('#contact-no-specialty-1');

      expect(contactWithNoSpecialty.exists()).toBe(false);
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

    it('will call goToUrlViaRedirector when the contact the clinic button is clicked', () => {
      const button = wrapper.find('#manageInReviewReferral-1');

      button.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Requested specialty is not set', () => {
    const wrapper = mountReferralReviewOverdue({
      propsData: {
        referredBy: 'Mahogany GP Surgery',
        referredDate: '2022-04-10T10:00:00',
        requestedSpecialty: null,
        referralId: '1',
        deepLinkUrl,
      },
    });

    it('will display an h3 header', () => {
      const headerTarget = wrapper.find('h3');

      expect(headerTarget.exists()).toBe(true);
      expect(headerTarget.text()).toBe('Review by clinic overdue');
    });

    it('will hide the requested specialty', () => {
      const requestedSpecialty = wrapper.find('#requested-specialty-1');

      expect(requestedSpecialty.exists()).toBe(false);
    });

    it('will display the contact message without requested specialty ', () => {
      const contactWithNoSpecialty = wrapper.find('#contact-no-specialty-1');

      expect(contactWithNoSpecialty.exists()).toBe(true);
      expect(contactWithNoSpecialty.text()).toBe('Your healthcare provider has requested for you to be referred. A review of this request is overdue. You need to contact the clinic.');
    });

    it('will hide the contact message with requested specialty ', () => {
      const contactWithSpecialty = wrapper.find('#contact-specialty-1');

      expect(contactWithSpecialty.exists()).toBe(false);
    });
  });
});
