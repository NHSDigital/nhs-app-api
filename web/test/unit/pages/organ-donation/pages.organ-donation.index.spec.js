import AlreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import NextSteps from '@/components/organ-donation/NextSteps';
import OrganDonation from '@/pages/organ-donation';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import ReaffirmDecisionLink from '@/components/organ-donation/ReaffirmDecisionLink';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  initialState,
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
  STATE_CONFLICTED,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

const createState =
  ({ decision = DECISION_UNKNOWN, originalDecision = decision, originalChoices } = {}) => {
    const state = {
      organDonation: initialState(),
      device: {
        isNativeApp: true,
      },
    };

    state.organDonation.registration.decision = decision;
    state.organDonation.originalRegistration.decision = originalDecision;

    if (originalChoices) {
      state.organDonation.originalRegistration.decisionDetails.choices = originalChoices;
    }

    return state;
  };

const createPageStore = ({ state, isSomeOrgans = false } = {}) => createStore({
  state,
  getters: {
    'organDonation/isSomeOrgans': isSomeOrgans,
  },
});

const createStyle = () => ({
  button: 'button',
  grey: 'grey',
  appointedRep: 'appointedRep',
});

const verifyComponentExistence = ({
  alreadyRegistered,
  amendDecisionLink,
  appointedRepresentative,
  decisionDetails,
  faithDetailsRegistered,
  findOutMore,
  makeDecision,
  nextSteps,
  otherThingsToDo,
  reaffirmDecisionLink,
  yourDecision,
}, wrapperFn) => {
  const showOrNot = bool => (bool ? 'show' : 'not show');

  it(`will ${showOrNot(alreadyRegistered)} AlreadyRegisteredLink`, () => {
    expect(wrapperFn().find(AlreadyRegisteredLink).exists()).toBe(alreadyRegistered);
  });

  it(`will ${showOrNot(amendDecisionLink)} AmendDecisionLink`, () => {
    expect(wrapperFn().find(AmendDecisionLink).exists()).toEqual(amendDecisionLink);
  });

  it(`will ${showOrNot(appointedRepresentative)} appointed representative`, () => {
    expect(wrapperFn().find('.appointedRep').exists()).toBe(appointedRepresentative);
  });

  it(`will ${showOrNot(decisionDetails)} DecisionDetails`, () => {
    expect(wrapperFn().find(DecisionDetails).exists()).toBe(decisionDetails);
  });

  it(`will ${showOrNot(faithDetailsRegistered)} FaithDetailsRegistered`, () => {
    expect(wrapperFn().find(FaithDetailsRegistered).exists()).toBe(faithDetailsRegistered);
  });

  it(`will ${showOrNot(findOutMore)} FindOutMoreLink`, () => {
    expect(wrapperFn().find(FindOutMoreLink).exists()).toBe(findOutMore);
  });

  it(`will ${showOrNot(makeDecision)} MakeDecision`, () => {
    expect(wrapperFn().find(MakeDecision).exists()).toBe(makeDecision);
  });

  it(`will ${showOrNot(nextSteps)} NextSteps`, () => {
    expect(wrapperFn().find(NextSteps).exists()).toBe(nextSteps);
  });

  it(`will ${showOrNot(otherThingsToDo)} OtherThingsToDo`, () => {
    expect(wrapperFn().find(OtherThingsToDo).exists()).toBe(otherThingsToDo);
  });

  it(`will ${showOrNot(reaffirmDecisionLink)} ReaffirmDecisionLink`, () => {
    expect(wrapperFn().find(ReaffirmDecisionLink).exists()).toEqual(reaffirmDecisionLink);
  });

  it(`will ${showOrNot(yourDecision)} YourDecision`, () => {
    expect(wrapperFn().find(YourDecision).exists()).toBe(yourDecision);
  });
};

