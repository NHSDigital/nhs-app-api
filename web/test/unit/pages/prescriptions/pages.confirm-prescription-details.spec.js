import ConfirmPrescription from '@/pages/prescriptions/confirm-prescription-details';
import { PRESCRIPTIONS_ORDER_SUCCESS_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import { mount } from '../../helpers';

jest.mock('@/lib/utils');

const createStore = ({
  hasNoNominatedPharmacy = false,
  pharmacyName = undefined,
  sjrEnabled = true,
  pharmacyEnabled = false,
  isProxying = false,
}) => ({
  dispatch: jest.fn(),
  $env: {},
  state: {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName,
      },
    },
    repeatPrescriptionCourses: {
      specialRequest: '',
      specialRequestNecessity: 'NotAllowed',
    },
  },
  getters: {
    'repeatPrescriptionCourses/selectedPrescriptions': [{ id: 1 }],
    'nominatedPharmacy/hasNoNominatedPharmacy': hasNoNominatedPharmacy,
    'serviceJourneyRules/nominatedPharmacyEnabled': sjrEnabled,
    'nominatedPharmacy/nominatedPharmacyEnabled': pharmacyEnabled,
    'session/isProxying': isProxying,
  },
});

const mountPage = ($store) => {
  const page = mount(ConfirmPrescription, {
    $store,
  });
  return page;
};

describe('confirm prescriptions', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    dependency.redirectTo = jest.fn();
  });

  it('should submit the prescription order once even when clicked multiple times', async () => {
    // arrange
    const confirmButtonId = '#btn_confirm_and_order_prescription';
    $store = createStore({});
    wrapper = mountPage($store);

    // act
    wrapper.find(confirmButtonId).trigger('click');
    wrapper.find(confirmButtonId).trigger('click');

    // assert
    expect($store.dispatch)
      .toHaveBeenNthCalledWith(1, 'repeatPrescriptionCourses/orderRepeatPrescription', {
        CourseIds: [1],
        SpecialRequest: '',
      });
  });

  it('should submit the prescription when clicked as proxy and redirect to confirmation page', async () => {
    // arrange
    $store = createStore({ isProxying: true });
    wrapper = mountPage($store);

    // act
    await wrapper.vm.onConfirmButtonClicked();

    // assert
    expect($store.dispatch)
      .toHaveBeenNthCalledWith(1, 'repeatPrescriptionCourses/orderRepeatPrescription', {
        CourseIds: [1],
        SpecialRequest: '',
      });

    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_ORDER_SUCCESS_PATH, null);
  });

  it('should submit the prescription when clicked the user and redirect to confirmation page', async () => {
    // arrange
    $store = createStore({ isProxying: false });
    wrapper = mountPage($store);

    // act
    await wrapper.vm.onConfirmButtonClicked();

    // assert
    expect($store.dispatch)
      .toHaveBeenNthCalledWith(1, 'repeatPrescriptionCourses/orderRepeatPrescription', {
        CourseIds: [1],
        SpecialRequest: '',
      });

    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_ORDER_SUCCESS_PATH, null);
  });

  describe('nominated pharmacy summary', () => {
    const pharmacyBlockId = '#my-nominated-pharmacy';


    describe('SJR disabled', () => {
      beforeEach(() => {
        $store = createStore({ hasNoNominatedPharmacy: true, pharmacyName: 'boots', sjrEnabled: false });
        wrapper = mountPage($store);
      });

      it('will not exist', () => {
        expect(wrapper.find(pharmacyBlockId).exists()).toBe(false);
      });
    });

    describe('SJR enabled', () => {
      const sjrEnabled = true;
      const pharmacyEnabled = true;
      const hasNoNominatedPharmacy = true;

      describe('has no nominated pharmacy', () => {
        beforeEach(() => {
          $store = createStore({ hasNoNominatedPharmacy: true, sjrEnabled, pharmacyEnabled });
          wrapper = mountPage($store);
        });

        it('will show no nominated pharmacy message', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(true);
        });
      });

      describe('has nominated pharmacy', () => {
        beforeEach(() => {
          $store = createStore({ hasNoNominatedPharmacy: false, pharmacyName: 'Boots', sjrEnabled, pharmacyEnabled });
          wrapper = mountPage($store);
        });

        it('will exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(true);
        });
      });

      describe('has no nominated pharmacy and nominated pharmacy is disabled', () => {
        beforeEach(() => {
          $store = createStore({ hasNoNominatedPharmacy, sjrEnabled, pharmacyEnabled: false });
          wrapper = mountPage($store);
        });

        it('will not exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(false);
        });
      });
    });
  });
});
