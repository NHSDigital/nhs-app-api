<template>
  <div id="message-dialog" ref="messageDialogContainer" data-purpose="error-container"
       :class="[mType,
                ...extraClasses,
                'nhsuk-width-container--full',
                {[$style.desktopWeb] : !$store.state.device.isNativeApp},
                plainStyle]">
    <h2 v-if="showIcon"
        :class="['nhsuk-heading-m', $style.icon, $style.break]"
        :aria-label="headingAriaLabel">
      {{ iText }}
    </h2>
    <div :id="messageId" :class="$style['msg-content']"
         :data-purpose="messageType">
      <slot/>
    </div>
  </div>
</template>

<script>
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';

export default {
  name: 'MessageDialog',
  props: {
    focusable: {
      type: Boolean,
      default: false,
    },
    iconText: {
      type: String,
      default: undefined,
    },
    messageType: {
      type: String,
      default: 'error',
      validator: value => ['success', 'warning', 'error', 'message'].indexOf(value) !== -1,
    },
    messageId: {
      type: String,
      default: undefined,
    },
    extraClasses: {
      type: Array,
      default: () => [],
    },
    overrideStyle: {
      type: String,
      default: 'none',
      validator: value => ['none', 'plain'].indexOf(value) !== -1,
    },
  },
  computed: {
    iText() {
      return this.iconText ? this.iconText : this.$t(`generic.${this.messageType}`);
    },
    mType() {
      return this.showIcon ? [this.$style.msg, this.$style[this.messageType]] : [];
    },
    plainStyle() {
      return this.overrideStyle === 'plain' ? this.$style.plain : '';
    },
    showIcon() {
      return this.overrideStyle !== 'plain';
    },
    headingAriaLabel() {
      return this.messageType === 'warning' && this.iText.toLowerCase().indexOf('important') < 0 ? `Important: ${this.iText}` : undefined;
    },
  },
  beforeMount() {
    EventBus.$on(FOCUS_ERROR_ELEMENT, this.scrollToTopAndFocusDialog);
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_ERROR_ELEMENT, this.scrollToTopAndFocusDialog);
  },
  mounted() {
    this.focusDialog();
  },
  methods: {
    scrollToTopAndFocusDialog() {
      window.scrollTo(0, 0);
      this.focusDialog();
    },
    focusDialog() {
      if (this.focusable) {
        this.$refs.messageDialogContainer.setAttribute('tabindex', '-1');
        this.$refs.messageDialogContainer.focus();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/message-dialog";
</style>
