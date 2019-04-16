import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import { $t, mount } from '../../../helpers';

const mountWrapper = () => mount(ContactOrganDonation, {
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
  });
});
