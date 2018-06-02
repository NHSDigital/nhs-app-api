<template>
  <main v-show="isVisible" class="content">
    <error-warning-dialog error-or-warning="error">
      <p class="header">
        {{ header }}
      </p>
      <p>
        {{ subheader }}
      </p>
    </error-warning-dialog>
    <button class="button" @click="onRetryButtonClicked">
      {{ retryButtonText }}
    </button>
    <p>
      {{ message }}
    </p>
  </main>
</template>
<script>
/* eslint-disable import/extensions */
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  components: {
    ErrorWarningDialog,
  },
  data() {
    return {
      noConnection: false,
    };
  },
  computed: {
    isVisible() {
      return this.noConnection;
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
    this.noConnection = !navigator.onLine;
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

<style lang="scss">
  @import '../../style/html';
  @import '../../style/elements';
  @import '../../style/buttons';

</style>
