import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { createStore, mount } from '../../../helpers';
import { redirectTo } from '@/lib/utils';
import { WAYFINDER_HELP_PATH } from '@/router/paths';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const HELP_LINK_WITH_BODY = 'btn_linkWithBody';
const HELP_LINK_WITHOUT_BODY = 'btn_linkWithoutABody';
const HELP_LINK_BODY = 'This is the body';
const HELP_LINK_TEXT = 'This is the main text';
const BACK_LINK_OVERRIDE = 'appointments/hospital-referrals-appointments/waiting-lists';
const WAIT_LIST_CRUMB = 'waitTimes';
const DEFAULT_CRUMB = 'defaultCrumb';

describe('Wayfinder help link', () => {
  let $router;
  let $store;
  let propsData;
  let wrapper;

  const mountAs = () => {
    $store = createStore({
      state: {
        navigation: {
          backLinkOverride: undefined,
          crumbSetName: DEFAULT_CRUMB,
        },
      },
    });

    return mount(HelpLink, { $store, $router, propsData });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    $router = [];
  });

  describe('Wayfinder help link', () => {
    describe('link with a body', () => {
      beforeEach(() => {
        propsData = {
          id: HELP_LINK_WITH_BODY,
          body: HELP_LINK_BODY,
          text: HELP_LINK_TEXT,
          path: '#',
          backlinkOverride: undefined,
          routeCrumb: DEFAULT_CRUMB,
        };
        wrapper = mountAs(propsData);
      });

      it('will display the help link with text', () => {
        const text = wrapper.find('#btn_linkWithBody').find('h2');
        expect(text.exists()).toBe(true);
        expect(text.text()).toBe(HELP_LINK_TEXT);
      });

      it('will display the help link with a body', () => {
        const body = wrapper.find('#btn_linkWithBody').find('p');
        expect(body.exists()).toBe(true);
        expect(body.text()).toBe(HELP_LINK_BODY);
      });
    });

    describe('link without a body', () => {
      beforeEach(() => {
        propsData = {
          id: HELP_LINK_WITHOUT_BODY,
          text: HELP_LINK_TEXT,
          path: '#',
          backlinkOverride: undefined,
          routeCrumb: DEFAULT_CRUMB,
        };
        wrapper = mountAs(propsData);
      });

      it('will display the help link with text', () => {
        const text = wrapper.find('#btn_linkWithoutABody').find('h2');
        expect(text.exists()).toBe(true);
        expect(text.text()).toBe(HELP_LINK_TEXT);
      });

      it('will display the help link without a body', () => {
        const body = wrapper.find('#btn_linkWithoutABody').find('p');
        expect(body.exists()).toBe(false);
      });
    });

    describe('back button clicked when visiting help page from wait times help page jump off link', () => {
      beforeEach(() => {
        propsData = {
          id: HELP_LINK_WITHOUT_BODY,
          text: HELP_LINK_TEXT,
          path: WAYFINDER_HELP_PATH,
          backLinkOverride: BACK_LINK_OVERRIDE,
          routeCrumb: WAIT_LIST_CRUMB,
        };
        wrapper = mountAs();
      });

      it('will redirect to the Wayfinder help path ', () => {
        wrapper = mountAs();
        wrapper.vm.onClickHelpLink();
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, WAYFINDER_HELP_PATH);
      });

      describe('back link override value is set', () => {
        it('the correct value is added to the store and the waitTimes breadcrumb is used', () => {
          wrapper = mountAs();
          wrapper.vm.onClickHelpLink();
          expect($store.dispatch).toHaveBeenCalledWith('navigation/setBackLinkOverride', BACK_LINK_OVERRIDE);
        });
      });

      describe('breadcrumb is set correctly', () => {
        it('the waitTimes breadcrumb is used', () => {
          wrapper = mountAs();
          wrapper.vm.onClickHelpLink();
          expect($store.dispatch).toHaveBeenCalledWith('navigation/setRouteCrumb', WAIT_LIST_CRUMB);
        });
      });
    });

    describe('back button clicked when visiting help page from any of the secondary care help page jump off links', () => {
      beforeEach(() => {
        propsData = {
          id: HELP_LINK_WITHOUT_BODY,
          text: HELP_LINK_TEXT,
          path: WAYFINDER_HELP_PATH,
          backLinkOverride: undefined,
          routeCrumb: DEFAULT_CRUMB,
        };
        wrapper = mountAs();
      });

      it('will redirect to the Wayfinder help path ', () => {
        wrapper = mountAs();
        wrapper.vm.onClickHelpLink();
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, WAYFINDER_HELP_PATH);
      });

      describe('back link override value does not get set', () => {
        it('the value is undefined in the store', () => {
          wrapper = mountAs();
          wrapper.vm.onClickHelpLink();
          expect($store.dispatch).toHaveBeenCalledWith('navigation/setBackLinkOverride', undefined);
        });
      });

      describe('breadcrumb is set correctly', () => {
        it('the default breadcrumb is used', () => {
          wrapper = mountAs();
          wrapper.vm.onClickHelpLink();
          expect($store.dispatch).toHaveBeenCalledWith('navigation/setRouteCrumb', DEFAULT_CRUMB);
        });
      });
    });
  });
});
