<template>
  <div>
    <error-warning-icon :icon-type="errorOrWarning"/>
    <div :class="[msgClass, errorOrWarning]" :id="errorWarningId">
      <slot/>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import ErrorWarningIcon from '@/components/icons/ErrorWarningIcon';

export default {
  components: {
    ErrorWarningIcon,
  },
  props: {
    errorOrWarning: {
      type: String,
      default: 'error',
    },
    errorWarningId: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      msgClass: 'msg',
    };
  },
};
</script>

<style lang="scss">
@import "../../style/messages";
</style>
