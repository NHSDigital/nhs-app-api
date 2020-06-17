<template>
  <div :id="$style.serverError" class="pull-content">
    <message-dialog :override-style="overrideStyle" message-type="error">
      <message-text :is-header="true" :override-style="overrideStyle" data-purpose="msg-header">
        {{ $t('errors.404.header') }}
      </message-text>
      <message-text data-purpose="msg-subheader">
        {{ $t('errors.404.subheader') }}
      </message-text>
      <message-text :is-before-footer="true" :aria-label="$t('errors.404.message.label')"
                    data-purpose="msg-text">
        {{ $t('errors.404.message.text') }}
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'NotFoundPage',
  components: {
    MessageDialog,
    MessageText,
  },
  props: {
    error: {
      type: Object,
      default() {
        return {};
      },
    },
  },
  computed: {
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
  },
};
</script>
<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
</style>
<style module lang="scss" scoped>
  @import '../style/buttons';
</style>
