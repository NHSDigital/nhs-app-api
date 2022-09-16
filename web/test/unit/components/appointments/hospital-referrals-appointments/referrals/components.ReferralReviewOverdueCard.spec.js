import ReferralReviewOverdue from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralReviewOverdueCard';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { mount } from '../../../../helpers';

jest.mock('@/components/appointments/hospital-referrals-appointments/RedirectorMixin', () => ({
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
  let reviewDueDateHeader;
  let reviewDueDate;
  let contactClinicButton;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        reviewDueDate: '2022-04-05T10:00:00',
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
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

    it('will display the review date value', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('5 April 2022');
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
        reviewDueDate: '2022-04-02T10:00:00',
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
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

    it('will display the review date value', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('2 April 2022');
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
