import ReferralBookable from '@/components/wayfinder/referrals/ReferralBookableCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = referral => mount(
  ReferralBookable, {
    propsData: { item: referral },
  },
);

describe('Referral Bookable Card', () => {
  let wrapper;
  let header;
  let specialty;
  let referralDateHeader;
  let referralDate;
  let referrerHeader;
  let referrer;
  let bookOrManageReferralButton;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Mahogany GP Surgery',
        referredDateTime: '2022-04-11T10:00:00',
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      bookOrManageReferralButton = wrapper.find('[data-purpose="book-or-manage-referral-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to book');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
    });

    it('will display the referral date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('11 April 2022');
    });

    it('will display the referrer value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Mahogany GP Surgery');
    });

    it('will display a button', () => {
      expect(bookOrManageReferralButton.text()).toBe('Book or manage this referral');
    });

    it('will call goToUrlViaRedirector when the book or manage button is clicked', () => {
      bookOrManageReferralButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });

  describe('Requested specialty is not set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        referrerOrganisation: 'Birch GP Surgery',
        referredDateTime: '2022-04-12T10:00:00',
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      referralDateHeader = wrapper.find('[data-purpose="referral-date-header"]');
      referralDate = wrapper.find('[data-purpose="referral-date"]');
      referrerHeader = wrapper.find('[data-purpose="referrer-header"]');
      referrer = wrapper.find('[data-purpose="referrer"]');
      bookOrManageReferralButton = wrapper.find('[data-purpose="book-or-manage-referral-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to book');
    });

    it('will display the specialty', () => {
      expect(specialty.exists()).toBe(false);
    });

    it('will display the referral date value', () => {
      expect(referralDateHeader.text()).toBe('Date you were referred:');
      expect(referralDate.text()).toBe('12 April 2022');
    });

    it('will display the referrer value', () => {
      expect(referrerHeader.text()).toBe('Referred by:');
      expect(referrer.text()).toBe('Birch GP Surgery');
    });

    it('will display a button', () => {
      expect(bookOrManageReferralButton.text()).toBe('Book or manage this referral');
    });

    it('will call goToUrlViaRedirector when the book or manage button is clicked', () => {
      bookOrManageReferralButton.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });
});
