<template>
  <div :id="$style.serverError" class="pull-content">
    <message-dialog :override-style="overrideStyle" message-type="error">
      <message-text :is-header="true" :override-style="overrideStyle" data-purpose="msg-header">
        {{ $t('generic.errors.pageNotFound') }}
      </message-text>
      <message-text data-purpose="msg-subheader">
        {{ $t('generic.errors.ifYouEnteredCheckItWasCorrect') }}
      </message-text>
      <message-text :is-before-footer="true"
                    :aria-label="$t('generic.errors.youCanGoDirectlyToOrOneOneOne')"
                    data-purpose="msg-text">
        {{ $t('generic.errors.youCanGoDirectlyToOr111') }}
        <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
           style="display:inline">
          {{ $t('generic.nhs111Link') }}
        </a>
        {{ $t('generic.orCall') }}
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
