<template>
  <div>
    <div id="app">
      <div v-if="showError"
           :class="!isNativeApp && $style.desktopWeb">
        <div v-if="!isNativeApp" :class="$style['header-container-desktop']">
          <web-header :show-menu="false" :show-links="false"/>
        </div>
        <div v-else>
          <header-slim :show-in-native="true" :click-url="loginUrl"/>
        </div>
      </div>
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div ref="mainContent" tabindex="-1" class="nhsuk-grid-column-two-thirds">
            <div id="maincontent">
              <div v-if="showError && !termsNotAccepted"
                   id="authReturnError"
                   :class="!isNativeApp && $style.desktopWeb">
                <div tabindex="-1"
                     :class="[mainClass, $style['main-container-desktop']]">
                  <div :id="$style.serverError"
                       :class="isNativeApp
                         ? 'pull-content nhsuk-u-padding-top-7'
                         : ''">
                    <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                        nhsuk-u-margin-bottom-0">
                      {{ $t('login.authReturn.loginFailed') }} </h1>
                    <error-container v-if="statusCode===464">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-header from="login.authReturn.ifYourSurgeryIsInWales" />
                      <error-paragraph from="login.authReturn.notAvailableInWales" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedInWales" />
                      <error-header from="login.authReturn.ifYourSurgeryIsInEngland" />
                      <error-paragraph
                        from="login.authReturn.weCannotConnectToSurgeryOrMatchYourNhsNumber" />
                      <error-paragraph-with-links from="login.authReturn.ifYouNeedInEngland"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouStillNeedHelp"
                                                  :query-param="contactUsParam"/>
                      <error-header
                        from="login.authReturn.ifYourSurgeryIsInNorthernIrelandOrScotland" />
                      <error-paragraph
                        from="login.authReturn.notAvailableInNorthernIrelandOrScotland" />
                      <error-paragraph
                        from="login.authReturn.ifYouNeedInNorthernIrelandOrScotland" />
                      <p id="errorCode" class="nhsuk-u-font-size-16">
                        {{ $t('login.authReturn.reference') }}
                        {{ serviceDeskReference }}
                      </p>
                    </error-container>
                    <error-container v-else-if="statusCode===465">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph-with-links
                        from="login.authReturn.dueToLegalRestrictionsUntilYouAreThirteen" />
                    </error-container>
                    <error-container v-else-if="statusCode===400">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph
                        from="login.authReturn.ifYouKeepSeeingThisQuoteCodeToHelpUs"
                        :variable="serviceDeskReference"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===403">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph
                        from="login.authReturn.weCannotGetYourDetailsFromYourGpSurgery" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph
                        from="login.authReturn.ifYouKeepSeeingThisQuoteCodeToHelpUs"
                        :variable="serviceDeskReference"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===500">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph from="login.authReturn.weCannotLoginYouIn" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph
                        from="login.authReturn.ifYouKeepSeeingThisQuoteCodeToHelpUs"
                        :variable="serviceDeskReference"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===502">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph from="login.authReturn.thisCanBeOneOfTwoProblems" />
                      <error-unordered-list
                        from="login.authReturn.weCannotGetYourLoginDetailsOrConnectToYourSurgery" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph
                        from="login.authReturn.ifYouKeepSeeingThisQuoteCodeToHelpUs"
                        :variable="serviceDeskReference"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===504">
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph from="login.authReturn.thisCanBeOneOfTwoProblems" />
                      <error-unordered-list
                        from="login.authReturn.weCannotGetYourLoginDetailsOrConnectToYourSurgery" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouKeepSeeingThisQuoteCode"
                                       :variable="serviceDeskReference" />
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else>
                      <error-title title="login.authReturn.loginFailed"/>
                      <error-paragraph from="login.authReturn.weCannotLoginYouIn" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph
                        from="login.authReturn.ifYouKeepSeeingThisQuoteCodeToHelpUs"
                        :variable="serviceDeskReference"/>
                      <error-paragraph-with-links from="login.authReturn.ifYouNeed" />
                      <error-link from="generic.contactUs"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="login.authReturn.backToHome" :action="loginUrl"/>
                    </error-container>
                  </div>
                </div>
              </div>
              <div v-else-if="termsNotAccepted"
                   id="termsAndConditionsError"
                   :class="isNativeApp && 'pull-content nhsuk-u-padding-top-7'">
                <page-title>
                  {{ $t('login.authReturn.termsNotAccepted') }}
                </page-title>
                <p class="nhsuk-u-margin-top-3">
                  {{ $t('login.authReturn.youCannotUse') }}
                </p>
                <p>{{ $t('login.authReturn.ifYouNeedToBook') }}</p>
                <contact-111
                  :text="$t('appointments.confirmation.error.forUrgentMedicalAdvice.text')"
                  :aria-label="$t('appointments.confirmation.error.forUrgentMedicalAdvice.label')"/>
                <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
              </div>
              <div v-else>
                <main >
                  <div :class="$style['blue-body']">
                    <spinner />
                    <connection-error />
                    <api-error />
                    <flash-message />
                    <slot />
                  </div>
                </main>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-if="showError && !$store.state.device.isNativeApp"
         :class="$style['footer-container-desktop']">
      <web-footer />
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import Contact111 from '@/components/widgets/Contact111';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorHeader from '@/components/errors/ErrorHeader';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorParagraphWithLinks from '@/components/errors/ErrorParagraphWithLinks';
import ErrorTitle from '@/components/errors/ErrorTitle';
import ErrorUnorderedList from '@/components/errors/ErrorUnorderedList';
import FlashMessage from '@/components/widgets/FlashMessage';
import HeaderSlim from '@/components/HeaderSlim';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import PageTitle from '@/components/widgets/PageTitle';
import Spinner from '@/components/widgets/Spinner';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import { LOGIN_PATH } from '@/router/paths';
import { CONSENT_NOT_GIVEN_DESCRIPTION } from '@/lib/utils';

