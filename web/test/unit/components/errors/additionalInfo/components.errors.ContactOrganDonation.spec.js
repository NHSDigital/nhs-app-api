import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import { mount } from '../../../helpers';

const mountWrapper = () => mount(ContactOrganDonation, {
  state: {
    device: {
      source: 'web',
    },
  },
});

describe('ContactOrganDonation', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountWrapper();
  });

  describe('text translations', () => {
    it('will display the email label', () => {
      expect(wrapper.text()).toContain('translate_organ_donation.errors.contact.email');
    });

    it('will display the NHSApp Enquiries email', () => {
      expect(wrapper.text()).toContain('NHSApp.Enquiries@nhsbt.nhs.uk');
    });
  });
});
