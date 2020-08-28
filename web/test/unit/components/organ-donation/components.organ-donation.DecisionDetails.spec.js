import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import i18n from '@/plugins/i18n';
import mapValues from 'lodash/fp/mapValues';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { mount } from '../../helpers';


const createChoices = () => initialState().registration.decisionDetails.choices;
const mountDecisionInfo = ({
  $style = { chosen: 'chosen', notChosen: 'notChosen', notStated: 'notStated' },
  choices = {},
  isSomeOrgans = false,
} = {}) =>
  mount(DecisionDetails, { $style, propsData: { choices, isSomeOrgans }, mountOpts: { i18n } });

describe('DecisionDetails', () => {
  let wrapper;
  let choices;
  let chosenHeader;
  let notChosenHeader;
  let notStatedHeader;

  beforeEach(() => {
    wrapper = mountDecisionInfo();
  });

  it('will have a decision details header', () => {
    const header = wrapper.find('#decision-details-header');
    expect(header.exists()).toBe(true);
    expect(header.text()).toEqual('Decision details');
  });

  it('will have decision details text', () => {
    const text = wrapper.find('#decision-details-text');
    expect(text.exists()).toBe(true);
    expect(text.text()).toEqual('I want to donate all my organs and tissue.');
  });

  it('will have a choices property', () => {
    const prop = wrapper.vm.$options.props.choices;
    expect(prop).not.toBeUndefined();
    expect(prop.default()).toEqual({});
    expect(prop.type).toEqual(Object);
  });

  it('will have a isSomeOrgans property', () => {
    const prop = wrapper.vm.$options.props.isSomeOrgans;
    expect(prop).not.toBeUndefined();
    expect(prop.type).toEqual(Boolean);
  });

  describe('all organs', () => {
    beforeEach(() => {
      wrapper = mountDecisionInfo({ isSomeOrgans: false });
    });

    it('will display the decision details text for all organs', () => {
      expect(wrapper.find('#decision-details-text').text())
        .toEqual('I want to donate all my organs and tissue.');
    });

    it('will not display chosen items', () => {
      expect(wrapper.find('#chosen h4').exists()).toBe(false);
    });

    it('will not display not chosen items', () => {
      expect(wrapper.find('#notChosen h4').exists()).toBe(false);
    });

    it('will not display not stated items', () => {
      expect(wrapper.find('#notStated h4').exists()).toBe(false);
    });
  });

  describe('some organs', () => {
    describe('all choices selected', () => {
      beforeEach(() => {
        choices = mapValues(() => 'Yes')(createChoices());
        wrapper = mountDecisionInfo({ choices, isSomeOrgans: true });
        chosenHeader = wrapper.find('#chosen h4');
        notChosenHeader = wrapper.find('#notChosen h4');
        notStatedHeader = wrapper.find('#notStated h4');
      });

      it('will display the decision details text for all organs', () => {
        expect(wrapper.find('#decision-details-text').text())
          .toEqual('I want to donate some organs and tissue.');
      });

      it('will show the heading for "You have chosen to donate', () => {
        expect(chosenHeader.exists()).toEqual(true);
      });

      it('will not show the heading for "You have chosen not to donate', () => {
        expect(notChosenHeader.exists()).toEqual(false);
      });

      it('will not show the heading for "We do not have a decision for', () => {
        expect(notStatedHeader.exists()).toEqual(false);
      });
    });

    describe('some choices selected', () => {
      beforeEach(() => {
        choices = mapValues(() => 'No')(createChoices());
        choices.heart = 'Yes';
        choices.kidney = 'Yes';
        wrapper = mountDecisionInfo({ choices, isSomeOrgans: true });
        chosenHeader = wrapper.find('#chosen > h4');
        notChosenHeader = wrapper.find('#notChosen > h4');
        notStatedHeader = wrapper.find('#notStated > h4');
      });

      it('will display the decision details text for all organs', () => {
        expect(wrapper.find('#decision-details-text').text())
          .toEqual('I want to donate some organs and tissue.');
      });

      it('will show the heading for "You have chosen to donate', () => {
        expect(chosenHeader.exists()).toEqual(true);
      });

      it('will show the heading for "You have chosen not to donate', () => {
        expect(notChosenHeader.exists()).toEqual(true);
      });

      it('will not show the heading for "We do not have a decision for', () => {
        expect(notStatedHeader.exists()).toEqual(false);
      });

      it('will have the correct text for the chosen header', () => {
        expect(chosenHeader.text()).toEqual('You have chosen to donate:');
      });

      it('will have the correct text for the not chosen header', () => {
        expect(notChosenHeader.text()).toEqual('You have chosen not to donate:');
      });

      it('will have a list containing the chosen organs', () => {
        const list = wrapper.find('#chosen > ul');
        expect(list.exists()).toEqual(true);

        expect(list.text()).toContain('Heart');
        expect(list.text()).toContain('Kidney');
      });

      it('will have a list containing the non-chosen organs', () => {
        const list = wrapper.find('#notChosen > ul');
        expect(list.exists()).toEqual(true);

        expect(list.text()).toContain('Lungs');
        expect(list.text()).toContain('Liver');
        expect(list.text()).toContain('Corneas');
        expect(list.text()).toContain('Pancreas');
        expect(list.text()).toContain('Tissue');
        expect(list.text()).toContain('Small bowel');
      });
    });

    describe('some choices selected, with some not stated', () => {
      beforeEach(() => {
        choices = mapValues(() => 'No')(createChoices());
        choices.heart = 'Yes';
        choices.kidney = 'Yes';
        choices.tissue = 'NotStated';
        wrapper = mountDecisionInfo({ choices, isSomeOrgans: true });
        chosenHeader = wrapper.find('#chosen > h4');
        notChosenHeader = wrapper.find('#notChosen > h4');
        notStatedHeader = wrapper.find('#notStated > h4');
      });

      it('will display the decision details text for all organs', () => {
        expect(wrapper.find('#decision-details-text').text())
          .toEqual('I want to donate some organs and tissue.');
      });

      it('will show the heading for "You have chosen to donate', () => {
        expect(chosenHeader.exists()).toEqual(true);
      });

      it('will show the heading for "You have chosen not to donate', () => {
        expect(notChosenHeader.exists()).toEqual(true);
      });

      it('will show the heading for "We do not have a decision for', () => {
        expect(notStatedHeader.exists()).toEqual(true);
      });

      it('will have a list containing the chosen organs', () => {
        const list = wrapper.find('#chosen > ul');
        expect(list.exists()).toEqual(true);

        expect(list.text()).toContain('Heart');
        expect(list.text()).toContain('Kidney');
      });

      it('will have a list containing the non-chosen organs', () => {
        const list = wrapper.find('#notChosen > ul');
        expect(list.exists()).toEqual(true);

        expect(list.text()).toContain('Lungs');
        expect(list.text()).toContain('Liver');
        expect(list.text()).toContain('Corneas');
        expect(list.text()).toContain('Pancreas');
        expect(list.text()).toContain('Small bowel');
      });

      it('will have a list containing the non-stated organs', () => {
        const list = wrapper.find('#notStated > ul');
        expect(list.exists()).toEqual(true);

        expect(list.text()).toBe('Tissue');
      });
    });
  });
});
