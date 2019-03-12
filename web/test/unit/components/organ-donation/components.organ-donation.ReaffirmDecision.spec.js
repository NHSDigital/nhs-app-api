import ReaffirmDecision from '@/components/organ-donation/ReaffirmDecision';
import {
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_YOUR_CHOICE,
} from '@/lib/routes';

import { createStore, mount } from '../../helpers';

const mountReaffirmDecision = ({ $router, $store, isSomeOrgans = false } = {}) =>
  mount(ReaffirmDecision, {
    $router,
    $store,
    propsData: {
      isSomeOrgans,
    },
  });

describe('reaffirm decision', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountReaffirmDecision();
  });

  it('will display the title', () => {
    expect(wrapper.find('h3').text())
      .toEqual('translate_organDonation.reaffirmDecision.subheader');
  });

  it('will display the text', () => {
    expect(wrapper.find('p').text())
      .toEqual('translate_organDonation.reaffirmDecision.text');
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
      expect(link.text()).toEqual('translate_organDonation.reaffirmDecision.linkText');
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
          expect($router).toContain(ORGAN_DONATION_REVIEW_YOUR_DECISION.path);
        });
      });

      describe('isSomeOrgans is true', () => {
        beforeEach(() => {
          wrapper = mountReaffirmDecision({ $router, $store, isSomeOrgans: true });
          wrapper.find('#reaffirmDecisionLink').trigger('click');
        });

        it('will push ORGAN_DONATION_YOUR_CHOICE to the router', () => {
          expect($router).toContain(ORGAN_DONATION_YOUR_CHOICE.path);
        });
      });
    });
  });
});
