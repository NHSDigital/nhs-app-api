<template>
  <div :class="[mType, isDesktopWeb ? $style.desktopWeb : undefined]">
    <div v-if="showIcon" :class="$style.icon">
      {{ iText }}
    </div>
    <div :id="messageId" :class="$style['msg-content']"
         :data-purpose="messageType">
      <slot/>
    </div>
  </div>
</template>

<script>

export default {
  props: {
    iconText: {
      type: String,
      default: undefined,
    },
    messageType: {
      type: String,
      default: 'error',
      validator: value => ['success', 'warning', 'error'].indexOf(value) !== -1,
    },
    messageId: {
      type: String,
      default: undefined,
    },
    overrideStyle: {
      type: String,
      default: 'none',
      validator: value => ['none', 'plain'].indexOf(value) !== -1,
    },
  },
  data() {
    return {
      defaultIconTexts: {
        success: 'Success',
        warning: 'Warning',
        error: 'Error',
      },
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
  },
  computed: {
    iText() {
      return this.iconText ? this.iconText : this.defaultIconTexts[this.messageType];
    },
    mType() {
      return this.showIcon ? [this.$style.msg, this.$style[this.messageType]] : [];
    },
    showIcon() {
      return this.overrideStyle !== 'plain';
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/messages";

</style>
