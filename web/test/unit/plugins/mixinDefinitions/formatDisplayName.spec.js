import FormatDisplayName from '@/plugins/mixinDefinitions/FormatDisplayName';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../helpers';

describe('format display name mixin', () => {
  let $store;
  let component;
  let wrapper;
  let result;

  beforeEach(() => {
    $store = createStore();
    component = {
      template: '<div></div>',
      mixins: [FormatDisplayName],
    };
    wrapper = mount(component, { $store, mountOpts: { i18n } });
  });

  it('mixin returns empty string if input name is undefined', () => {
    // act
    result = wrapper.vm.getDisplayNameText(undefined);

    // assert
    expect(result).toBe('');
  });

  it('mixin returns empty string if input name is null', () => {
    // act
    result = wrapper.vm.getDisplayNameText(null);

    // assert
    expect(result).toBe('');
  });

  it('mixin returns empty string if input name is empty string', () => {
    // act
    result = wrapper.vm.getDisplayNameText('');

    // assert
    expect(result).toBe('');
  });

  it('mixin returns empty string if input name is whitespace only string', () => {
    // act
    result = wrapper.vm.getDisplayNameText(' ');

    // assert
    expect(result).toBe('');
  });

  it('mixin returns UPPERCASE string if input name is lowercase string', () => {
    // act
    result = wrapper.vm.getDisplayNameText('i was a lowercase name');

    // assert
    expect(result).toBe('I WAS A LOWERCASE NAME');
    expect($store.dispatch).not.toHaveBeenCalled();
  });

  it('mixin returns UPPERCASE string if input name is UPPERCASE string', () => {
    // act
    result = wrapper.vm.getDisplayNameText('I WAS AN UPPERCASE NAME');

    // assert
    expect(result).toBe('I WAS AN UPPERCASE NAME');
  });
});
