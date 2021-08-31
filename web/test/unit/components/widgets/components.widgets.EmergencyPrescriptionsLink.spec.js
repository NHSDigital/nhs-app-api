import EmergencyPrescriptionsLink from '@/components/widgets/EmergencyPrescriptionsLink';
import { mount } from '../../helpers';

describe('EmergencyPrescriptionsLink', () => {
  const mountEmergencyPrescriptionsLink = () => mount(EmergencyPrescriptionsLink);
  let wrapper;
  let link;

  beforeEach(() => {
    wrapper = mountEmergencyPrescriptionsLink();
    link = wrapper.find('a');
  });

  it('will contain hyperlink', () => {
    expect(link.exists()).toBe(true);
  });

  it(' hyperlink goes to 111 website on new page', () => {
    expect(link.attributes().target).toEqual('_blank');
    expect(link.attributes().href).toEqual('https://111.nhs.uk/emergency-prescription');
  });
});
