<template>
  <div v-if="isVisible" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="isStandardError" :id="$style.serverError" class="pull-content">
      <message-dialog :override-style="overrideStyle" message-type="error" :aria-live="ariaLive">
        <message-text :is-header="true"
                      :unindent="isPlainNativeError"
                      :override-style="overrideStyle"
                      data-purpose="msg-header">
          {{ header }}
        </message-text>
        <message-text v-if="subheader!==''"
                      :unindent="isPlainNativeError"
                      data-purpose="msg-subheader">
          {{ subheader }}
        </message-text>
        <message-text :is-before-footer="true"
                      :unindent="isPlainNativeError"
                      :aria-label="messageLabel"
                      data-purpose="msg-text">
          {{ messageText }}
        </message-text>
        <message-text v-if="hasAdditionalInfo"
                      :unindent="isPlainNativeError"
                      :aria-label="additionalInfoLabel"
                      :class="$style.additionalInformation"
                      data-purpose="msg-extratext">
          {{ additionalInfoText }}
        </message-text>
        <component :is="additionalInfoComponentName" v-if="additionalInfoComponentName"
                   :class="$style.additionalInformation" />
        <message-text v-if="hasSessionReferenceCode">
          <report-a-problem :reference="hasSessionReferenceCode"/>
        </message-text>
      </message-dialog>
      <form v-if="retryButtonText && ($store.state.device.isNativeApp || retryAction)"
            ref="retryFormRef" :action="retryUrl" method="get" tabindex="-1">
        <generic-button
          :class="[
            ...dynamicStyle('nhs-button'),
            $style.retryButton,
            buttonClasses
          ]"
          data-purpose="retry-or-back-button"
          click-delay="medium"
          @click.stop.prevent="onRetryButtonClicked">
          {{ retryButtonText }}
        </generic-button>
      </form>
      <desktopGenericBackLink
        v-if="retryButtonText && !$store.state.device.isNativeApp && retryUrl"
        :path="retryUrl"
        :button-text="retryButtonText"
        data-purpose="retry-or-back-button"
        @clickAndPrevent="onRetryButtonClicked"
      />
    </div>
    <div v-else>
      <header-slim :show-in-native="true" :show-in-desktop="false">{{ header }}</header-slim>
      <div :class="$style['information-error']">
        <h2>{{ subheader }}</h2>
        <p :aria-label="messageLabel">{{ messageText }}</p>
      </div>
    </div>
  </div>
</template>
<script>
import isObject from 'lodash/fp/isObject';
import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import GenericButton from '@/components/widgets/GenericButton';
import HeaderSlim from '@/components/HeaderSlim';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessagesSenderError from '@/components/errors/additional-info/MessagesSenderError';
import MessageText from '@/components/widgets/MessageText';
import ReportAProblem from '@/components/errors/ReportAProblem';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { getMessage, getComponentErrorCodeKey, getComponentKey } from '@/lib/errors';
import NativeApp from '@/services/native-app';

const getMappedValue = ({ map, statusCode, errorCode }) => {
  if (!map) {
    return '';
  }

  return (errorCode && map[errorCode])
    || map[statusCode]
    || map.default;
};

export default {
  name: 'ApiError',
  components: {
    ContactOrganDonation,
    DesktopGenericBackLink,
    GenericButton,
    HeaderSlim,
    MessageDialog,
    MessagesSenderError,
    MessageText,
    NativeApp,
    ReportAProblem,
  },
  mixins: [ErrorMessageMixin],
  props: {
    ariaLive: {
      type: String,
      default: 'polite',
    },
  },
  computed: {
    additionalInfo() {
      return getMessage(this, 'additionalInfo');
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
      let clazzes = 'nhsuk-button';
      const url = this.retryUrl;
      if (url && url.length > 0) {
        clazzes = 'nhsuk-button nhsuk-button--secondary';
      }
      return clazzes;
    },
    hasAdditionalInfo() {
      return this.additionalInfo !== '';
    },
    hasSessionReferenceCode() {
      return this.$store.state.session.userSessionCreateReferenceCode;
    },
    header() {
      const headerMessage = getMessage(this, 'header');
      this.trackSystemError(headerMessage);
      return headerMessage;
    },
    isPlainNativeError() {
      return this.$store.state.device.isNativeApp &&
      (this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode]
      === 'plain');
    },
    isStandardError() {
      return this.$store.getters['errors/isStandardError'];
    },
    isVisible() {
      return this.showError();
    },
    message() {
      return getMessage(this, 'message');
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
      return getMessage(this, 'pageHeader');
    },
    pageTitle() {
      return getMessage(this, 'pageTitle') || this.pageHeader;
    },
    retryAction() {
      return getMappedValue({
        map: this.$store.state.errors.pageSettings.action,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      });
    },
    retryButtonText() {
      return getComponentErrorCodeKey(this, this.showError, this.component,
        'retryButtonText', this.errorCode, this.statusCode)
        || getComponentKey(this, this.component, 'retryButtonText', 'errors');
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
      return getMessage(this, 'subheader');
    },
  },
  updated() {
    if (this.showError()) {
      this.setFocus();
      this.updatePageTitle();
      this.updatePageHeader();
      window.scrollTo(0, 0);
    }
  },
  methods: {
    setFocus() {
      if (this.$refs.retryFormRef) {
        this.$refs.retryFormRef.focus();
      }
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    onRetryButtonClicked() {
      if (this.retryAction) {
        this.$store.dispatch(this.retryAction);
        return;
      }
      this.goToUrl(this.retryUrl, this.statusCode);
    },
    showError() {
      return this.hasApiError && !this.hasConnectionError;
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
  @import '../../style/spacings';

  .additionalInformation {
     margin-bottom: 1em
  }

  .information-error {
    p {
      margin-top: 0.5em
    }
  }
</style>
