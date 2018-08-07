<template>
  <div>
    <error-warning-icon :icon-type="errorOrWarning" aria-hidden="true"/>
    <div :class="[msgClass, errorOrWarning]" :id="errorWarningId" >
      <slot/>
    </div>
  </div>
</template>

<script>
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
