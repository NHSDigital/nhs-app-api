import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import { createStore, mount, createRouter, createEvent } from '../../helpers';
import { SYMPTOMS, APPOINTMENT_ADMIN_HELP } from '@/lib/routes';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

describe('Appointment guidance menu', () => {
  let $store;
  let $router;

  const createWrapper = () => {
    $router = createRouter();
    $store = createStore({
      $router,
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
    return mount(AppointmentGuidanceMenu, { $store, $router });
  };

  it('will contain the correct content ', () => {
    const wrapper = createWrapper();
    const tagArray = wrapper.findAll(AnalyticsTrackedTag);
    expect(tagArray.length).toBe(2);

    const symptomsButtonHeader = tagArray.at(0).find('a h2');
    expect(symptomsButtonHeader.text()).toContain('translate_appointments.guidance.menuItem1.header');

    const symptomsButtonText = tagArray.at(0).find('a p');
    expect(symptomsButtonText.text()).toContain('translate_appointments.guidance.menuItem1.text');

    const requestGPHelpButtonHeader = tagArray.at(1).find('a h2');
    expect(requestGPHelpButtonHeader.text()).toContain('translate_appointments.guidance.menuItem3.header');

    const requestGPHelpButtonText = tagArray.at(1).find('a p');
    expect(requestGPHelpButtonText.text()).toContain('translate_appointments.guidance.menuItem3.text');
  });

  it('will link to the check symptoms page when check symptoms menu item clicked', () => {
    const wrapper = createWrapper();
    const event = createEvent({ currentTarget: { pathname: SYMPTOMS.path } });
    wrapper.vm.navigate(event);
    expect($router.push).toHaveBeenCalledWith(SYMPTOMS.path);
  });

  it('will link to the online consultation orchastrator page when request admin help menu item clicked', () => {
    const wrapper = createWrapper();
    const event = createEvent({ currentTarget: { pathname: APPOINTMENT_ADMIN_HELP.path } });
    wrapper.vm.navigate(event);
    expect($store.dispatch).toHaveBeenCalledWith('navigation/setNewMenuItem', 1);
    expect($router.push).toHaveBeenCalledWith(APPOINTMENT_ADMIN_HELP.path);
  });
});
