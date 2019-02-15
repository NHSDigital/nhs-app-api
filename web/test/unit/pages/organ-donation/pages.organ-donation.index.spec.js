import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import OrganDonation from '@/pages/organ-donation';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
  STATE_CONFLICTED,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

const createState =
  ({ decision = DECISION_UNKNOWN, originalDecision = decision, originalChoices } = {}) => {
    const state = {
      organDonation: initialState(),
      device: {
        source: 'web',
      },
    };

    state.organDonation.registration.decision = decision;
    state.organDonation.originalRegistration.decision = originalDecision;

    if (originalChoices) {
      state.organDonation.originalRegistration.decisionDetails.choices = originalChoices;
    }

    return state;
  };

const createStyle = () => ({
  button: 'button',
  grey: 'grey',
  appointedRep: 'appointedRep',
});

describe('organ donation index page', () => {
  let $store;
  let $style;
  let wrapper;

  const mountOrganDonation = () => mount(OrganDonation, { $store, $style, $t });

  describe('created', () => {
    beforeEach(() => {
      $store = createStore({ state: createState() });
      wrapper = mountOrganDonation();
    });

    it('will dispach the "organDonation/amendCancel" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendCancel');
    });

    it('will dispach the "organDonation/setAdditionalDetails" action with empty values', () => {
      const value = { ethnicityId: '', religionId: '' };
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setAdditionalDetails', value);
    });

    it('will dispach the "organDonation/resetAcceptanceChecks" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/resetAcceptanceChecks');
    });
  });

  describe('new registration (original decision is not found)', () => {
    beforeEach(() => {
      $store = createStore({ state: createState() });
      $style = createStyle();
      wrapper = mountOrganDonation();
    });

    it('will show the "MakeDecision" component', () => {
      expect(wrapper.find(MakeDecision).exists()).toEqual(true);
    });

    it('will not have a "YourDecision" component', () => {
      expect(wrapper.find(YourDecision).exists()).toEqual(false);
    });

    it('will not have a "DecisionDetails" component', () => {
      expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
    });

    it('will not show a GenericButton', () => {
      expect(wrapper.find('#back-button').exists()).toEqual(false);
    });

    it('will not show a AmendDecisionLink', () => {
      expect(wrapper.find(AmendDecisionLink).exists()).toEqual(false);
    });

    it('will not have a "YourDecision" component', () => {
      expect(wrapper.find(YourDecision).exists()).toEqual(false);
    });

    it('will not have a "DecisionDetails" component', () => {
      expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
    });

    describe('async data', () => {
      it('will dispatch the "organDonation/getRegistration" action', () => {
        wrapper.vm.$options.asyncData({ store: $store });
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/getRegistration');
      });
    });

    describe('computed', () => {
      describe('hasExistingDecision', () => {
        it('will be false as the original decision was not found', () => {
          expect(wrapper.vm.hasExistingDecision).toEqual(false);
        });
      });
    });
  });
  describe('Decision found conflicted state', () => {
    beforeEach(() => {
      $store = createStore({
        state: createState({
          originalDecision: DECISION_UNKNOWN,
        }),
      });
      $store.state.organDonation.originalRegistration.decision = DECISION_UNKNOWN;
      $store.state.organDonation.originalRegistration.decisionDetails.all = false;
      $store.state.organDonation.originalRegistration.state = STATE_CONFLICTED;
      $store.state.organDonation.originalRegistration.identifier = '';
      wrapper = mountOrganDonation();
    });

    it('will show the Decision found dialog text', () => {
      expect(wrapper.text())
        .toContain('translate_organDonation.viewDecision.conflictedState.dialogText');
    });

    it('will show the Decision found message text', () => {
      expect(wrapper.text())
        .toContain('translate_organDonation.viewDecision.conflictedState.messageText');
    });

    it('will not show the "MakeDecision" component', () => {
      expect(wrapper.find(MakeDecision).exists()).toEqual(false);
    });
  });

  describe('loaded registration (original decision is found)', () => {
    beforeEach(() => {
      $store = createStore({
        state: createState({
          originalDecision: DECISION_OPT_IN,
        }),
      });

      wrapper = mountOrganDonation();
    });

    it('will not show the "MakeDecision" component', () => {
      expect(wrapper.find(MakeDecision).exists()).toEqual(false);
    });

    describe('opt-out', () => {
      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_OUT;
        $store.state.organDonation.originalRegistration.decisionDetails.all = undefined;
        $store.state.organDonation.originalRegistration.decisionDetails.choices = undefined;
        wrapper = mountOrganDonation();
      });

      describe('Amend decision link', () => {
        it('will exist', () => {
          expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
        });
      });

      describe('DecisionDetails component', () => {
        it('will not exist', () => {
          expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
        });
      });

      describe('YourDecision component', () => {
        it('will exist', () => {
          expect(wrapper.find(YourDecision).exists()).toEqual(true);
        });

        it('will have the decision set to opt-out', () => {
          expect(wrapper.find(YourDecision).props().decision).toEqual(DECISION_OPT_OUT);
        });

        it('will have the decision details set', () => {
          expect(wrapper.find(YourDecision).props().decisionDetails)
            .toEqual($store.state.organDonation.originalRegistration.decisionDetails);
        });

        it('will have the header key set to `organDonation.registered.yourDecision.subheader`', () => {
          expect(wrapper.find(YourDecision).props().headerKey)
            .toEqual('organDonation.registered.yourDecision.subheader');
        });
      });

      describe('appointed representative section', () => {
        let appointedRepSection;

        beforeEach(() => {
          appointedRepSection = wrapper.find('.appointedRep');
        });

        it('will not exist', () => {
          expect(appointedRepSection.exists()).toBe(false);
        });
      });

      describe('computed', () => {
        describe('hasAllOrgans', () => {
          it('will be false as no organs are selected', () => {
            expect(wrapper.vm.hasAllOrgans).toEqual(false);
          });
        });

        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-out', () => {
            expect(wrapper.vm.hasExistingDecision).toEqual(true);
          });
        });

        describe('hasSomeOrgans', () => {
          it('will be false as no organs are selected', () => {
            expect(wrapper.vm.hasSomeOrgans).toEqual(false);
          });
        });
      });
    });

    describe('opt-in, all organs', () => {
      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
        $store.state.organDonation.originalRegistration.decisionDetails.all = true;
        wrapper = mountOrganDonation();
      });

      describe('Amend decision link', () => {
        it('will exist', () => {
          expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
        });
      });

      describe('DecisionDetails component', () => {
        it('will not exist', () => {
          expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
        });
      });

      describe('YourDecision component', () => {
        it('will exist', () => {
          expect(wrapper.find(YourDecision).exists()).toEqual(true);
        });

        it('will have the decision set to opt-in', () => {
          expect(wrapper.find(YourDecision).props().decision).toEqual(DECISION_OPT_IN);
        });

        it('will have the decision details set', () => {
          expect(wrapper.find(YourDecision).props().decisionDetails)
            .toEqual($store.state.organDonation.originalRegistration.decisionDetails);
        });

        it('will have the header key set to `organDonation.registered.yourDecision.subheader`', () => {
          expect(wrapper.find(YourDecision).props().headerKey)
            .toEqual('organDonation.registered.yourDecision.subheader');
        });
      });

      describe('appointed representative section', () => {
        let appointedRepSection;

        beforeEach(() => {
          appointedRepSection = wrapper.find('.appointedRep');
        });

        it('will not exist', () => {
          expect(appointedRepSection.exists()).toBe(false);
        });
      });

      describe('computed', () => {
        describe('hasAllOrgans', () => {
          it('will be true as all organs are selected', () => {
            expect(wrapper.vm.hasAllOrgans).toEqual(true);
          });
        });

        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-in', () => {
            expect(wrapper.vm.hasExistingDecision).toEqual(true);
          });
        });

        describe('hasSomeOrgans', () => {
          it('will be false as all organs are selected', () => {
            expect(wrapper.vm.hasSomeOrgans).toEqual(false);
          });
        });
      });
    });

    describe('opt-in, some organs', () => {
      const choices = { heart: 'Yes' };

      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
        $store.state.organDonation.originalRegistration.decisionDetails.all = false;
        $store.state.organDonation.originalRegistration.decisionDetails.choices = choices;
        wrapper = mountOrganDonation();
      });

      describe('Amend decision link', () => {
        it('will exist', () => {
          expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
        });
      });

      describe('DecisionDetails component', () => {
        it('will exist', () => {
          expect(wrapper.find(DecisionDetails).exists()).toEqual(true);
        });

        it('will have its choices set from the original registration choices', () => {
          expect(wrapper.find(DecisionDetails).props().choices).toEqual(choices);
        });
      });

      describe('YourDecision component', () => {
        it('will exist', () => {
          expect(wrapper.find(YourDecision).exists()).toEqual(true);
        });

        it('will have the decision set to opt-in', () => {
          expect(wrapper.find(YourDecision).props().decision).toEqual(DECISION_OPT_IN);
        });

        it('will have the decision details set', () => {
          expect(wrapper.find(YourDecision).props().decisionDetails)
            .toEqual($store.state.organDonation.originalRegistration.decisionDetails);
        });

        it('will have the header key set to `organDonation.registered.yourDecision.subheader`', () => {
          expect(wrapper.find(YourDecision).props().headerKey)
            .toEqual('organDonation.registered.yourDecision.subheader');
        });
      });

      describe('appointed representative section', () => {
        let appointedRepSection;

        beforeEach(() => {
          appointedRepSection = wrapper.find('.appointedRep');
        });

        it('will not exist', () => {
          expect(appointedRepSection.exists()).toBe(false);
        });
      });

      describe('computed', () => {
        describe('hasAllOrgans', () => {
          it('will be false as only some organs are selected', () => {
            expect(wrapper.vm.hasAllOrgans).toEqual(false);
          });
        });

        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-in', () => {
            expect(wrapper.vm.hasExistingDecision).toEqual(true);
          });
        });

        describe('hasSomeOrgans', () => {
          it('will be true as only some organs are selected', () => {
            expect(wrapper.vm.hasSomeOrgans).toEqual(true);
          });
        });
      });
    });

    describe('app-rep', () => {
      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_APPOINTED_REP;
        wrapper = mountOrganDonation();
      });

      describe('DecisionDetails component', () => {
        it('will not exist', () => {
          expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
        });
      });

      describe('YourDecision component', () => {
        it('will exist', () => {
          expect(wrapper.find(YourDecision).exists()).toEqual(true);
        });

        it('will have the decision set to app-rep', () => {
          expect(wrapper.find(YourDecision).props().decision).toEqual(DECISION_APPOINTED_REP);
        });

        it('will have the header key set to `organDonation.registered.yourDecision.subheader`', () => {
          expect(wrapper.find(YourDecision).props().headerKey)
            .toEqual('organDonation.registered.yourDecision.subheader');
        });
      });

      describe('appointed representative section', () => {
        let appointedRepSection;

        beforeEach(() => {
          appointedRepSection = wrapper.find('.appointedRep');
        });

        it('will exist', () => {
          expect(appointedRepSection.exists()).toBe(true);
        });

        it('will translate the phone label', () => {
          expect($t).toHaveBeenCalledWith('organDonation.registered.appointedRep.phoneLabel');
        });
      });

      describe('computed', () => {
        describe('hasAllOrgans', () => {
          it('will be false because the user appointed a representative', () => {
            expect(wrapper.vm.hasAllOrgans).toEqual(false);
          });
        });

        describe('hasExistingDecision', () => {
          it('will be true as the original decision is appointed representative', () => {
            expect(wrapper.vm.hasExistingDecision).toEqual(true);
          });
        });

        describe('hasSomeOrgans', () => {
          it('will be false because the user appointed a representative', () => {
            expect(wrapper.vm.hasSomeOrgans).toEqual(false);
          });
        });
      });
    });
  });
});

