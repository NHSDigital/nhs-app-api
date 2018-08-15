<template>
  <div v-if="isVisible" :id="$style.serverError" class="pull-content">
    <message-dialog message-type="error">
      <message-text :is-header="true">{{ header }}</message-text>
      <message-text>{{ subheader }}</message-text>
      <message-text>{{ message }}</message-text>
      <message-text v-if="hasAdditionalInfo">
        {{ additionalInfo }}
      </message-text >
    </message-dialog>
    <button v-if="retryButtonText" :class="buttonClasses" @click="onRetryButtonClicked">
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
    additionalInfo() {
      return this.getMessage('additionalInfo');
    },
    hasAdditionalInfo() {
      return this.getMessage('additionalInfo') !== '';
    },
    retryButtonText() {
      if (this.hasComponentErrorCodeKey('retryButtonText') || this.hasComponentKey('retryButtonText')) {
        return this.getMessage('retryButtonText');
      }
      return '';
    },
    buttonClasses() {
      const clazzes = [this.$style.button];
      const url = this.getRedirectUrl();
      if (url && url.length > 0) {
        clazzes.push(this.$style.grey);
      }
      return clazzes;
    },
  },
  updated() {
    this.setPageHeader();
  },
  methods: {
    showError() {
      return this.$store.getters['errors/showApiError'];
    },
    onRetryButtonClicked() {
      const url = this.getRedirectUrl();
      if (url === '') {
        this.$router.go();
      } else {
        this.$router.push(url);
      }
    },
    getRedirectUrl() {
      return this.$store.state.errors.pageSettings.redirectUrl;
    },
    getApiErrorResponse() {
      return this.$store.state.errors.apiErrors[0];
    },
    getRoutePath() {
      return this.$store.state.errors.routePath.substring(1).replace(/\//g, '.').replace(/-/g, '_');
    },
    getComponentErrorCodeKey(type) {
      if (!this.showError()) {
        return '';
      }
      const component = this.getRoutePath();

      return `${component}.errors.${this.getApiErrorResponse().status}.${type}`;
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
      return `errors.${this.getApiErrorResponse().status}.${type}`;
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
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
</style>
