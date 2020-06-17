<template>
  <div v-if="showError" :class="!isNativeApp && $style.desktopWeb">
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
        <message-text v-if="additionalInfo"
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
      <form v-if="retryButtonText && (isNativeApp || retryAction)"
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
        v-if="retryButtonText && !isNativeApp && retryUrl"
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
import get from 'lodash/fp/get';
import isObject from 'lodash/fp/isObject';
import ContactOrganDonation from '@/components/errors/additional-info/ContactOrganDonation';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import HeaderSlim from '@/components/HeaderSlim';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessagesSenderError from '@/components/errors/additional-info/MessagesSenderError';
import MessageText from '@/components/widgets/MessageText';
import ReportAProblem from '@/components/errors/ReportAProblem';
import { getDynamicStyle } from '@/lib/desktop-experience';
import NativeApp from '@/services/native-app';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

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
  props: {
    ariaLive: {
      type: String,
      default: 'polite',
    },
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    component() {
      let routePathFormatted = this.$store.state.errors.routePath.replace(/\//g, '.').replace(/-/g, '_');
      if (routePathFormatted.indexOf('.') === 0) {
        routePathFormatted = routePathFormatted.substr(1);
      }
      const prefix = 'patient.';
      const hasPrefix = routePathFormatted.indexOf(prefix) === 0;
      if (hasPrefix) {
        return routePathFormatted.substr(prefix.length);
      }
      return routePathFormatted;
    },
    errorCode() {
      return get('$store.state.errors.apiErrors[0].error')(this);
    },
    statusCode() {
      return get('$store.state.errors.apiErrors[0].status')(this);
    },
    pageHeader() {
      return this.getMessage('pageHeader');
    },
    pageTitle() {
      return this.getMessage('pageTitle') || this.pageHeader;
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
    messageLabel() {
      return isObject(this.message) ? this.message.label : undefined;
    },
    messageText() {
      return isObject(this.message) ? this.message.text : this.message;
    },
    additionalInfo() {
      return this.getMessage('additionalInfo');
    },
    additionalInfoLabel() {
      return isObject(this.additionalInfo) ? this.additionalInfo.label : undefined;
    },
    additionalInfoText() {
      return isObject(this.additionalInfo) ? this.additionalInfo.text : this.additionalInfo;
    },
    additionalInfoComponentName() {
      return this.$store.state.errors.pageSettings.additionalInfoComponent;
    },
    retryButtonText() {
      return this.getComponentErrorCodeKey('retryButtonText') || this.getComponentKey('retryButtonText');
    },
    buttonClasses() {
      let clazzes = 'nhsuk-button';
      const url = this.retryUrl;
      if (url && url.length > 0) {
        clazzes = 'nhsuk-button nhsuk-button--secondary';
      }
      return clazzes;
    },
    hasSessionReferenceCode() {
      return this.$store.state.session.userSessionCreateReferenceCode;
    },
    isPlainNativeError() {
      return this.isNativeApp &&
      (this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode]
      === 'plain');
    },
    isStandardError() {
      return this.$store.getters['errors/isStandardError'];
    },
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    retryAction() {
      return getMappedValue({
        map: this.$store.state.errors.pageSettings.action,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      });
    },
    retryUrl() {
      return this.correctUrl(getMappedValue({
        map: this.$store.state.errors.pageSettings.redirectUrl,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      }));
    },
    showError() {
      return this.hasApiError && !this.$store.state.errors.hasConnectionProblem;
    },
  },
  updated() {
    if (this.showError) {
      this.setFocus();
      EventBus.$emit(UPDATE_HEADER, this.pageHeader, true, true);
      EventBus.$emit(UPDATE_TITLE, this.pageTitle, true);
      window.scrollTo(0, 0);
    }
  },
  methods: {
    getComponentErrorCodeKey(type) {
      if (this.hasApiError) {
        return (this.errorCode && this.getText(`${this.component}.errors.${this.statusCode}.${this.errorCode}.${type}`))
          || (this.errorCode && this.getText(`${this.component}.errors.${this.errorCode}.${type}`))
          || this.getText(`${this.component}.errors.${this.statusCode}.${type}`);
      }

      return '';
    },
    getComponentKey(type) {
      return this.getText(`${this.component}.errors.${type}`);
    },
    getMessage(type) {
      if (this.showError) {
        return this.getComponentErrorCodeKey(type)
          || this.getText(`${this.component}.errors.${type}`)
          || this.getText(`errors.${this.statusCode}.${type}`)
          || this.getText(`errors.${type}`);
      }

      return '';
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
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
    trackSystemError(message) {
      const errorMessage = {
        type: 'system_error',
        messages: [message],
      };
      this.$store.dispatch('analytics/trackError', errorMessage);
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
