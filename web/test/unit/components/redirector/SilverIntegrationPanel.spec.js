import SilverIntegrationPanel from '@/components/redirector/SilverIntegrationPanel';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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
      redirectPath: 'https://test.url.com/nhs-login/login?phrPath=/diary/listAppointments.action',
      sessionStorageName: 'sessionName',
    },
  });

  beforeEach(() => {
    EventBus.$emit.mockClear();
    wrapper = mountComponent();
  });

  describe('mounted', () => {
    it('will emit UPDATE_TITLE on EventBus with third party feature name', async () => {
      await wrapper.vm.$nextTick();
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER);
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'View appointments', true);
    });
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
