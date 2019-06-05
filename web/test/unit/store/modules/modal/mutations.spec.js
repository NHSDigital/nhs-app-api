import mutations from '@/store/modules/modal/mutations';

const {
  SHOW_MODAL,
  HIDE_MODAL,
  DESTROY_MODAL,
} = mutations;

describe('SHOW_MODAL, HIDE_MODAL', () => {
  it('should set the modal config state to visible', () => {
    const state = {
      config: { visible: false },
    };
    SHOW_MODAL(state, { content: { name: 'SomeComponentName' } });
    expect(state.config).toEqual({
      visible: true,
      content: { name: 'SomeComponentName' },
    });
  });

  it('should set the modal config state untouched', () => {
    const state = {
      config: {
        content: { name: 'SomeComponentName' },
        visible: true,
      },
    };
    HIDE_MODAL(state);
    expect(state.config).toEqual({
      visible: true,
      content: { name: 'SomeComponentName' },
    });
  });

  it('should set the modal config state to not visible', () => {
    const state = {
      config: {
        content: { name: 'SomeComponentName' },
        visible: true,
      },
    };
    DESTROY_MODAL(state);
    expect(state.config).toEqual({
      visible: false,
    });
  });
});
