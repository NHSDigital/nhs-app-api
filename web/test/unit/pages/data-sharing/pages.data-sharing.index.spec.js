/* eslint-disable import/no-extraneous-dependencies */
import DataSharing from '@/pages/data-sharing/';
import { create$T, createStore, mount } from '../../helpers';
import { initialState } from '@/store/modules/navigation/mutation-types';

const $t = create$T();

const createState = () => ({
  device: {
    source: 'web',
  },
  navigation: initialState(),
});

describe('data sharing index', () => {
  let button;
  let getNdopToken;
  let wrapper;
  let insetText;

  const findInsetText = () => wrapper.find('.nhsuk-inset-text');

  beforeEach(() => {
    const $store = createStore({ state: createState() });
    getNdopToken = jest.fn();

    wrapper = mount(DataSharing, {
      data() {
        return {
          pageIds: ['p1', 'p2', 'p3', 'p4'],
        };
      },
      t: (key) => {
        switch (key) {
          case 'ds01.pages.p4.paragraphs':
          case 'ds01.pages.p2.intro.paragraphs':
          case 'ds01.pages.p2.intro.listItems':
          case 'ds01.pages.p2.thoseWhoCant.listItems':
          case 'ds01.pages.p2.dataProtection.paragraphs':
          case 'ds01.pages.p2.dataProtection.listItems':
          case 'ds01.pages.p3.paragraphs':
          case 'ds01.pages.p3.listItems':
          case 'ds01.pages.p1.confidential.listItems':
          case 'ds01.pages.p1.patientInformation.researchAndPlanningListItems':
          case 'ds01.pages.ndop.paragraphs':
            return ['paragraph 1', 'paragraph 2'];
          default:
            return $t(key);
        }
      },
      $store,
    });
    wrapper.vm.getNdopToken = getNdopToken;
    button = wrapper.find('#next-button');
  });

  it('will retrieve the token on the last on the page', () => {
    for (let i = 0; i <= 3; i += 1) {
      button.trigger('click');
      wrapper.setData({ pageIndex: i });

      if (i === 3) {
        expect(getNdopToken).toHaveBeenCalled();
      } else {
        expect(getNdopToken).not.toHaveBeenCalled();
      }
    }
  });

  it('will contain inset text', () => {
    for (let i = 0; i <= 3; i += 1) {
      wrapper.setData({ pageIndex: i });
      insetText = findInsetText();

      if (i === 0 || i === 3) {
        expect(insetText.exists()).toBe(true);
      }
    }
  });

  it('will not contain inset text', () => {
    for (let i = 0; i <= 3; i += 1) {
      wrapper.setData({ pageIndex: i });
      insetText = findInsetText();

      if (i === 1 || i === 2) {
        expect(insetText.exists()).toBe(false);
      }
    }
  });
});
