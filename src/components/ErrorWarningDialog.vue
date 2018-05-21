<template>
  <div>
    <error-warning-icon :icon-type="errorOrWarning"/>
    <div :class="[msgClass, errorOrWarning]">
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
      default: 'error',
      type: String,
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
@import "../style/messages";
</style>
