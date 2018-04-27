<template>
  <div>
    <error-warning-icon :iconType="errorOrWarning"/>
    <div :class="[msgClass, errorOrWarning]">
      <slot/>
    </div>
  </div>
</template>

<script>
import ErrorWarningIcon from '@/components/icons/ErrorWarningIcon';

export default {
  props: {
    errorOrWarning: {
      default: 'error',
      type: String,
    },
  },
  components: {
    ErrorWarningIcon,
  },
  data() {
    return {
      msgClass: 'msg',
    };
  },
};
</script>

<style lang="scss">
  @import '../style/messages';

</style>
