import { shallow } from '@vue/test-utils';
import SymptomBanner from '@/components/SymptomBanner';


describe('SymptonBanner.vue', () => {
  it('should display the correct header text', () => {
    const myMock = jest.fn();
    myMock.mockReturnValueOnce('How are you feeling right now?').mockReturnValueOnce('Symptom checker');

    const wrapper = shallow(SymptomBanner, {
      mocks: {
        $t: myMock,
        $config: { SYMPTOM_CHECKER_URL: 'SymptomChecker Test URL' },
      },
    });

    expect(wrapper.html()).toMatchSnapshot();
  });
});

