import ErrorTitle from '@/components/errors/ErrorTitle';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

const mountWrapper = ({ $store, header, title }) => mount(ErrorTitle, {
  $store,
  propsData: {
    header,
    title,
  },
});

describe('ErrorTitle', () => {
  let $store;

  beforeEach(() => {
    EventBus.$emit.mockClear();
    $store = createStore();
    mountWrapper({ $store, title: 'foo.title' });
  });

  it('will emit UPDATE_HEADER on EventBus with title', () => {
    expect(EventBus.$emit).toBeCalledWith(UPDATE_HEADER, 'foo.title');
  });

  it('will emit UPDATE_TITLE on EventBus with title', () => {
    expect(EventBus.$emit).toBeCalledWith(UPDATE_TITLE, 'foo.title');
  });

  describe('header', () => {
    beforeEach(() => {
      mountWrapper({ $store, header: 'foo.header', title: 'foo.title' });
    });

    it('will emit UPDATE_HEADER on EventBus with header', () => {
      expect(EventBus.$emit).toBeCalledWith(UPDATE_HEADER, 'foo.header');
    });

    it('will emit UPDATE_TITLE on EventBus with title', () => {
      expect(EventBus.$emit).toBeCalledWith(UPDATE_TITLE, 'foo.title');
    });
  });
});
