import i18n from '@/plugins/i18n';
import ReaffirmDecisionLink from '@/components/organ-donation/ReaffirmDecisionLink';
import {
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  ORGAN_DONATION_YOUR_CHOICE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const mountReaffirmDecision = ({ $router, $store, isSomeOrgans = false } = {}) =>
  mount(ReaffirmDecisionLink, {
    $router,
    $store,
    propsData: {
      isSomeOrgans,
    },
    mountOpts: { i18n },
  });

describe('reaffirm decision', () => {
  let wrapper;

  beforeEach(() => {
    redirectTo.mockClear();
    wrapper = mountReaffirmDecision();
  });

  describe('reaffirm decision link', () => {
    let link;
    let $store;
    let $router;

    beforeEach(() => {
      $router = [];
      $store = createStore();
      link = wrapper.find('#reaffirmDecisionLink');
    });

    it('will display the link', () => {
      expect(link.exists()).toEqual(true);
    });

    it('will have the correct link text', () => {
      expect(link.text()).toEqual('This is still my decision');
    });

    describe('click', () => {
      beforeEach(() => {
        wrapper = mountReaffirmDecision({ $router, $store, isSomeOrgans: false });
        wrapper.find('#reaffirmDecisionLink').trigger('click');
      });

      it('will dispatch "organDonation/reaffirmStart"', () => {
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/reaffirmStart');
      });

      describe('isSomeOrgans is false', () => {
        beforeEach(() => {
          wrapper = mountReaffirmDecision({ $router, $store, isSomeOrgans: false });
          wrapper.find('#reaffirmDecisionLink').trigger('click');
        });

        it('will push ORGAN_DONATION_REVIEW_YOUR_DECISION to the router', () => {
          expect(redirectTo)
            .toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
        });
      });

      describe('isSomeOrgans is true', () => {
        beforeEach(() => {
          wrapper = mountReaffirmDecision({ $router, $store, isSomeOrgans: true });
          wrapper.find('#reaffirmDecisionLink').trigger('click');
        });

        it('will push ORGAN_DONATION_YOUR_CHOICE to the router', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_YOUR_CHOICE_PATH);
        });
      });
    });
  });
});
