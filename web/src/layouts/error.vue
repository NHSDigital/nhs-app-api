<template>
  <div v-if="error.statusCode === 404" :id="$style.serverError" class="pull-content">
    <message-dialog :override-style="overrideStyle" message-type="error">
      <message-text :is-header="true" :override-style="overrideStyle" data-purpose="msg-header">
        {{ $t('errors.404.header') }}
      </message-text>
      <message-text data-purpose="msg-subheader">
        {{ $t('errors.404.subheader') }}
      </message-text>
      <message-text :is-before-footer="true" data-purpose="msg-text">
        {{ $t('errors.404.message') }}
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
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
  layout: 'default',
  computed: {
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../style/buttons';
</style>
