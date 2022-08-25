<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showError" :class="!isNativeApp && $style.desktopWeb">
    <div v-if="isStandardError" :id="$style.serverError" class="pull-content">
      <message-dialog override-style="plain"
                      :focusable="true"
                      :aria-live="ariaLive">
        <message-text v-if="!isEmpty(header)"
                      :is-header="true"
                      :unindent="isPlainNativeError"
                      override-style="plain"
                      data-purpose="msg-header">
          {{ header }}
        </message-text>
        <message-text v-if="subheader !== '' && displayPrescriptionsSubheader"
                      :unindent="isPlainNativeError"
                      override-style="plain"
                      data-purpose="msg-subheader">
          {{ subheader }}
          <a v-if="displayBackToPrescriptionsLinkText" id="prescriptionsLink" :href="prescriptionsPath">
            {{ backToPrescriptionsLinkText }}
          </a>
        </message-text>
        <message-text v-if="isSystemOrServerError">
          <a id="systemOrServerErrorBackLink" :href="backLinkUrl"> {{ $t('apiErrors.goBackAndTryAgain') }} </a>
        </message-text>
        <message-text v-if="isSystemOrServerError || isGenericServiceDownError"
                      :is-before-footer="true"
                      :unindent="isPlainNativeError"
                      data-purpose="ifYouNeedToBookAnAppointment-text"
                      v-html="$t('apiErrors.ifYouNeedToBookAnAppointment')"/>
        <message-text :is-before-footer="true"
                      :unindent="isPlainNativeError"
                      :aria-label="messageLabel"
                      data-purpose="msg-text"
                      v-html="messageText"/>
        <message-text v-if="deviceSettings"
                      :unindent="isPlainNativeError"
                      :aria-label="deviceSettingsLabel"
                      data-purpose="msg-device-settings">
          <a id="device-settings" href="#" @click.prevent.stop="openAppSettings">
            {{ deviceSettingsText }}
          </a>
        </message-text>
        <component :is="additionalInfoComponentName" v-if="additionalInfoComponentName"
                   :unindent="isPlainNativeError"
                   :class="$style.additionalInformation"/>
        <message-text v-if="hasSessionReferenceCode && !isGenericServiceDownError">
          <report-a-problem :reference="hasSessionReferenceCode"/>
        </message-text>
      </message-dialog>
      <service-desk-reference-link v-if="contactWithErrorCode && showServiceDeskReferenceLink"/>
      <form v-if="retryButtonText && !backLinkUrl && !deviceSettings" id="retryButton"
            ref="retryFormRef"
            :action="retryUrl" method="get" tabindex="-1">
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
      <desktop-generic-back-link
        v-if="retryButtonText && backLinkUrl"
        id="backLink"
        :path="backLinkUrl"
        :button-text="retryButtonText"
        data-purpose="back-button"
      />
    </div>
    <div v-else>
      <header-slim :show-in-native="true" :show-in-desktop="false">{{ header }}</header-slim>
      <div :class="$style['information-error']">
        <h2>{{ subheader }}</h2>
        <p :aria-label="messageLabel" v-html="messageText"/>
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
import MessageText from '@/components/widgets/MessageText';
import ReportAProblem from '@/components/errors/ReportAProblem';
import { getDynamicStyle } from '@/lib/desktop-experience';
import NativeApp from '@/services/native-app';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { isBlankString } from '@/lib/utils';
import { PRESCRIPTIONS_PATH } from '@/router/paths';
import ServiceDeskReferenceLink from '@/components/errors/ServiceDeskReferenceLink';

const getMappedValue = ({ map, statusCode, errorCode }) => {
  if (!map) {
    return '';
  }

  if (errorCode && map[errorCode] !== undefined) {
    return map[errorCode];
  }
  return map[statusCode] || map.default;
};

