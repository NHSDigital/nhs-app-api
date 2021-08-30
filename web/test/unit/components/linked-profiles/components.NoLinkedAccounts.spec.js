import NoLinkedProfiles from '@/components/linked-profiles/NoLinkedProfiles';
import { mount } from '../../helpers';

const BASE_NHS_APP_HELP_URL = 'http://stubs.local.bitraft.io/help-and-support/';

describe('switch profile button component', () => {
  const mountComponent = () =>
    mount(NoLinkedProfiles, { $env: { BASE_NHS_APP_HELP_URL } });

  describe('content', () => {
    let page;

    beforeEach(() => {
      page = mountComponent();
    });

    it('should contain the correct text content', () => {
      // assert
      const paragraphs = page.findAll('p');
      expect(paragraphs.at(0).text()).toEqual('You do not have any linked profiles set up on your account.');
      expect(paragraphs.at(1).text()).toEqual('Family members and carers can access health services on behalf of someone else through linked profiles.');
      expect(paragraphs.at(2).text()).toEqual('You can book appointments for them, order repeat prescriptions, and view their health record, where appropriate.');
    });

    it('should contain a link to more information', () => {
      // assert
      const link = page.find('#findOutMoreLink');
      expect(link.text()).toEqual('Find out more about linked profiles');
      expect(link.attributes('href')).toEqual('http://stubs.local.bitraft.io/help-and-support/linked-profiles-in-the-nhs-app/');
    });
  });
});
