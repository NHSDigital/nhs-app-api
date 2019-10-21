import { INDEX, APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';
import { mount } from '../../helpers';
import BreadCrumbTrail from '../../../../src/components/widgets/BreadCrumbTrail';

describe('BreadCrumbTrail.vue', () => {
  const createBreadCrumbTrail = (
    {
      $store = {
        state: {
          device: {
            isNativeApp: false,
          },
          session: {
            csrfToken: 'some token',
          },
        },
      },
      propsData,
    } = {},
  ) =>
    mount(BreadCrumbTrail, {
      $store,
      $style: {
        native: 'native',
      },
      propsData,
      stubs: { 'nuxt-link': '<a></a>' },
    });


  it('will not render a breadcrumb as there are no items to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [],
      },
    });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will not render a breadcrumb as the user is not logged in.', () => {
    const wrapper = createBreadCrumbTrail({
      $store: {
        state: {
          session: {
            csrfToken: undefined,
          },
        },
      },
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will render a single breadcrumb item to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`p>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);
  });

  it('will return a multiple breadcrumbs item to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [INDEX, APPOINTMENT_BOOKING_GUIDANCE],
      },
    });

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${APPOINTMENT_BOOKING_GUIDANCE.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`p>a[to='${APPOINTMENT_BOOKING_GUIDANCE.path}']`)
      .exists()).toEqual(true);
  });

  it('will add native class when source is native', () => {
    const wrapper = createBreadCrumbTrail({
      $store: {
        state: {
          device: {
            isNativeApp: true,
          },
          session: {
            csrfToken: 'some token',
          },
        },
      },
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find('a.nhsuk-breadcrumb__backlink.native').exists()).toEqual(true);
  });

  it('will not add native class when source is not native', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find('a.nhsuk-breadcrumb__backlink.native').exists()).toEqual(false);
  });
});
