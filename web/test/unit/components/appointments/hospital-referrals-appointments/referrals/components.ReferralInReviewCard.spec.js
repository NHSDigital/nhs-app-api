import ReferralInReview from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralInReviewCard';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { mount } from '../../../../helpers';

jest.mock('@/components/appointments/hospital-referrals-appointments/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = referral => mount(
  ReferralInReview, {
    propsData: { item: referral },
  },
);

describe('Referral In Review Card', () => {
  let wrapper;
  let header;
  let specialty;
  let specialtyInfo;
  let reviewDueDateHeader;
  let reviewDueDate;
  let noSpecialtyInfo;
  let manageReferralLink;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        reviewDueDate: '2022-04-18T10:00:00',
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
      manageReferralLink = wrapper.find('[data-purpose="manage-referral-link"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Referral request in review');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
    });

    it('will display the specialty message', () => {
      expect(specialtyInfo.text()).toBe('Your healthcare provider has requested for you to be referred to Cardiology. This request is being reviewed. You do not need to do anything.');
    });

    it('will hide the no specialty message', () => {
      expect(noSpecialtyInfo.exists()).toBe(false);
    });

    it('will display the review due date', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('18 April 2022');
    });

    it('will display a deep link', () => {
      expect(manageReferralLink.text()).toBe('View or manage this referral');
    });

    it('will call goToUrlViaRedirector when the manage referral link is clicked', () => {
      manageReferralLink.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Requested specialty is empty', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        reviewDueDate: '2022-04-19T10:00:00',
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
      manageReferralLink = wrapper.find('[data-purpose="manage-referral-link"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Referral request in review');
    });

    it('will hide the specialty', () => {
      expect(specialty.exists()).toBe(false);
    });

    it('will hide the specialty message', () => {
      expect(specialtyInfo.exists()).toBe(false);
    });

    it('will display the no specialty message', () => {
      expect(noSpecialtyInfo.text()).toBe('Your healthcare provider has requested for you to be referred. This request is being reviewed. You do not need to do anything.');
    });

    it('will display the review due date', () => {
      expect(reviewDueDateHeader.text()).toBe('Due to be reviewed by:');
      expect(reviewDueDate.text()).toBe('19 April 2022');
    });

    it('will display a deep link', () => {
      expect(manageReferralLink.text()).toBe('View or manage this referral');
    });

    it('will call goToUrlViaRedirector when the manage referral link is clicked', () => {
      manageReferralLink.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Review due date is empty', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        reviewDueDate: null,
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      reviewDueDateHeader = wrapper.find('[data-purpose="review-due-date-header"]');
      reviewDueDate = wrapper.find('[data-purpose="review-due-date"]');
      manageReferralLink = wrapper.find('[data-purpose="manage-referral-link"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Referral request in review');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
    });

    it('will display the specialty message', () => {
      expect(specialtyInfo.text()).toBe('Your healthcare provider has requested for you to be referred to Cardiology. This request is being reviewed. You do not need to do anything.');
    });

    it('will hide the no specialty message', () => {
      expect(noSpecialtyInfo.exists()).toBe(false);
    });

    it('will hide the review due date', () => {
      expect(reviewDueDateHeader.exists()).toBe(false);
      expect(reviewDueDate.exists()).toBe(false);
    });

    it('will display a deep link', () => {
      expect(manageReferralLink.text()).toBe('View or manage this referral');
    });

    it('will call goToUrlViaRedirector when the manage referral link is clicked', () => {
      manageReferralLink.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });
});
