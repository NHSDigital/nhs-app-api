import { shallowMount } from '@vue/test-utils';
/* eslint-disable import/extensions */
import SymptonBanner from '../../../src/components/SymptonBanner';

describe('SymptonBanner.vue', () => {
  test('should display the correct header text', () => {
    const myMock = jest.fn();
    myMock
      .mockReturnValueOnce('How are you feeling right now?')
      .mockReturnValueOnce('Symptom checker');

    process.env.SYMPTOM_CHECKER_URL = 'https://111.nhs.uk';

    const wrapper = shallowMount(SymptonBanner, {
      mocks: {
        $t: myMock,
        process: {
          env: {
            SYMPTOM_CHECKER_URL: 'https://111.nhs.uk',
          },
        },
      },
    });
    expect(wrapper.html()).toMatchSnapshot();
  });
});
