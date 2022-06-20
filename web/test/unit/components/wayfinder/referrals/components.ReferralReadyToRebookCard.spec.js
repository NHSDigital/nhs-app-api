import ReferralReadyToRebook from '@/components/wayfinder/referrals/ReferralReadyToRebookCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = referral => mount(
  ReferralReadyToRebook, {
    propsData: { item: referral },
  },
);

describe('Referral Ready To Rebook Card', () => {
  let wrapper;
  let header;
  let specialty;
  let specialtyInfo;
  let referrerHeader;
  let referrer;
  let referralDateHeader;
  let referralDate;
  let noSpecialtyInfo;
  let bookOrManageButton;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Oak GP Surgery',
        referredDateTime: '2022-04-15T10:00:00',
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      bookOrManageButton = wrapper.find('[data-purpose="book-or-manage-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to rebook');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
    });

    it('will display the specialty message', () => {
      expect(specialtyInfo.text()).toBe('You need to rebook your Cardiology referral appointment as the one you had booked has been cancelled.');
    });

    it('will hide the no specialty message', () => {
      expect(noSpecialtyInfo.exists()).toBe(false);
    });

    it('will display the referred by value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Oak GP Surgery');
    });

    it('will display the referred date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('15 April 2022');
    });

    it('will display a button', () => {
      expect(bookOrManageButton.text()).toBe('Book or manage this referral');
    });

    it('will call goToUrlViaRedirector when the book or manage button is clicked', () => {
      bookOrManageButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Requested specialty is not set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Pine GP Surgery',
        referredDateTime: '2022-04-16T10:00:00',
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      specialtyInfo = wrapper.find('[data-purpose="specialty-info"]');
      noSpecialtyInfo = wrapper.find('[data-purpose="no-specialty-info"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      bookOrManageButton = wrapper.find('[data-purpose="book-or-manage-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to rebook');
    });

    it('will hide the specialty', () => {
      expect(specialty.exists()).toBe(false);
    });

    it('will hide the specialty message', () => {
      expect(specialtyInfo.exists()).toBe(false);
    });

    it('will display the no specialty message', () => {
      expect(noSpecialtyInfo.text()).toBe('You need to rebook your referral appointment as the one you had booked has been cancelled.');
    });

    it('will display the referred by value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Pine GP Surgery');
    });

    it('will display the referred date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('16 April 2022');
    });

    it('will display a button', () => {
      expect(bookOrManageButton.text()).toBe('Book or manage this referral');
    });

    it('will call goToUrlViaRedirector when the book or manage button is clicked', () => {
      bookOrManageButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });
});
