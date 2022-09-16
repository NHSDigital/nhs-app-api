import ReferralBookable from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralBookableCard';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { mount } from '../../../../helpers';

jest.mock('@/components/appointments/hospital-referrals-appointments/RedirectorMixin', () => ({
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
  let bookOrManageReferralButton;

  describe('Requested specialty is set', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        serviceSpecialty: 'Cardiology',
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      bookOrManageReferralButton = wrapper.find('[data-purpose="book-or-manage-referral-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to book');
    });

    it('will display the specialty', () => {
      expect(specialty.text()).toBe('Cardiology');
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
        serviceSpecialty: null,
        deepLinkUrl,
      });

      header = wrapper.find('h3');
      specialty = wrapper.find('[data-purpose="specialty"]');
      bookOrManageReferralButton = wrapper.find('[data-purpose="book-or-manage-referral-button"]');
    });

    it('will display an h3 header', () => {
      expect(header.text()).toBe('Ready to book');
    });

    it('will display the specialty', () => {
      expect(specialty.exists()).toBe(false);
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
