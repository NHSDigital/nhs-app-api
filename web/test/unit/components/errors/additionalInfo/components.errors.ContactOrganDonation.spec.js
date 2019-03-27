import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import { $t, mount } from '../../../helpers';

const contactLink = 'https://www.foo.com';
const mountWrapper = () => mount(ContactOrganDonation, {
  $env: {
    ORGAN_DONATION_CONTACT_US_URL: contactLink,
  },
  state: {
    device: {
      source: 'web',
    },
  },
  $t,
});

describe('ContactOrganDonation', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountWrapper();
  });

  describe('text translations', () => {
    it('will display the email label', () => {
      expect($t).toHaveBeenCalledWith('organ_donation.errors.contact.email');
    });

    it('will display the NHSApp Enquiries email', () => {
      expect(wrapper.text()).toContain('NHSApp.Enquiries@nhsbt.nhs.uk');
    });

    it('will display the phone label', () => {
      expect($t).toHaveBeenCalledWith('organ_donation.errors.contact.phone');
    });

    it('will display the other ways text', () => {
      expect($t).toHaveBeenCalledWith('organ_donation.errors.contact.otherWays');
    });
  });

  describe('contact link', () => {
    let link;

    beforeEach(() => {
      link = wrapper.find('#contactUs');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have the contact URL for the href', () => {
      expect(link.attributes().href).toEqual(contactLink);
    });
  });

  describe('computed', () => {
    describe('contactUsText', () => {
      it('will format URL for display', () => {
        expect(wrapper.vm.contactUsText).toEqual('foo.com');
      });
    });
  });
});
