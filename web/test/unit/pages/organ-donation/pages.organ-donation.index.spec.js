import AlreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import NextSteps from '@/components/organ-donation/NextSteps';
import OrganDonation from '@/pages/organ-donation';
import ReaffirmDecision from '@/components/organ-donation/ReaffirmDecision';
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

const verifyComponentExistence = ({
  alreadyRegistered,
  amendDecision,
  appointedRepresentative,
  decisionDetails,
  findOutMore,
  makeDecision,
  reaffirmDecision,
  yourDecision,

}, wrapperFn) => {
  const showOrNot = bool => (bool ? 'show' : 'not show');

  it(`will ${showOrNot(alreadyRegistered)} AlreadyRegisteredLink`, () => {
    expect(wrapperFn().find(AlreadyRegisteredLink).exists()).toEqual(alreadyRegistered);
  });

  it(`will ${showOrNot(amendDecision)} AmendDecisionLink`, () => {
    expect(wrapperFn().find(AmendDecisionLink).exists()).toEqual(amendDecision);
  });

  it(`will ${showOrNot(appointedRepresentative)} appointed representative`, () => {
    expect(wrapperFn().find('.appointedRep').exists()).toBe(appointedRepresentative);
  });

  it(`will ${showOrNot(decisionDetails)} DecisionDetails`, () => {
    expect(wrapperFn().find(DecisionDetails).exists()).toEqual(decisionDetails);
  });

  it(`will ${showOrNot(findOutMore)} FindOutMoreLink`, () => {
    expect(wrapperFn().find(FindOutMoreLink).exists()).toEqual(findOutMore);
  });

  it(`will ${showOrNot(makeDecision)} MakeDecision`, () => {
    expect(wrapperFn().find(MakeDecision).exists()).toEqual(makeDecision);
  });

  it(`will ${showOrNot(reaffirmDecision)} ReaffirmDecision`, () => {
    expect(wrapperFn().find(ReaffirmDecision).exists()).toEqual(reaffirmDecision);
  });

  it(`will ${showOrNot(yourDecision)} YourDecision`, () => {
    expect(wrapperFn().find(YourDecision).exists()).toEqual(yourDecision);
  });
};

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

    it('will dispatch the "organDonation/amendCancel" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendCancel');
    });

    it('will dispatch the "organDonation/setAdditionalDetails" action with empty values', () => {
      const value = { ethnicityId: '', religionId: '' };
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/setAdditionalDetails', value);
    });

    it('will dispatch the "organDonation/resetAcceptanceChecks" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/resetAcceptanceChecks');
    });

    it('will dispatch the "organDonation/reaffirmCancel" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/reaffirmCancel');
    });
  });

  describe('asyncData', () => {
    beforeEach(() => {
      $store = createStore({ state: createState() });
      wrapper = mountOrganDonation();
      wrapper.vm.$options.asyncData({ store: $store });
    });

    it('will request reference data from the api', async () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/getReferenceData');
    });

    it('will dispatch the "organDonation/getRegistration" action', async () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/getRegistration');
    });
  });

  describe('new registration (original decision is not found)', () => {
    beforeEach(() => {
      $store = createStore({ state: createState() });
      $style = createStyle();
      wrapper = mountOrganDonation();
    });

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: true,
        amendDecision: false,
        appointedRepresentative: false,
        decisionDetails: false,
        findOutMore: true,
        makeDecision: true,
        reaffirmDecision: false,
        yourDecision: false,
      }, () => wrapper);
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

    it('will not have a "NextSteps" component', () => {
      expect(wrapper.find(NextSteps).exists()).toEqual(false);
    });

    it('will show the already registered link', () => {
      expect(wrapper.find(AlreadyRegisteredLink).exists()).toEqual(true);
    });

    describe('computed', () => {
      describe('hasExistingDecision', () => {
        it('will be false as the original decision was not found', () => {
          expect(wrapper.vm.hasExistingDecision).toEqual(false);
        });
      });
    });
  });

  describe('loaded registration (appointed representative)', () => {
    beforeEach(() => {
      $store = createStore({
        state: createState({
          decision: DECISION_APPOINTED_REP,
        }),
      });
      wrapper = mountOrganDonation();
    });

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: false,
        amendDecision: true,
        appointedRepresentative: true,
        decisionDetails: false,
        makeDecision: false,
        findOutMore: false,
        reaffirmDecision: false,
        yourDecision: true,
      }, () => wrapper);
    });

    describe('YourDecision component', () => {
      it('will have the decision set to app-rep', () => {
        expect(wrapper.find(YourDecision).props().decision).toEqual(DECISION_APPOINTED_REP);
      });

      it('will have the header key set to `organDonation.registered.yourDecision.subheader`', () => {
        expect(wrapper.find(YourDecision).props().headerKey)
          .toEqual('organDonation.registered.yourDecision.subheader');
      });
    });

    describe('appointed representative section', () => {
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

  describe('loaded registration (conflicted state)', () => {
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

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: false,
        amendDecision: false,
        appointedRepresentative: false,
        decisionDetails: false,
        makeDecision: false,
        findOutMore: false,
        reaffirmDecision: false,
        yourDecision: false,
      }, () => wrapper);
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

    it('will not show the already registered link', () => {
      expect(wrapper.find(AlreadyRegisteredLink).exists()).toEqual(false);
    });

    it('will not show the find out more link', () => {
      expect(wrapper.find(FindOutMoreLink).exists()).toEqual(false);
    });

    it('will not show a AmendDecisionLink', () => {
      expect(wrapper.find(AmendDecisionLink).exists()).toEqual(false);
    });

    it('will not have a "NextSteps" component', () => {
      expect(wrapper.find(NextSteps).exists()).toEqual(false);
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

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: false,
        amendDecision: true,
        appointedRepresentative: false,
        decisionDetails: false,
        makeDecision: false,
        findOutMore: false,
        reaffirmDecision: true,
        yourDecision: true,
      }, () => wrapper);
    });

    describe('opt-out', () => {
      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_OUT;
        $store.state.organDonation.originalRegistration.decisionDetails.all = undefined;
        $store.state.organDonation.originalRegistration.decisionDetails.choices = undefined;
        wrapper = mountOrganDonation();
      });

      it('Amend decision link will exist', () => {
        expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
      });

      it('Next Steps component will exist', () => {
        expect(wrapper.find(NextSteps).exists()).toEqual(true);
      });

      it('DecisionDetails component will not exist', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
      });

      describe('ReaffirmDecision component', () => {
        it('will exist', () => {
          expect(wrapper.find(ReaffirmDecision).exists()).toEqual(true);
        });
      });

      describe('YourDecision component', () => {
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

      it('Amend decision link will exist', () => {
        expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
      });

      it('Next Steps component will exist', () => {
        expect(wrapper.find(NextSteps).exists()).toEqual(true);
      });

      it('DecisionDetails component will not exist', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
      });

      describe('ReaffirmDecision component', () => {
        let reaffirmDecision;

        beforeEach(() => {
          reaffirmDecision = wrapper.find(ReaffirmDecision);
        });

        it('will have isSomeOrgans of false', () => {
          expect(reaffirmDecision.vm.isSomeOrgans).toEqual(false);
        });
      });

      describe('YourDecision component', () => {
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

      it('Amend decision link will exist', () => {
        expect(wrapper.find(AmendDecisionLink).exists()).toEqual(true);
      });

      it('Next Steps component will exist', () => {
        expect(wrapper.find(NextSteps).exists()).toEqual(true);
      });

      describe('DecisionDetails component', () => {
        it('will have its choices set from the original registration choices', () => {
          expect(wrapper.find(DecisionDetails).props().choices).toEqual(choices);
        });
      });

      describe('ReaffirmDecision component', () => {
        let reaffirmDecision;

        beforeEach(() => {
          reaffirmDecision = wrapper.find(ReaffirmDecision);
        });

        it('will have an isSomeOrgans of true', () => {
          expect(reaffirmDecision.vm.isSomeOrgans).toEqual(true);
        });
      });

      describe('YourDecision component', () => {
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

      it('DecisionDetails component will not exist', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
      });

      it('Next Steps component will exist', () => {
        expect(wrapper.find(NextSteps).exists()).toEqual(false);
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

