import DynamicBackLinkMixin from '@/components/organ-donation/DynamicBackLinkMixin';
import Vuex from 'vuex';
import { createLocalVue, mount } from '@vue/test-utils';
import { INDEX_PATH } from '@/router/paths';

jest.mock('@/lib/utils');

const localVue = createLocalVue();
localVue.use(Vuex);
localVue.mixin(DynamicBackLinkMixin);

const firstPath = 'first path';
const secondPath = 'second path';
const lastPath = 'last Path';

describe('backClick', () => {
  let wrapper;
  let router;

  const mountPage = ({ $router }) => {
    const testPage = { template: '<div></div>' };
    router = $router;
    return mount(testPage, {
      localVue,
      mocks: {
        $router,
      },
    });
  };

  const setUpTest = (setUp) => {
    wrapper = mountPage(setUp);
    wrapper.vm.backClicked();
  };

  describe('With previous paths', () => {
    beforeEach(() => {
      setUpTest({
        $router: {
          history: {
            router: {
              previousPaths: [firstPath, secondPath, lastPath],
            },
          },
          goBack: jest.fn(),
        },
      });
    });

    it('will redirect to last path in router history', () => {
      expect(wrapper.vm.backLink).toEqual(lastPath);
      expect(router.goBack).toBeCalled();
    });
  });

  describe('With no previous paths', () => {
    beforeEach(() => {
      setUpTest({
        $router: {
          history: {
            router: {
              previousPaths: [],
            },
          },
          goBack: jest.fn(),
        },
      });
    });

    it('will redirect to index', () => {
      expect(wrapper.vm.backLink).toEqual(INDEX_PATH);
      expect(router.goBack).toBeCalled();
    });
  });
});
