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
        <message-text :is-before-footer="true" :aria-label="messageLabel"
                      data-purpose="msg-text">
          {{ messageText }}
        </message-text>
        <message-text v-if="hasAdditionalInfo" :aria-label="additionalInfoLabel"
                      :class="$style.additionalInfomation"
                      data-purpose="msg-extratext">
          {{ additionalInfoText }}
        </message-text>
        <component :is="additionalInfoComponentName" v-if="additionalInfoComponentName"
                   :class="$style.additionalInfomation" />
      </message-dialog>
      <form :action="retryUrl" method="get">
        <generic-button v-if="retryButtonText" :class="buttonClasses"
                        data-purpose="retry-or-back-button"
                        @click.stop.prevent="onRetryButtonClicked">
          {{ retryButtonText }}
        </generic-button>
      </form>
    </div>
    <div v-else>
      <header-slim>{{ header }}</header-slim>
      <div :class="$style['information-error']">
        <h2>{{ subheader }}</h2>
        <p :aria-label="messageLabel">{{ messageText }}</p>
      </div>
    </div>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import isObject from 'lodash/fp/isObject';
import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import GenericButton from '@/components/widgets/GenericButton';
import HeaderSlim from '@/components/HeaderSlim';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

const getMappedValue = ({ map, statusCode, errorCode }) => {
  if (!map) {
    return '';
  }

  return (errorCode && map[errorCode])
    || map[statusCode]
    || map.default;
};

export default {
  components: {
    ContactOrganDonation,
    GenericButton,
    HeaderSlim,
    MessageDialog,
    MessageText,
  },
  mixins: [ErrorMessageMixin],
  computed: {
    additionalInfo() {
      return this.getMessage('additionalInfo');
    },
    additionalInfoComponentName() {
      return this.$store.state.errors.pageSettings.additionalInfoComponent;
    },
    additionalInfoLabel() {
      return isObject(this.additionalInfo) ? this.additionalInfo.label : undefined;
    },
    additionalInfoText() {
      return isObject(this.additionalInfo) ? this.additionalInfo.text : this.additionalInfo;
    },
    buttonClasses() {
      const clazzes = [this.$style.button];
      const url = this.retryUrl;
      if (url && url.length > 0) {
        clazzes.push(this.$style.grey);
      }
      return clazzes;
    },
    hasAdditionalInfo() {
      return this.getMessage('additionalInfo') !== '';
    },
    header() {
      const headerMessage = this.getMessage('header');
      this.trackSystemError(headerMessage);
      return headerMessage;
    },
    isStandardError() {
      return this.$store.getters['errors/isStandardError'];
    },
    isVisible() {
      return this.showError();
    },
    message() {
      return this.getMessage('message');
    },
    messageLabel() {
      return isObject(this.message) ? this.message.label : undefined;
    },
    messageText() {
      return isObject(this.message) ? this.message.text : this.message;
    },
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    pageHeader() {
      return this.getMessage('pageHeader');
    },
    pageTitle() {
      return this.getMessage('pageTitle') || this.pageHeader;
    },
    retryAction() {
      return getMappedValue({
        map: this.$store.state.errors.pageSettings.action,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      });
    },
    retryButtonText() {
      return this.getComponentErrorCodeKey('retryButtonText')
        || this.getComponentKey('retryButtonText', 'errors');
    },
    retryUrl() {
      const url = getMappedValue({
        map: this.$store.state.errors.pageSettings.redirectUrl,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      });

      return this.correctUrl(url);
    },
    subheader() {
      return this.getMessage('subheader');
    },
  },
  updated() {
    if (this.showError()) {
      this.updatePageTitle();
      this.updatePageHeader();
      window.scrollTo(0, 0);
    }
  },
  methods: {
    onRetryButtonClicked() {
      if (this.retryAction) {
        this.$store.dispatch(this.retryAction);
        return;
      }
      this.goToUrl(this.retryUrl);
    },
    showError() {
      return this.hasApiError;
    },
    trackSystemError(message) {
      const errorMessage = {
        type: 'system_error',
        messages: [message],
      };
      this.$store.dispatch('analytics/trackError', errorMessage);
    },
    updatePageHeader() {
      this.$store.dispatch('header/updateHeaderText', this.pageHeader);
    },
    updatePageTitle() {
      this.$store.dispatch('pageTitle/updatePageTitle', this.pageTitle);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';

  .additionalInfomation {
     margin-bottom: 1em
  }

  .information-error {
    p {
      margin-top: 0.5em
    }
  }
</style>
