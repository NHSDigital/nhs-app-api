import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import { createStore, mount } from '../../helpers';

describe('Silver Integration warning panel', () => {
  let wrapper;

  const mountComponent = () => mount(SilverIntegrationPanel, {
    $store: createStore(),
    propsData: {
      knownService: {
        id: 'pkb',
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        url: 'http://www.url.com',
      },
      redirectPath: 'path',
      sessionStorageName: 'sessionName',
    },
  });

  beforeEach(() => {
    wrapper = mountComponent();
  });

  describe('button', () => {
    let continueButton;

    beforeEach(() => {
      continueButton = wrapper.find('a.nhsuk-button');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    describe('on click', () => {
      beforeEach(() => {
        continueButton.trigger('click');
      });

      it('will emit click', () => {
        expect(wrapper.emitted().click).toBeTruthy();
      });

      it('will disable button', () => {
        expect(wrapper.vm.buttonDisabled).toEqual(true);
        expect(wrapper.find('a.nhsuk-button.nhsuk-button--disabled').exists()).toBe(true);
      });
    });
  });
});
