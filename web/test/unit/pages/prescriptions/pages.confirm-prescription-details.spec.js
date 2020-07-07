import ConfirmPrescription from '@/pages/prescriptions/confirm-prescription-details';
import { mount } from '../../helpers';
import { PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS, PRESCRIPTIONS_ORDER_SUCCESS } from '@/lib/routes';
import * as dependency from '@/lib/utils';

jest.mock('@/lib/utils');

const createStore = ({
  hasNoNominatedPharmacy = false,
  pharmacyName = undefined,
  sjrEnabled = true,
  isProxying = false,
}) => ({
  dispatch: jest.fn(),
  app: {
    $env: {},
  },
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
    'session/isProxying': isProxying,
  },
});

const createStoreForNoJsTesting = ({ selectedCoursesNoJs, submitted, specialRequest }) => ({
  dispatch: jest.fn(() => Promise.resolve()),
  app: {
    router: {
      push: jest.fn(),
    },
    $env: {},
  },
  state: {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'pharmacy name',
      },
    },
    repeatPrescriptionCourses: {
      submitted,
      selectedCoursesNoJs,
      specialRequest,
      specialRequestNecessity: 'Optional',
    },
  },
  getters: {
    'repeatPrescriptionCourses/selectedPrescriptions': [{ id: 'course-id-1' }],
    'nominatedPharmacy/hasNoNominatedPharmacy': false,
    'serviceJourneyRules/nominatedPharmacyEnabled': true,
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
  let $redirect;
  let wrapper;

  beforeEach(() => {
    $redirect = jest.fn();
    dependency.redirectTo = jest.fn();
  });

  it('calling fetch when submitted is true (for no js) should submit the prescription order - multiple courses', async () => {
    // arrange
    $store = createStoreForNoJsTesting({ selectedCoursesNoJs: ['course-id-1', 'course-id-2'], submitted: true, specialRequest: 'asap please' });
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch');

    // act
    await wrapper.vm.$options.fetch({ store: $store, redirect: $redirect });

    // assert
    expect($store.dispatch).toHaveBeenCalledWith(
      'repeatPrescriptionCourses/orderRepeatPrescription',
      { CourseIds: ['course-id-1', 'course-id-2'], SpecialRequest: 'asap please' },
    );
    expect($redirect).toHaveBeenCalled();
    expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
  });

  it('calling fetch when submitted is true (for no js) should submit the prescription order - single course', async () => {
    // arrange
    $store = createStoreForNoJsTesting({ selectedCoursesNoJs: 'course-id-1', submitted: true, specialRequest: 'asap please' });
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch');

    // act
    await wrapper.vm.$options.fetch({ store: $store, redirect: $redirect });

    // assert
    expect($store.dispatch).toHaveBeenCalledWith(
      'repeatPrescriptionCourses/orderRepeatPrescription',
      { CourseIds: ['course-id-1'], SpecialRequest: 'asap please' },
    );
    expect($redirect).toHaveBeenCalled();
    expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
  });

  it('calling fetch when submitted is true (for no js) should submit the prescription order - partial success result', async () => {
    // arrange
    $store = createStoreForNoJsTesting({ selectedCoursesNoJs: ['course-id-1', 'course-id-2'], submitted: true, specialRequest: 'asap please' });
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch').mockImplementation((calledWith) => {
      if (calledWith === 'repeatPrescriptionCourses/orderRepeatPrescription') {
        $store.state.repeatPrescriptionCourses.partialOrderResult = {
          successfulOrders: [{ courseId: 'course-id-1' }],
          unsuccessfulOrders: [{ courseId: 'course-id-2' }],
        };
      }
    });

    // act
    await wrapper.vm.$options.fetch({ store: $store, redirect: $redirect });

    // assert
    expect($store.dispatch).toHaveBeenCalledWith(
      'repeatPrescriptionCourses/orderRepeatPrescription',
      { CourseIds: ['course-id-1', 'course-id-2'], SpecialRequest: 'asap please' },
    );

    expect($redirect).not.toHaveBeenCalled();
    expect($store.app.router.push).toHaveBeenCalledWith(PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS.path);
    expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
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
      .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_ORDER_SUCCESS.path, null);
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
      .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_ORDER_SUCCESS.path, null);
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

      describe('has no nominated pharmacy', () => {
        beforeEach(() => {
          $store = createStore({ hasNoNominatedPharmacy: true, sjrEnabled });
          wrapper = mountPage($store);
        });

        it('will not exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(false);
        });
      });

      describe('has nominated pharmacy', () => {
        beforeEach(() => {
          $store = createStore({ hasNoNominatedPharmacy: false, pharmacyName: 'Boots', sjrEnabled });
          wrapper = mountPage($store);
        });

        it('will exist', () => {
          expect(wrapper.find(pharmacyBlockId).exists()).toBe(true);
        });
      });
    });
  });
});
