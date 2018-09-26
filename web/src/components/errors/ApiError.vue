<template>
  <div v-if="isVisible">
    <div v-if="isStandardError" :id="$style.serverError" class="pull-content">
      <message-dialog :override-style="overrideStyle" message-type="error">
        <message-text :is-header="true" :override-style="overrideStyle" data-purpose="msg-header">
          {{ header }}
        </message-text>
        <message-text v-if="subheader!==''" data-purpose="msg-subheader">
          {{ subheader }}
        </message-text>
        <message-text data-purpose="msg-text">
          {{ message }}
        </message-text>
        <message-text v-if="hasAdditionalInfo" data-purpose="msg-extratext">
          {{ additionalInfo }}
        </message-text >
      </message-dialog>
      <button v-if="retryButtonText" :class="buttonClasses"
              data-purpose="retry-or-back-button" @click="onRetryButtonClicked">
        {{ retryButtonText }}
      </button>
    </div>
    <div v-else>
      <header-slim>{{ header }}</header-slim>
      <div :class="$style['information-error']">
        <h2>{{ subheader }}</h2>
        <p>{{ message }}</p>
      </div>
    </div>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import HeaderSlim from '@/components/HeaderSlim';

export default {
  components: {
    MessageDialog,
    MessageText,
    HeaderSlim,
  },
  computed: {
    isVisible() {
      return this.showError();
    },
    isStandardError() {
      return this.showStandardErrorView();
    },
    header() {
      const headerMessage = this.getMessage('header');
      this.trackSystemError(headerMessage);
      return headerMessage;
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
    overrideStyle() {
      const overrideStyles = this.$store.state.errors.pageSettings.errorOverrideStyles;
      const overrideStyle = overrideStyles[this.getApiErrorResponse().status];
      return overrideStyle;
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
    if (this.showError()) {
      this.updatePageTitle();
      this.updatePageHeader();
    }
  },
  methods: {
    showError() {
      return this.$store.getters['errors/showApiError'];
    },
    showStandardErrorView() {
      return this.$store.getters['errors/isStandardError'];
    },
    trackSystemError(message) {
      const errorMessage = {
        type: 'system_error',
        messages: [message],
      };
      this.$store.dispatch('analytics/trackError', errorMessage);
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
      const errorCode = this.getApiErrorResponse().status;
      const redirectUrlMap = this.$store.state.errors.pageSettings.redirectUrl;
      if (!redirectUrlMap) {
        return '';
      }
      let redirectUrl = redirectUrlMap[errorCode];
      if (!redirectUrl) {
        redirectUrl = redirectUrlMap.default;
      }
      return redirectUrl;
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
    getPageHeader() {
      return this.getMessage('pageHeader');
    },
    getPageTitle() {
      let pageTitle = this.getMessage('pageTitle');

      if (!pageTitle || pageTitle === '') {
        pageTitle = this.getPageHeader();
      }

      return pageTitle;
    },
    updatePageTitle() {
      this.$store.dispatch('pageTitle/updatePageTitle', this.getPageTitle());
    },
    updatePageHeader() {
      this.$store.dispatch('header/updateHeaderText', this.getPageHeader());
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';

  .information-error {
      margin-top: -2em;
      p {
        margin-top: 0.5em
      }
  }
</style>
