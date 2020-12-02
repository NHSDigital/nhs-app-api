import NoLinkedProfiles from '@/components/linked-profiles/NoLinkedProfiles';
import { LINKED_PROFILES_FIND_OUT_MORE_URL } from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

describe('switch profile button component', () => {
  const mountComponent = () =>
    mount(NoLinkedProfiles, { $store: createStore() });

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
      expect(link.attributes('href')).toEqual(LINKED_PROFILES_FIND_OUT_MORE_URL);
    });
  });
});
