<template>
  <div v-if="isVisible" class="pull-content">
    <message-dialog message-type="error">
      <message-text :is-header="true">{{ header }}</message-text>
      <message-text>{{ subheader }}</message-text>
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <form method="get">
      <generic-button :class="$style.button" @click.prevent="onRetryButtonClicked">
        {{ retryButtonText }}
      </generic-button>
    </form>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import GenericButton from '@/components/widgets/GenericButton';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';

export default {
  components: {
    MessageDialog,
    MessageText,
    GenericButton,
  },
  mixins: [ErrorMessageMixin],
  computed: {
    isVisible() {
      return this.showError();
    },
    header() {
      return this.getMessage('header');
    },
    subheader() {
      return this.getMessage('subheader');
    },
    message() {
      return this.getMessage('message');
    },
    retryButtonText() {
      return this.getMessage('retryButtonText');
    },
  },
  updated() {
    this.$store.dispatch('errors/setConnectionProblem', process.client && !navigator.onLine);
    this.$store.dispatch('header/updateHeaderText', this.getMessage('header'));
    this.$store.dispatch('pageTitle/updatePageTitle', this.getMessage('header'));
  },
  methods: {
    onRetryButtonClicked() {
      this.$router.go();
    },
    showError() {
      return this.hasConnectionError;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';

</style>
