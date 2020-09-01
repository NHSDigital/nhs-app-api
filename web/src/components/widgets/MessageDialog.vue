<template>
  <div id="message-dialog" data-purpose="error-container"
       :class="[mType,
                ...extraClasses,
                'nhsuk-width-container--full',
                {[$style.desktopWeb] : !$store.state.device.isNativeApp},
                plainStyle]">
    <h2 v-if="showIcon" :class="['nhsuk-heading-m', $style.icon]">
      {{ iText }}
    </h2>
    <div :id="messageId" :class="$style['msg-content']"
         :data-purpose="messageType">
      <slot/>
    </div>
  </div>
</template>

<script>

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
  },
  mounted() {
    if (this.focusable) {
      this.setScreenReaderFocus();
    }
  },
  methods: {
    setScreenReaderFocus() {
      const element = document.getElementById('message-dialog');

      if (!element) {
        return;
      }

      element.setAttribute('tabindex', '-1');
      element.blur();
      element.focus();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/messages";
@import "../../style/spacings";

.plain {
  &.desktopWeb {
    margin-left: 0-$three;
  }
}
</style>
