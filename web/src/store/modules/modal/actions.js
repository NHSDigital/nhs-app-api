import { SHOW_MODAL, HIDE_MODAL, DESTROY_MODAL } from './mutation-types';

export default {
  /**
   * Show modal based on supplied config.
   * @param commit
   * @param config - should follow the structure defined below:
   * {
   *   visible: <true | false>
   *   content: <VueComponentBody>
   *   width: override as string to define width e.g. '500px' or '80%' or '2em'
   * }
   */
  show({ commit }, config) {
    if (!config || !config.content) {
      throw new Error('Modal config is required for modal display.');
    }

    const contentName = config.content.name ? config.content.name : config.content;
    commit(SHOW_MODAL, { ...config, content: contentName });
  },

  /**
   * Deactivates the focus trap ready for modal destroy.
   * @param commit
   */
  hide({ commit }) {
    commit(HIDE_MODAL);
  },

  /**
   * Destroys modal window from the dom.
   * @param commit
   */
  destroy({ commit }) {
    commit(DESTROY_MODAL);
  },

  init({ commit }) {
    commit(HIDE_MODAL);
  },
};
