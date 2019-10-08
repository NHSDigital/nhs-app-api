import { mount } from '../../helpers';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';


describe('AnalyticsTrackedTag.vue', () => {
  let tag;
  let mockClickFunc;
  let mockPreventDefaultFunc;

  const createAnalyticsTrackedTag = (propsData) => {
    const wrapper = mount(AnalyticsTrackedTag, {
      parentComponent: {},
      propsData,
    });
    wrapper.vm.$parent.$vnode = { tag: '' };
    global.digitalData = {};
    return wrapper;
  };

  beforeEach(() => {
    mockClickFunc = jest.fn();
    mockPreventDefaultFunc = jest.fn();
  });

  describe('on click function specified', () => {
    let propsData;
    it('will execute click function when clicked', () => {
      propsData = {
        clickFunc: mockClickFunc,
        text: '',
      };
      tag = createAnalyticsTrackedTag(propsData);
      tag.trigger('click');

      expect(mockClickFunc).toHaveBeenCalled();
    });

    it('will prevent default action by default', () => {
      propsData = {
        clickFunc: mockClickFunc,
        text: '',
      };
      tag = createAnalyticsTrackedTag(propsData);
      tag.trigger('click', {
        preventDefault: mockPreventDefaultFunc,
      });

      expect(mockClickFunc).toHaveBeenCalled();
      expect(mockPreventDefaultFunc).toHaveBeenCalled();
    });

    it('will not prevent default action when preventDefault specified false', () => {
      propsData = {
        clickFunc: mockClickFunc,
        text: '',
        preventDefault: false,
      };
      tag = createAnalyticsTrackedTag(propsData);
      tag.trigger('click', {
        preventDefault: mockPreventDefaultFunc,
      });

      expect(mockClickFunc).toHaveBeenCalled();
      expect(mockPreventDefaultFunc).not.toHaveBeenCalled();
    });

    it('will prevent default action when preventDefault specified true', () => {
      propsData = {
        clickFunc: mockClickFunc,
        text: '',
        preventDefault: true,
      };
      tag = createAnalyticsTrackedTag(propsData);
      tag.trigger('click', {
        preventDefault: mockPreventDefaultFunc,
      });

      expect(mockClickFunc).toHaveBeenCalled();
      expect(mockPreventDefaultFunc).toHaveBeenCalled();
    });
  });

  describe('on clickFunc not specified', () => {
    it('will not prevent default action', () => {
      tag = createAnalyticsTrackedTag({ text: '' });
      tag.trigger('click', {
        preventDefault: mockPreventDefaultFunc,
      });

      expect(mockPreventDefaultFunc).not.toHaveBeenCalled();
    });
  });
});
