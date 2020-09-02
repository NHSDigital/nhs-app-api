<template>
  <div data-purpose="error-container"
       :class="[mType,
                ...extraClasses,
                'nhsuk-width-container--full',
                {[$style.desktopWeb] : !$store.state.device.isNativeApp},
                plainStyle]">
    <h2 v-if="showIcon" :class="[nhsHeaderStyle, $style.icon]">
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
  data() {
    return {
      defaultIconTexts: {
        success: this.$t('generic.success'),
        warning: this.$t('generic.warning'),
        error: this.$t('generic.error'),
        message: this.$t('generic.message'),
      },
    };
  },
  computed: {
    iText() {
      return this.iconText ? this.iconText : this.defaultIconTexts[this.messageType];
    },
    mType() {
      return this.showIcon ? [this.$style.msg, this.$style[this.messageType]] : [];
    },
    plainStyle() {
      return this.overrideStyle === 'plain' ? this.$style.plain : '';
    },
    nhsHeaderStyle() {
      return 'nhsuk-heading-m';
    },
    showIcon() {
      return this.overrideStyle !== 'plain';
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
