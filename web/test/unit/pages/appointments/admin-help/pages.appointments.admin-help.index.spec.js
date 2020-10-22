import AdminHelpPage from '@/pages/appointments/gp-appointments/admin-help';
import each from 'jest-each';
import i18n from '@/plugins/i18n';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH, APPOINTMENTS_PATH } from '@/router/paths';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createRouter, createStore } from '../../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('Admin Help page', () => {
  let page;
  let $store;

  const $style = {
    desktopWeb: 'desktopWeb',
  };

  const mountPage = ({
    stubDemographicsQuestion = true,
    available,
    shouldShowLeavingModal = true,
    error = false,
    demographicsQuestionAnswered,
  } = {}) => {
    const stubs = {
      orchestrator: '<div class="orchestrator"></div>',
      'online-consultations-unavailable': '<div class="online-consultations-unavailable"></div>',
      'page-title': '<div></div>',
    };

    if (stubDemographicsQuestion) {
      stubs['demographics-question'] = '<div class="demographicsQuestion"></div>';
    }

    $store = createStore({
      state: {
        device: { isNativeApp: true },
        onlineConsultations: {
          available,
          adminProviderName: 'eConsult Health Ltd',
          error,
          demographicsQuestionAnswered,
        },
        pageLeaveWarning: {
          shouldSkipDisplayingLeavingWarning: false,
        },
        serviceJourneyRules: {
          rules: {
            cdssAdmin: { serviceDefinition: 'NHS_ADMIN', provider: 'stubs' },
            cdssAdvice: { serviceDefinition: 'NHS_ADVICE', provider: 'stubs' },
          },
        },
      },
    });

    page = mount(AdminHelpPage, {
      $store,
      $style,
      stubs,
      getters: {
        'pageLeaveWarning/shouldShowLeavingModal': shouldShowLeavingModal,
      },
      $router: createRouter(),
      mountOpts: {
        i18n,
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    window.onbeforeunload = () => {};
  });

  describe('created', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
      EventBus.$on.mockClear();
      EventBus.$off.mockClear();
    });

    describe('online consultations available', () => {
      beforeEach(() => {
        mountPage({ available: true });
      });

      it('will dispatch onlineConsultations/serviceDefinitionIsValid with provider as argument', () => {
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/serviceDefinitionIsValid', 'stubs');
      });

      it('will not emit any update header events', () => {
        expect(EventBus.$emit).not.toHaveBeenCalled();
      });

      it('will dispatch onlineConsultations/setJourneyInfo passing provider and service definition id as an argument', () => {
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setJourneyInfo', {
          provider: 'stubs',
          serviceDefinitionId: 'NHS_ADMIN',
        });
      });

      it('will set available to true on the component', () => {
        expect(page.vm.available).toBe(true);
      });
    });

    describe('online consultations unavailable', () => {
      beforeEach(() => {
        mountPage({ available: false });
      });

      it('will check if the service definition is valid for the given provider', () => {
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/serviceDefinitionIsValid', 'stubs');
      });

      it('will emit UPDATE_HEADER passing unavailable header and caption', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, {
          headerKey: 'appointments.adminHelp.onlineConsultationsUnavailable',
          captionKey: 'appointments.adminHelp.additionalGpServices',
        });
      });

      it('will emit UPDATE_TITLE passing unavailable header', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'appointments.adminHelp.onlineConsultationsUnavailable');
      });

      it('will not set the journey info in the store', () => {
        expect($store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/setJourneyInfo', expect.anything());
      });

      it('will set available to false on the component', () => {
        expect(page.vm.available).toBe(false);
      });
    });

    describe('online consultations isValid call failed', () => {
      beforeEach(() => {
        mountPage();
      });

      it('will check if the service definition is valid for the given provider', () => {
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/serviceDefinitionIsValid', 'stubs');
      });

      it('will not update the page header, caption, and title to unavailable', () => {
        expect(EventBus.$emit).not.toHaveBeenCalled();
      });

      it('will not set the journey info in the store', () => {
        expect($store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/setJourneyInfo', expect.anything());
      });

      it('will leave available as undefined on the component', () => {
        expect(page.vm.available).toBeUndefined();
      });
    });
  });

  describe('beforeRouteLeave', () => {
    describe('should show modal', () => {
      const next = jest.fn();
      beforeEach(() => {
        next.mockClear();
        $store.dispatch.mockClear();
        $store.state.pageLeaveWarning.shouldSkipDisplayingLeavingWarning = false;
        $store.getters['pageLeaveWarning/shouldShowLeavingModal'] = true;
      });
      each([INDEX_PATH, APPOINTMENTS_PATH])
        .it('will show page leaving warning', (path) => {
          const showModal = jest.fn();

          AdminHelpPage.beforeRouteLeave.call({ $store, showModal }, { path }, undefined, next);

          expect(next).toHaveBeenCalledWith(false);
          expect($store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/setAttemptedRedirectRoute', path);
          expect(showModal).toHaveBeenCalled();

          expect(window.onbeforeunload).not.toBe(null);
        });
    });

    describe('should not show modal', () => {
      const next = jest.fn();
      beforeEach(() => {
        next.mockClear();
        $store.dispatch.mockClear();
        $store.state.pageLeaveWarning.shouldSkipDisplayingLeavingWarning = true;
        $store.getters['pageLeaveWarning/shouldShowLeavingModal'] = false;
      });
      each([INDEX_PATH, APPOINTMENTS_PATH])
        .it('will not show page leaving warning', (path) => {
          const showModal = jest.fn();

          AdminHelpPage.beforeRouteLeave.call({ $store, showModal }, { path }, undefined, next);

          expect(next).toHaveBeenCalledWith(true);
          expect(showModal).toHaveBeenCalledTimes(0);
          expect(window.onbeforeunload).toBe(null);
        });
    });
  });

  describe('lifecycle hooks', () => {
    describe('beforeDestroy', () => {
      it('should call the onlineConsultations clear action with true', () => {
        // Arrange
        mountPage();

        page.destroy();

        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/clear', true);
      });

      it('should call the page leave warning reset action', () => {
        mountPage();

        page.destroy();

        expect($store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/reset');
      });
    });
  });

  describe('template', () => {
    describe('online consultations unavailable message', () => {
      each([
        ['will not be visible when available is true', true],
        ['will be visible when available is false', false],
      ]).it('%s', async (_, available) => {
        mountPage({ available });
        await page.vm.$nextTick();

        expect(page.find('div.online-consultations-unavailable').exists()).toBe(!available);
      });
    });

    describe('error dialog', () => {
      it('should not appear is available is false', async () => {
        mountPage({ available: false });
        await page.vm.$nextTick();

        expect(page.find('[data-purpose=error-heading]').exists()).toBe(false);
        expect(page.find('[data-purpose=reason-error]').exists()).toBe(false);
      });
      it('should not appear if onlineConsultations error state is false', async () => {
        mountPage({ error: false, available: true });
        await page.vm.$nextTick();

        expect(page.find('[data-purpose=error-heading]').exists()).toBe(false);
        expect(page.find('[data-purpose=reason-error]').exists()).toBe(false);
      });
      it('should appear if onlineConsultations error state is true', async () => {
        mountPage({ error: true, available: true });
        await page.vm.$nextTick();

        expect(page.find('[data-purpose=error-container]').exists()).toBe(true);
      });
      it('should display the default error content for admin help', async () => {
        mountPage({ error: true, available: true });
        await page.vm.$nextTick();

        const errorHeading = page.find('[data-purpose=error-heading]');
        const errorReason = page.find('[data-purpose=reason-error]');

        expect(errorHeading.exists()).toBe(true);
        expect(errorReason.exists()).toBe(true);
        expect(errorHeading.text()).toBe('Sorry, we\'re experiencing technical difficulties');
        expect(errorReason.text()).toBe('If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.');
      });
    });
    describe('demographicsQuestion', () => {
      it('should not appear is available is false', async () => {
        mountPage({ available: false });
        await page.vm.$nextTick();

        expect(page.find('div.demographicsQuestion').exists()).toBe(false);
      });
      each([
        true,
        false,
      ]).it('will be shown if demographics question is answered', async (demographicsQuestionAnswered) => {
        mountPage({ demographicsQuestionAnswered, available: true });
        await page.vm.$nextTick();

        expect(page.find('div.demographicsQuestion')).toBeDefined();
      });
      it('will have appropriate attributes set for provider and serviceDefinitionId', async () => {
        mountPage({ available: true });
        await page.vm.$nextTick();

        const { provider, serviceDefinitionId } = page.find('div.demographicsQuestion').vm;

        expect(provider).toEqual('stubs');
        expect(serviceDefinitionId).toEqual('NHS_ADMIN');
      });
      it('will display three demographics question paragraphs passed via slot', async () => {
        mountPage({ stubDemographicsQuestion: false, available: true });
        await page.vm.$nextTick();

        const demographicsQuestionParagraphs = page.find('div.demographicsQuestion').findAll('p').wrappers;

        expect(demographicsQuestionParagraphs[0].text()).toEqual('Use this service to contact your GP surgery for things like test results, sick notes, GP letters and medical reports.');
        expect(demographicsQuestionParagraphs[1].text()).toEqual('It takes around 5 minutes to answer a few questions.');
        expect(demographicsQuestionParagraphs[2].text()).toEqual('To save you typing in personal information the online consultation service needs, you can use the personal information we already hold.');
      });
    });
    describe('orchestrator', () => {
      it('should not appear is available is false', async () => {
        mountPage({ available: false });
        await page.vm.$nextTick();

        expect(page.find('div.orchestrator').exists()).toBe(false);
      });
      it('should appear if onlineConsultations error state is false and demographics question answered', async () => {
        mountPage({ error: false, demographicsQuestionAnswered: true, available: true });
        await page.vm.$nextTick();

        expect(page.find('div.orchestrator').exists()).toBe(true);
      });
      it('should not appear if onlineConsultations error state is true', async () => {
        mountPage({ error: true, available: true });
        await page.vm.$nextTick();

        expect(page.find('div.orchestrator').exists()).toBe(false);
      });
    });
  });
});
