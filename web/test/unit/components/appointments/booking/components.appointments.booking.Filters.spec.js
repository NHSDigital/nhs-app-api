/* eslint-disable import/no-extraneous-dependencies */
import { mount, createLocalVue } from '@vue/test-utils';
import Filters from '@/components/appointments/booking/Filters';

const $t = key => `translate_${key}`;

const createFiltersComponent = ($store, props = {}) => {
  const propsData = props;
  const $http = jest.fn();
  const localVue = createLocalVue();

  return mount(Filters, {
    localVue,
    propsData,
    mocks: {
      $store,
      $style: { form: 'form' },
      $http,
      $t,
    },
  });
};

describe('Filters.vue', () => {
  it('will do not render drop-down options by default', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store);

    expect(component.find('#type').findAll('option').length).toEqual(0);
    expect(component.find('#location').findAll('option').length).toEqual(0);
    expect(component.find('#clinician').findAll('option').length).toEqual(0);
    expect(component.find('#time-period').findAll('option').length).toEqual(0);
  });

  it('will render type drop-down element', () => {
    const props = {
      options: {
        types: [
          { value: '1', name: 'TYPE 1', translate: false },
          { value: '2', name: 'type_2', translate: true },
        ],
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#type').findAll('option').length).toEqual(2);
    expect(component.find('#type').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#type').findAll('option').at(0).text()).toEqual('TYPE 1');
    expect(component.find('#type').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#type').findAll('option').at(1).text()).toEqual('translate_type_2');
  });

  it('will render location drop-down element', () => {
    const props = {
      options: {
        locations: [
          { value: '1', name: 'LOCATION 1', translate: false },
          { value: '2', name: 'location_2', translate: true },
        ],
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#location').findAll('option').length).toEqual(2);
    expect(component.find('#location').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#location').findAll('option').at(0).text()).toEqual('LOCATION 1');
    expect(component.find('#location').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#location').findAll('option').at(1).text()).toEqual('translate_location_2');
  });

  it('will render clinician drop-down element', () => {
    const props = {
      options: {
        clinicians: [
          { value: '1', name: 'CLINICIAN 1', translate: false },
          { value: '2', name: 'clinician_2', translate: true },
        ],
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#clinician').findAll('option').length).toEqual(2);
    expect(component.find('#clinician').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#clinician').findAll('option').at(0).text()).toEqual('CLINICIAN 1');
    expect(component.find('#clinician').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#clinician').findAll('option').at(1).text()).toEqual('translate_clinician_2');
  });

  it('will render date drop-down element', () => {
    const props = {
      options: {
        dates: [
          { value: '1', name: 'DATE 1', translate: false },
          { value: '2', name: 'date_2', translate: true },
        ],
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#time-period').findAll('option').length).toEqual(2);
    expect(component.find('#time-period').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#time-period').findAll('option').at(0).text()).toEqual('DATE 1');
    expect(component.find('#time-period').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#time-period').findAll('option').at(1).text()).toEqual('translate_date_2');
  });

  it('will select correct option for type drop-down', () => {
    const props = {
      options: {
        types: [
          { value: '1', name: 'TYPE 1', translate: false },
          { value: '2', name: 'type_2', translate: true },
        ],
      },
      selectedOptions: {
        type: '2',
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#type').find('option:selected').attributes().value).toEqual('2');
  });

  it('will select correct option for location drop-down', () => {
    const props = {
      options: {
        locations: [
          { value: '1', name: 'LOCATION 1', translate: false },
          { value: '2', name: 'location_2', translate: true },
        ],
      },
      selectedOptions: {
        location: '2',
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#location').find('option:selected').attributes().value).toEqual('2');
  });

  it('will select correct option for clinician drop-down', () => {
    const props = {
      options: {
        clinicians: [
          { value: '1', name: 'CLINICIAN 1', translate: false },
          { value: '2', name: 'clinician_2', translate: true },
        ],
      },
      selectedOptions: {
        clinician: '2',
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#clinician').find('option:selected').attributes().value).toEqual('2');
  });

  it('will select correct option for date drop-down', () => {
    const props = {
      options: {
        dates: [
          { value: '1', name: 'DATE 1', translate: false },
          { value: '2', name: 'date_2', translate: true },
        ],
      },
      selectedOptions: {
        date: '2',
      },
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    const component = createFiltersComponent($store, props);

    expect(component.find('#time-period').find('option:selected').attributes().value).toEqual('2');
  });
});
