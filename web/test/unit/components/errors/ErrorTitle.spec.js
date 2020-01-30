import ErrorTitle from '@/components/errors/ErrorTitle';
import { createStore, mount } from '../../helpers';

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
    $store = createStore();
    mountWrapper({ $store, title: 'foo.title' });
  });

  it('will dispatch `header/updateHeaderText` with title', () => {
    expect($store.dispatch).toBeCalledWith('header/updateHeaderText', 'translate_foo.title');
  });

  it('will dispatch `pageTitle/updatePageTitle` with title', () => {
    expect($store.dispatch).toBeCalledWith('pageTitle/updatePageTitle', 'translate_foo.title');
  });

  describe('header', () => {
    beforeEach(() => {
      mountWrapper({ $store, header: 'foo.header', title: 'foo.title' });
    });

    it('will dispatch `header/updateHeaderText` with header', () => {
      expect($store.dispatch).toBeCalledWith('header/updateHeaderText', 'translate_foo.header');
    });

    it('will dispatch `pageTitle/updatePageTitle` with title', () => {
      expect($store.dispatch).toBeCalledWith('pageTitle/updatePageTitle', 'translate_foo.title');
    });
  });
});
