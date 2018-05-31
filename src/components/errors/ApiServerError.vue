<template>
  <main class="content" v-show="isVisible">
    <error-warning-dialog errorOrWarning="error">
      <p class="header">
        {{header}}
      </p>
      <p>
        {{subheader}}
      </p>
      <p>
        {{message}}
      </p>
    </error-warning-dialog>
    <button class="button" v-if="retryButtonText" @click="onRetryButtonClicked">
      {{retryButtonText}}
    </button>
  </main>
</template>
<script>

import { mapGetters } from 'vuex';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  components: {
    ErrorWarningDialog,
  },
  data() {
    return {
      connection: true,
    };
  },
  updated() {
    this.connection = navigator.onLine;
    this.setPageHeader();
  },
  methods: {
    showError() {
      return this.hasApiServerErrorResponse() && this.connection;
    },
    onRetryButtonClicked() {
      this.$router.go();
    },
    getApiErrorResponse() {
      return this.$store.state.http.apiErrorResponse;
    },
    hasApiServerErrorResponse() {
      const response = this.$store.state.http.apiErrorResponse;
      return (response && response.status >= 500);
    },
    getRoutePath() {
      return this.$route.path.substring(1).replace('/', '.').replace('-', '_');
    },
    getComponentErrorCodeKey(type) {
      if (!this.showError()) {
        return '';
      }
      const component = this.getRoutePath();
      return `${component}.errors.${this.getApiErrorResponse.status}.${type}`;
    },
    hasComponentErrorCodeKey(type) {
      return this.$te(this.getComponentErrorCodeKey(type));
    },
    getComponentKey(type) {
      if (!this.showError()) {
        return '';
      }
      const component = this.getRoutePath();
      return `${component}.errors.${type}`;
    },
    hasComponentKey(type) {
      return this.$te(this.getComponentKey(type));
    },
    getDefaultErrorCodeKey(type) {
      if (!this.showError()) {
        return '';
      }
      return `errors.${this.getApiErrorResponse.status}.${type}`;
    },
    hasDefaultErrorCodeKey(type) {
      return this.$te(this.getDefaultErrorCodeKey(type));
    },
    getMessage(type) {
      if (this.hasComponentErrorCodeKey(type)) {
        return this.$t(this.getComponentErrorCodeKey(type));
      } else if (this.hasComponentKey(type)) {
        return this.$t(this.getComponentKey(type));
      } else if (this.hasDefaultErrorCodeKey(type)) {
        return this.$t(this.getDefaultErrorCodeKey(type));
      } else if (this.$te(`errors.${type}`)) {
        return this.$t(`errors.${type}`);
      }
      return '';
    },
    setPageHeader() {
      if (this.showError()) {
        const pageHeader = this.getMessage('pageHeader');
        this.$store.dispatch('header/updateHeaderText', pageHeader);
      }
    },
  },
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
    showRetryButton() {
      return this.hasComponentErrorCodeKey('retryButtonText');
    },
    retryButtonText() {
      if (this.hasComponentErrorCodeKey('retryButtonText')) {
        return this.getMessage('retryButtonText');
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
