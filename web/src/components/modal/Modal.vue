<template>
  <div ref="modalContainer" tabindex="-1">
    <div v-if="isVisible" ref="nhsModalRoot"
         :class="$style.container"
         role="dialog"
         aria-modal="true">
      <div :class="$style['reveal-modal']" :style="`max-width: ${maxWidth};`">
        <component :is="modalContent"/>
      </div>
    </div>
  </div>
</template>

<script>
import createFocusTrap from 'focus-trap/index';
import { getModalContent } from '@/components/modal/modalRegister';

export default {
  name: 'Modal',
  components: {},
  computed: {
    /**
       * Note: Only visible in desktop mode.
       */
    isVisible() {
      return !this.$store.state.device.isNativeApp
          && !!this.$store.state.modal.config.visible;
    },
    modalContent() {
      return getModalContent(this.$store.state.modal.config.content);
    },
    /**
       * User specified width - default to 400px
       * @return {*|string} width
       */
    maxWidth() {
      return this.$store.state.modal.config.maxWidth || '400px';
    },
  },
  mounted() {
    if (!this.$store.state.device.isNativeApp) {
      /**
       * Establishes the focus trap after the vue renderer has settled down.
       */
      this.$nextTick(() => {
        /**
         * This creates a focus trap to ensure that the user
         * cannot leave the modal window using the tab key
         */
        const focusTrap = createFocusTrap(this.$refs.modalContainer, {
          escapeDeactivates: false,
        });

        this.$store.subscribe((mutation) => {
          switch (mutation.type) {
            case 'modal/SHOW_MODAL':
              if (this.$store.state.modal.config.content) {
                this.$nextTick(() => {
                  focusTrap.activate();
                });
              }
              break;
            case 'modal/HIDE_MODAL':
              if (this.$store.state.modal.config.content) {
                this.$nextTick(() => {
                  focusTrap.deactivate();
                  this.$store.dispatch('modal/destroy');
                });
              }
              break;
            default:
              break;
          }
        });
      });
    }
  },
};
</script>

<style module lang="scss" scoped>
  .container {
    width: 100%;
    position: fixed;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    background-color: rgba(255, 255, 255, 0.5);
    z-index: 9999;
  }

  .reveal-modal {
    min-width: 250px;
    width: 80%;
    background: #ffffff;
    margin: 0 auto;
    position: relative;
    z-index: 99999;
    top: 25%;
    padding: 30px;
    -webkit-box-shadow: 0 0 10px rgba(0, 0, 0, 0.4);
    -moz-box-shadow: 0 0 10px rgba(0, 0, 0, 0.4);
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.4);
  }
</style>
