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
        <message-text :is-before-footer="true" data-purpose="msg-text">
          {{ message }}
        </message-text>
        <message-text v-if="hasAdditionalInfo" :aria-label="additionalInfoLabel"
                      :class="$style['additionalInfomation']"
                      data-purpose="msg-extratext">
          {{ additionalInfo }}
        </message-text >
      </message-dialog>
      <form :action="retryUrl" method="get">
        <generic-button v-if="retryButtonText" :class="buttonClasses"
                        data-purpose="retry-or-back-button" @click="onRetryButtonClicked($event)">
          {{ retryButtonText }}
        </generic-button>
      </form>
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
import GenericButton from '@/components/widgets/GenericButton';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';

export default {
  components: {
    MessageDialog,
    MessageText,
    HeaderSlim,
    GenericButton,
  },
  mixins: [ErrorMessageMixin],
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
    additionalInfoLabel() {
      return this.getMessage('additionalInfoLabel');
    },
    hasAdditionalInfo() {
      return this.getMessage('additionalInfo') !== '';
    },
    retryButtonText() {
      if (this.hasComponentErrorCodeKey('retryButtonText') || this.hasComponentKey('retryButtonText', 'errors')) {
        return this.getMessage('retryButtonText');
      }
      return '';
    },
    retryUrl() {
      const url = this.getRedirectUrl();
      return this.correctUrl(url);
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
      return this.hasApiError();
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
    onRetryButtonClicked(event) {
      event.preventDefault();
      this.goToUrl(this.retryUrl);
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
      p {
        margin-top: 0.5em
      }
  }
    .additionalInfomation {
     margin-bottom: 1em
  }
</style>