describe('organ donation index page', () => {
  let $store;
  let $style;
  let wrapper;

  const mountOrganDonation = $route => mount(OrganDonation, { $route, $store, $style });

  describe('created', () => {
    beforeEach(() => {
      $store = createPageStore({ state: createState() });
      wrapper = mountOrganDonation();
    });

    it('will dispatch the "organDonation/amendCancel" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendCancel');
    });

    it('will dispatch the "organDonation/withdrawCancel" action', () => {
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/withdrawCancel');
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
      $store = createPageStore({ state: createState() });
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

  describe('watch - $router.query.ts', () => {
    beforeEach(() => {
      $store = createPageStore({ state: createState() });
      wrapper = mountOrganDonation({ query: {} });

      wrapper.setData({ $route: { query: { ts: 'foo' } } });
    });

    it('will dispatch `organDonation/getReferenceData`', () => {
      expect($store.dispatch).toBeCalledWith('organDonation/getReferenceData');
    });

    it('will dispatch `organDonation/getRegistration`', () => {
      expect($store.dispatch).toBeCalledWith('organDonation/getRegistration');
    });
  });

  describe('new registration (original decision is not found)', () => {
    beforeEach(() => {
      $store = createPageStore({ state: createState() });
      $style = createStyle();
      wrapper = mountOrganDonation();
    });

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: true,
        amendDecisionLink: false,
        appointedRepresentative: false,
        decisionDetails: false,
        faithDetailsRegistered: false,
        findOutMore: true,
        makeDecision: true,
        nextSteps: false,
        otherThingsToDo: false,
        reaffirmDecisionLink: false,
        yourDecision: false,
      }, () => wrapper);
    });

    describe('computed', () => {
      describe('hasExistingDecision', () => {
        it('will be false as the original decision was not found', () => {
          expect(wrapper.vm.hasExistingDecision).toBe(false);
        });
      });
    });
  });

  describe('loaded registration (appointed representative)', () => {
    beforeEach(() => {
      $store = createPageStore({
        state: createState({
          decision: DECISION_APPOINTED_REP,
        }),
      });
      wrapper = mountOrganDonation();
    });

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: false,
        amendDecisionLink: true,
        appointedRepresentative: true,
        decisionDetails: false,
        faithDetailsRegistered: false,
        findOutMore: false,
        makeDecision: false,
        nextSteps: false,
        otherThingsToDo: true,
        reaffirmDecisionLink: false,
        yourDecision: true,
      }, () => wrapper);
    });

    describe('YourDecision component', () => {
      it('will have the decision set to app-rep', () => {
        expect(wrapper.find(YourDecision).props().decision).toBe(DECISION_APPOINTED_REP);
      });
    });

    describe('appointed representative section', () => {
      it('will translate the phone label', () => {
        expect(wrapper.text()).toContain('translate_organDonation.registered.appointedRep.phoneLabel');
      });
    });

    describe('computed', () => {
      describe('hasExistingDecision', () => {
        it('will be true as the original decision is appointed representative', () => {
          expect(wrapper.vm.hasExistingDecision).toBe(true);
        });
      });
    });
  });

  describe('loaded registration (conflicted state)', () => {
    beforeEach(() => {
      $store = createPageStore({
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

    describe('component existence', () => {
      verifyComponentExistence({
        alreadyRegistered: false,
        amendDecisionLink: false,
        appointedRepresentative: false,
        decisionDetails: false,
        faithDetailsRegistered: false,
        findOutMore: false,
        makeDecision: false,
        nextSteps: false,
        otherThingsToDo: true,
        reaffirmDecisionLink: false,
        yourDecision: false,
      }, () => wrapper);
    });
  });

  describe('loaded registration (original decision is found)', () => {
    beforeEach(() => {
      $store = createPageStore({
        state: createState(),
      });
    });

    describe('opt-out', () => {
      beforeEach(() => {
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_OUT;
        $store.state.organDonation.originalRegistration.decisionDetails.all = undefined;
        $store.state.organDonation.originalRegistration.decisionDetails.choices = undefined;
        wrapper = mountOrganDonation();
      });

      describe('component existence', () => {
        verifyComponentExistence({
          alreadyRegistered: false,
          amendDecisionLink: true,
          appointedRepresentative: false,
          decisionDetails: false,
          faithDetailsRegistered: false,
          findOutMore: false,
          makeDecision: false,
          nextSteps: true,
          otherThingsToDo: true,
          reaffirmDecisionLink: true,
          yourDecision: true,
        }, () => wrapper);
      });

      describe('YourDecision component', () => {
        it('will have the decision set to opt-out', () => {
          expect(wrapper.find(YourDecision).props().decision).toBe(DECISION_OPT_OUT);
        });
      });

      describe('computed', () => {
        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-out', () => {
            expect(wrapper.vm.hasExistingDecision).toBe(true);
          });
        });
      });
    });

    describe('opt-in, all organs', () => {
      beforeEach(() => {
        $store = createPageStore({ state: createState() });
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
        $store.state.organDonation.originalRegistration.decisionDetails.all = true;
        wrapper = mountOrganDonation();
      });

      describe('component existence', () => {
        verifyComponentExistence({
          alreadyRegistered: false,
          amendDecisionLink: true,
          appointedRepresentative: false,
          decisionDetails: true,
          faithDetailsRegistered: true,
          findOutMore: false,
          makeDecision: false,
          nextSteps: true,
          otherThingsToDo: true,
          reaffirmDecisionLink: true,
          yourDecision: true,
        }, () => wrapper);
      });

      describe('ReaffirmDecisionLink', () => {
        let reaffirmDecisionLink;

        beforeEach(() => {
          reaffirmDecisionLink = wrapper.find(ReaffirmDecisionLink);
        });

        it('will have isSomeOrgans of false', () => {
          expect(reaffirmDecisionLink.vm.isSomeOrgans).toEqual(false);
        });
      });

      describe('YourDecision component', () => {
        it('will have the decision set to opt-in', () => {
          expect(wrapper.find(YourDecision).props().decision).toBe(DECISION_OPT_IN);
        });
      });

      describe('computed', () => {
        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-in', () => {
            expect(wrapper.vm.hasExistingDecision).toBe(true);
          });
        });
      });
    });

    describe('opt-in, some organs', () => {
      const choices = { heart: 'Yes' };

      beforeEach(() => {
        $store = createPageStore({ state: createState(), isSomeOrgans: true });
        $store.state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
        $store.state.organDonation.originalRegistration.decisionDetails.all = false;
        $store.state.organDonation.originalRegistration.decisionDetails.choices = choices;
        wrapper = mountOrganDonation();
      });

      describe('component existence', () => {
        verifyComponentExistence({
          alreadyRegistered: false,
          amendDecisionLink: true,
          appointedRepresentative: false,
          decisionDetails: true,
          faithDetailsRegistered: true,
          findOutMore: false,
          makeDecision: false,
          nextSteps: true,
          otherThingsToDo: true,
          reaffirmDecisionLink: true,
          yourDecision: true,
        }, () => wrapper);
      });

      describe('DecisionDetails component', () => {
        it('will have its choices set from the original registration choices', () => {
          expect(wrapper.find(DecisionDetails).props().choices).toEqual(choices);
        });
      });

      describe('ReaffirmDecisionLink component', () => {
        let reaffirmDecisionLink;

        beforeEach(() => {
          reaffirmDecisionLink = wrapper.find(ReaffirmDecisionLink);
        });

        it('will have an isSomeOrgans of true', () => {
          expect(reaffirmDecisionLink.vm.isSomeOrgans).toEqual(true);
        });
      });

      describe('YourDecision component', () => {
        it('will have the decision set to opt-in', () => {
          expect(wrapper.find(YourDecision).props().decision).toBe(DECISION_OPT_IN);
        });
      });

      describe('computed', () => {
        describe('hasExistingDecision', () => {
          it('will be true as the original decision is opt-in', () => {
            expect(wrapper.vm.hasExistingDecision).toBe(true);
          });
        });
      });
    });
  });
});

