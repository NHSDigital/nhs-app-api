import Filters from '@/components/appointments/booking/Filters';
import { createStore, mount } from '../../../helpers';

const selectedOptions = (options = {}) => ({
  type: options.type || '',
  location: options.location || '',
  clinician: options.clinician || '',
  date: options.date || '',
});

const createFiltersComponent = propsData => mount(Filters, {
  propsData,
  $store: createStore({
    state: {
      device: {
        source: 'web',
      },
    },
  }),
  $style: { form: 'form' },
});

describe('Filters.vue', () => {
  it('will do not render drop-down options by default', () => {
    const component = createFiltersComponent();

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
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
    };

    const component = createFiltersComponent(props);

    expect(component.find('#type').findAll('option').length).toEqual(2);
    expect(component.find('#type').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#type').findAll('option').at(0).text()).toEqual('TYPE 1');
    expect(component.find('#type').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#type').findAll('option').at(1).text()).toEqual('Today');
  });

  it('will render location drop-down element', () => {
    const props = {
      options: {
        locations: [
          { value: '1', name: 'LOCATION 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
    };

    const component = createFiltersComponent(props);

    expect(component.find('#location').findAll('option').length).toEqual(2);
    expect(component.find('#location').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#location').findAll('option').at(0).text()).toEqual('LOCATION 1');
    expect(component.find('#location').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#location').findAll('option').at(1).text()).toEqual('Today');
  });

  it('will render clinician drop-down element', () => {
    const props = {
      options: {
        clinicians: [
          { value: '1', name: 'CLINICIAN 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
    };

    const component = createFiltersComponent(props);
    expect(component.find('#clinician').findAll('option').length).toEqual(2);
    expect(component.find('#clinician').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#clinician').findAll('option').at(0).text()).toEqual('CLINICIAN 1');
    expect(component.find('#clinician').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#clinician').findAll('option').at(1).text()).toEqual('Today');
  });

  it('will render date drop-down element', () => {
    const props = {
      options: {
        dates: [
          { value: '1', name: 'DATE 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
    };

    const component = createFiltersComponent(props);

    expect(component.find('#time-period').findAll('option').length).toEqual(2);
    expect(component.find('#time-period').findAll('option').at(0).attributes().value).toEqual('1');
    expect(component.find('#time-period').findAll('option').at(0).text()).toEqual('DATE 1');
    expect(component.find('#time-period').findAll('option').at(1).attributes().value).toEqual('2');
    expect(component.find('#time-period').findAll('option').at(1).text()).toEqual('Today');
  });

  it('will select correct option for type drop-down', () => {
    const props = {
      options: {
        types: [
          { value: '1', name: 'TYPE 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
      value: selectedOptions({ type: '2' }),
    };

    const component = createFiltersComponent(props);

    expect(component.find('#type').findAll('option').at(1).attributes().value).toEqual('2');
  });

  it('will select correct option for location drop-down', () => {
    const props = {
      options: {
        locations: [
          { value: '1', name: 'LOCATION 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
      value: selectedOptions({ location: '2' }),
    };

    const component = createFiltersComponent(props);

    expect(component.find('#location').findAll('option').at(1).attributes().value).toEqual('2');
  });

  it('will select correct option for clinician drop-down', () => {
    const props = {
      options: {
        clinicians: [
          { value: '1', name: 'CLINICIAN 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
      value: selectedOptions({ clinician: '2' }),
    };

    const component = createFiltersComponent(props);

    expect(component.find('#clinician').findAll('option').at(1).attributes().value).toEqual('2');
  });

  it('will select correct option for date drop-down', () => {
    const props = {
      options: {
        dates: [
          { value: '1', name: 'DATE 1', translate: false },
          { value: '2', name: 'appointments.book.today', translate: true },
        ],
      },
      value: selectedOptions({ date: '2' }),
    };

    const component = createFiltersComponent(props);

    expect(component.find('#time-period').findAll('option').at(1).attributes().value).toEqual('2');
  });
});