export default {
  components: {
    ApiError,
    ConnectionError,
    Contact111,
    ErrorContainer,
    ErrorHeader,
    ErrorLink,
    ErrorParagraph,
    ErrorParagraphWithLinks,
    ErrorTitle,
    ErrorUnorderedList,
    FlashMessage,
    HeaderSlim,
    PageTitle,
    Spinner,
    WebHeader,
    WebFooter,
  },
  metaInfo() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: this.title,
    };
  },
  data() {
    return {
      consentNotGivenDescription: CONSENT_NOT_GIVEN_DESCRIPTION,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
    };
  },
  computed: {
    title() {
      const title = (this.termsNotAccepted) ?
        this.$t('login.authReturn.termsNotAccepted') :
        this.$t('login.authReturn.loginFailed');
      return this.showError && `${title} - ${this.$t('appTitle')}`;
    },
    contactUsParam() {
      return {
        param: 'errorcode',
        value: this.serviceDeskReference,
      };
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    loginUrl() {
      return LOGIN_PATH;
    },
    mainClass() {
      const classes = ['content', 'pull-body'];
      if (this.$store.state.device.isNativeApp) {
        classes.push('native');
        classes.push('slim');
        classes.push('nhsuk-u-margin-bottom-9');
      }
      return classes;
    },
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    serviceDeskReference() {
      return get('$store.state.errors.apiErrors[0].serviceDeskReference')(this) || '';
    },
    statusCode() {
      return get('$store.state.errors.apiErrors[0].status')(this);
    },
    showError() {
      return this.$store.getters['errors/showApiError'] || this.$store.state.errors.hasConnectionProblem;
    },
    termsNotAccepted() {
      return this.$route.query.error_description === this.consentNotGivenDescription;
    },
  },
  mounted() {
    NativeVersionSetup(this.$store);
  },
};
</script>

<style lang="scss">
@import "../style/main";
@import "../style/pulltorefresh";
@import "../style/elements";
@import "~nhsuk-frontend/packages/nhsuk";
</style>

<style module lang="scss" scoped>
@import "../style/home";

div {
  &.desktopWeb {
    .header-container-desktop,
    .footer-container-desktop {
      order: 0;
      flex: 0 0 auto;
      align-self: stretch;
      width: 100%;
    }
  }
}

.blue-body {
  background-color: rgb(0, 94, 184);
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
}
</style>
