import isEmpty from 'lodash/fp/isEmpty';
import mapKeys from 'lodash/fp/mapKeys';
import mapValues from 'lodash/fp/mapValues';
import pickBy from 'lodash/fp/pickBy';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { shallowMount } from '../../helpers';


const createChoices = () => initialState().registration.decisionDetails.choices;
const mount = ({ $style = { chosen: 'chosen', notChosen: 'notChosen' }, choices = {} } = {}) =>
  shallowMount(DecisionDetails, { $style, propsData: { choices } });

describe('DecisionDetails', () => {
  let wrapper;
  let choices;
  let chosenHeader;
  let notChosenHeader;

  beforeEach(() => {
    wrapper = mount();
  });

  it('will require choices', () => {
    const prop = wrapper.vm.$options.props.choices;
    expect(prop).not.toBeUndefined();
    expect(prop.required).toEqual(true);
    expect(prop.type).toEqual(Object);
  });

  describe('all choices selected', () => {
    beforeEach(() => {
      choices = mapValues(() => 'Yes')(createChoices());
      wrapper = mount({ choices });
      chosenHeader = wrapper.find('.chosen h4');
      notChosenHeader = wrapper.find('.notChosen h4');
    });

    it('will show the heading for "You have chosen to donate', () => {
      expect(chosenHeader.exists()).toEqual(true);
    });

    it('will not show the heading for "You have chosen not to donate', () => {
      expect(notChosenHeader.exists()).toEqual(false);
    });
  });

  describe('some choices selected', () => {
    beforeEach(() => {
      choices = mapValues(() => 'No')(createChoices());
      choices.heart = 'Yes';
      choices.kidney = 'Yes';
      wrapper = mount({ choices });
      chosenHeader = wrapper.find('.chosen > h4');
      notChosenHeader = wrapper.find('.notChosen > h4');
    });

    it('will show the heading for "You have chosen to donate', () => {
      expect(chosenHeader.exists()).toEqual(true);
    });

    it('will show the heading for "You have chosen not to donate', () => {
      expect(notChosenHeader.exists()).toEqual(true);
    });

    it('will have the correct text for the chosen header', () => {
      expect(chosenHeader.text())
        .toEqual('translate_organDonation.reviewYourDecision.decisionDetails.chosenHeader');
    });

    it('will have the correct text for the not chosen header', () => {
      expect(notChosenHeader.text())
        .toEqual('translate_organDonation.reviewYourDecision.decisionDetails.notChosenHeader');
    });

    it('will have a list containing the chosen organs', () => {
      const list = wrapper.find('.chosen > ul');
      expect(list.exists()).toEqual(true);

      const text = list.text();
      const chosen = pickBy(val => val === 'Yes')(choices);
      expect(isEmpty(chosen)).toEqual(false);

      mapKeys((value) => {
        expect(text)
          .toContain(`translate_organDonation.reviewYourDecision.decisionDetails.choices.${value}`);
      })(chosen);
    });

    it('will have a list containing the non-chosen organs', () => {
      const list = wrapper.find('.notChosen > ul');
      expect(list.exists()).toEqual(true);

      const text = list.text();
      const chosen = pickBy(val => val === 'No')(choices);
      expect(isEmpty(chosen)).toEqual(false);

      mapKeys((value) => {
        expect(text)
          .toContain(`translate_organDonation.reviewYourDecision.decisionDetails.choices.${value}`);
      })(chosen);
    });
  });
});