export default {
  name: 'ApiError',
  components: {
    ContactOrganDonation,
    DesktopGenericBackLink,
    GenericButton,
    HeaderSlim,
    MessageDialog,
    MessageText,
    NativeApp,
    ReportAProblem,
    ServiceDeskReferenceLink,
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
      symptomCheckerUrl: this.$store.$env.SYMPTOM_CHECKER_URL,
      prescriptionsPath: `/${PRESCRIPTIONS_PATH}`,
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
    backLinkUrl() {
      return getMappedValue({
        map: this.$store.state.errors.pageSettings.backLinks,
        errorCode: this.errorCode,
        statusCode: this.statusCode,
      });
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
      return (isObject(this.message) ? this.message.text : this.message).replace(/{111link}/g, this.get111HyperLink);
    },
    deviceSettings() {
      return this.getMessage('deviceSettings');
    },
    deviceSettingsLabel() {
      return isObject(this.deviceSettings) ? this.deviceSettings.label : undefined;
    },
    deviceSettingsText() {
      return isObject(this.deviceSettings) ? this.deviceSettings.text : this.deviceSettings;
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
    contactWithErrorCode() {
      return this.getComponentErrorCodeKey('contactWithErrorCode') || this.getComponentKey('contactWithErrorCode');
    },
    showServiceDeskReferenceLink() {
      return this.statusCode !== 504 && this.statusCode !== 403;
    },
    backToPrescriptionsLinkText() {
      return this.getMessage('backToPrescriptionsLinkText');
    },
    displayBackToPrescriptionsLinkText() {
      return this.statusCode !== 466 && this.component === 'prescriptions.confirm_prescription_details';
    },
    displayPrescriptionsSubheader() {
      if (this.component === 'prescriptions.confirm_prescription_details') {
        return this.statusCode !== 466;
      }
      return true;
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
    isSystemOrServerError() {
      return this.pageHeader === this.getText('apiErrors.pageHeader')
        && this.statusCode !== 404 && this.statusCode !== 502;
    },
    isGenericServiceDownError() {
      return this.pageHeader === this.getText('apiErrors.502.pageHeader') && this.statusCode === 502;
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
    /*
      Looks up the appropriate error messages in the locale based on a heirarchy of component and
      status/error code.
      The lookup goes from most specific to least specific:
        * component + status/error code
        * component
        * status code
        * default

      The following path are used:
       * apiErrors.components.[component].[code]
       * apiErrors.components.[component]
       * [component].errors.[code] (depreciated)
       * [component].errors (depreciated)
       * apiErrors.[code]
       * apiErrors

      The `type` argument refers to the property search for under these paths for specific text to
      be displayed on the error page, e.g. header or message.text
    */
    getComponentErrorCodeKey(type) {
      if (this.hasApiError) {
        return this.getPrefixedErrorCodeKey(`apiErrors.components.${this.component}`, type) ||
          this.getPrefixedErrorCodeKey(`${this.component}.errors`, type);
      }

      return '';
    },
    getPrefixedErrorCodeKey(prefix, type) {
      return (this.errorCode && this.getText(`${prefix}.${this.statusCode}.${this.errorCode}.${type}`)) ||
        (this.errorCode && this.getText(`${prefix}.${this.errorCode}.${type}`)) ||
        this.getText(`${prefix}.${this.statusCode}.${type}`);
    },
    getComponentKey(type) {
      return this.getText(`apiErrors.components.${this.component}.${type}`) ||
        this.getText(`${this.component}.errors.${type}`);
    },
    getMessage(type) {
      if (this.showError) {
        return this.getComponentErrorCodeKey(type) ||
          this.getText(`apiErrors.components.${this.component}.${type}`) ||
          this.getText(`${this.component}.errors.${type}`) ||
          this.getText(`apiErrors.${this.statusCode}.${type}`) ||
          this.getText(`apiErrors.${type}`);
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
    get111HyperLink() {
      return `<a href="${this.symptomCheckerUrl}" target="_blank" rel="noopener noreferrer" style="display:inline">111.nhs.uk</a>`;
    },
    openAppSettings() {
      NativeApp.openAppSettings();
    },
    isEmpty(value) {
      return isBlankString(value);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/api-error"
</style>
