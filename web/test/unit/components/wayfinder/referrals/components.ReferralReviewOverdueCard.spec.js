import ReferralReviewOverdue from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = referral => mount(
  ReferralReviewOverdue, {
    propsData: { item: referral },
  },
);

describe('Referral Ready Overdue Card', () => {
  let wrapper;
  let header;
  let specialty;
  let specialtyInfo;
  let noSpecialtyInfo;
  let referralDateHeader;
  let referralDate;
  let reviewDueDateHeader;
  let reviewDueDate;
  let referrerHeader;
  let referrer;
  let contactClinicButton;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Fir GP Surgery',
        referredDateTime: '2022-04-04T10:00:00',
        reviewDueDate: '2022-04-05T10:00:00',
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      contactClinicButton = wrapper.find('[data-purpose="contact-clinic-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Review by clinic overdue');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
    });

    it('will display the specialty message', () => {
      expect(specialtyInfo.text()).toBe('Your healthcare provider has requested for you to be referred to Cardiology. A review of this request is overdue. You need to contact the clinic.');
    });

    it('will hide the no specialty message', () => {
      expect(noSpecialtyInfo.exists()).toBe(false);
    });

    it('will display the referred by value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Fir GP Surgery');
    });

    it('will display the review date value', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('5 April 2022');
    });

    it('will display the referred date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('4 April 2022');
    });

    it('will display a button', () => {
      expect(contactClinicButton.text()).toBe('Contact the clinic');
    });

    it('will call goToUrlViaRedirector when the contact the clinic button is clicked', () => {
      contactClinicButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Requested specialty is not set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Sycamore GP Surgery',
        referredDateTime: '2022-04-01T10:00:00',
        reviewDueDate: '2022-04-02T10:00:00',
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      contactClinicButton = wrapper.find('[data-purpose="contact-clinic-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Review by clinic overdue');
    });

    it('will hide the specialty', () => {
      expect(specialty.exists()).toBe(false);
    });

    it('will hide the specialty message', () => {
      expect(specialtyInfo.exists()).toBe(false);
    });

    it('will display the no specialty message', () => {
      expect(noSpecialtyInfo.text()).toBe('Your healthcare provider has requested for you to be referred. A review of this request is overdue. You need to contact the clinic.');
    });

    it('will display the referred by value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Sycamore GP Surgery');
    });

    it('will display the review date value', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('2 April 2022');
    });

    it('will display the referred date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('1 April 2022');
    });

    it('will display a button', () => {
      expect(contactClinicButton.text()).toBe('Contact the clinic');
    });

    it('will call goToUrlViaRedirector when the contact the clinic button is clicked', () => {
      contactClinicButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });
});
