<template>
  <div :class="[$style.msg, mType]">
    <div :class="$style.icon">
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
  },
  data() {
    return {
      defaultIconTexts: {
        success: 'Success',
        warning: 'Warning',
        error: 'Error',
      },
    };
  },
  computed: {
    iText() {
      return this.iconText ? this.iconText : this.defaultIconTexts[this.messageType];
    },
    mType() {
      return this.$style[this.messageType];
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/messages";

</style>
