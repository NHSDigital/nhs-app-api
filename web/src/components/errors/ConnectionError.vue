<template>
  <div v-if="isVisible" class="pull-content">
    <message-dialog message-type="error">
      <message-text :is-header="true">{{ header }}</message-text>
      <message-text>{{ subheader }}</message-text>
      <message-text>{{ message }}</message-text>
    </message-dialog>
    <button :class="$style.button" @click="onRetryButtonClicked">
      {{ retryButtonText }}
    </button>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

export default {
  components: {
    MessageDialog,
    MessageText,
  },
  computed: {
    isVisible() {
      return this.$store.state.errors.hasConnectionProblem;
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
    this.$store.dispatch('errors/setConnectionProblem', !navigator.onLine);
  },
  methods: {
    onRetryButtonClicked() {
      this.$router.go();
    },
    getRoutePath() {
      return this.$route.path.substring(1).replace('/', '.').replace('-', '_');
    },
    getComponentKey(type) {
      const component = this.getRoutePath();
      return `${component}.noConnection.${type}`;
    },
    existsComponentKey(type) {
      return this.$te(this.getComponentKey(type));
    },
    getMessage(type) {
      if (this.existsComponentKey(type)) {
        return this.$t(this.getComponentKey(type));
      } else if (this.$te(`noConnection.${type}`)) {
        return this.$t(`noConnection.${type}`);
      }
      return '';
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';

</style>
