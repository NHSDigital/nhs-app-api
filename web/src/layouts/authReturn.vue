<template>
  <div>
    <div id="app">
      <div v-if="hasConnectionProblem"
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
              <div v-if="hasConnectionProblem && !termsNotAccepted"
                   id="authReturnError"
                   :class="!isNativeApp && $style.desktopWeb">
                <div tabindex="-1"
                     :class="[mainClass, $style['main-container-desktop']]">
                  <div :id="$style.serverError"
                       :class="isNativeApp
                         ? 'pull-content nhsuk-u-padding-top-7'
                         : ''">
                    <div v-if="statusCode===464">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                           nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.loginFailed') }} </h1>
                      <error-paragraph from="login.authReturn.ifYouAreNotRegisteredInEngland" />
                      <error-paragraph-with-links
                        from="login.authReturn.howToGetYourCovidPass" />
                      <error-header from="login.authReturn.ifYourSurgeryIsInWales" />
                      <error-paragraph
                        from="login.authReturn.notAvailableInWales" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedAnAppointment" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedUrgentMedicalAdviceInWales"/>
                      <error-header
                        from="login.authReturn.ifYouAreRegisteredWithTheArmedForces" />
                      <error-paragraph
                        from="login.authReturn.armedForcesMessage" />
                      <error-header
                        from="login.authReturn.ifYourSurgeryIsInScotlandOrNorthernIreland" />
                      <error-paragraph
                        from="login.authReturn.notAvailableInScotlandOrNorthernIreland" />
                      <error-paragraph
                        from="login.authReturn.contactGPSurgeryDirectly" />
                      <error-paragraph
                        from="login.authReturn.ifYouAreInScotland" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedUrgentMedicalAdviceInNorthernIreland" />
                      <error-header from="login.authReturn.ifYouAreRegisteredAndNotArmedForces" />
                      <error-paragraph
                        from="login.authReturn.cannotConnectToGpSurgery" />
                      <error-link from="generic.contactUsWithErrorCode"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"
                                  :params="{errorCode: serviceDeskReference}"/>
                    </div>
                    <error-container v-else-if="statusCode===468">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph-with-links
                        from="login.authReturn.findOutHowToGetYourCovidPass"/>
                      <error-paragraph from="login.authReturn.checkIfRegisteredWithSurgery" />
                      <error-paragraph
                        from="login.authReturn.yourSurgeryMayNeedToResubmitRegistration" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedUrgentMedicalAdvice"/>
                      <error-paragraph from="login.authReturn.stillUnableToAccessNhsApp" />
                      <error-link from="generic.contactUsWithErrorCode"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"
                                  :params="{errorCode: serviceDeskReference}"/>
                    </error-container>
                    <error-container v-else-if="statusCode===469">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.cannotMatchNhsNumberToSurgery" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph-with-links
                        from="login.authReturn.ifYouNeedUrgentMedicalAdvice"/>
                      <error-link from="generic.contactUsWithErrorCode"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"
                                  :params="{errorCode: serviceDeskReference}"/>
                    </error-container>
                    <error-container v-else-if="statusCode===465" override-style="plain">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.under13.title') }} </h1>
                      <p>{{ $t('login.authReturn.under13.mustBeOver13') }}</p>
                      <p>{{ $t('login.authReturn.under13.ifAged12') }}</p>
                      <error-link from="login.authReturn.under13.getDigitalCovidPass"
                                  :action="digitalCovidPassUrl"
                                  target="_blank" />
                      <error-link from="login.authReturn.under13.getCovidPassLetter"
                                  :action="letterCovidPassUrl"
                                  target="_blank" />
                      <p>{{ $t('login.authReturn.under13.contactGpToFindOut') }}</p>
                      <p aria-label="$t('login.authReturn.under13.u13UrgentMedicalAdvice.label')">
                        {{ $t('login.authReturn.under13.urgentMedicalAdvice.text') }}
                      </p>
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                    </error-container>
                    <error-container v-else-if="statusCode===400">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===403">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===500">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===502">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.thereWasAnErrorEither" />
                      <error-unordered-list
                        from="login.authReturn.connectToGetYourSurgeryOrGettingYourLoginDetails" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===504">
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.cannotLogin') }} </h1>
                      <error-paragraph from="login.authReturn.thereWasAnErrorEither" />
                      <error-unordered-list
                        from="login.authReturn.connectToGetYourSurgeryOrGettingYourLoginDetails" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else>
                      <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                          nhsuk-u-margin-bottom-0">
                        {{ $t('login.authReturn.loginFailed') }} </h1>
                      <error-paragraph from="login.authReturn.weCannotLoginYouIn" />
                      <error-paragraph from="login.authReturn.goBackAndTryAgain" />
                      <error-paragraph from="login.authReturn.ifYouNeedToBook" />
                      <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                      <error-link from="login.authReturn.goToNhs111"
                                  :action="nhs111Url"
                                  target="_blank" />
                      <service-desk-reference-link />
                      <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
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
                <error-paragraph from="login.authReturn.forUrgentMedicalAdvice" />
                <error-link from="login.authReturn.goToNhs111"
                            :action="nhs111Url"
                            target="_blank" />
                <error-link from="login.authReturn.backToLogin" :action="loginUrl"/>
              </div>
              <div v-else>
                <main>
                  <div :class="useDefaultPageStyling
                    ? $style['default-grey; footer-container-desktop']
                    : $style['grey-body']">
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
    <div v-if="hasConnectionProblem && !$store.state.device.isNativeApp"
         :class="$style['footer-container-desktop']">
      <web-footer />
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorHeader from '@/components/errors/ErrorHeader';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorParagraphWithLinks from '@/components/errors/ErrorParagraphWithLinks';
import ErrorUnorderedList from '@/components/errors/ErrorUnorderedList';
import FlashMessage from '@/components/widgets/FlashMessage';
import HeaderSlim from '@/components/HeaderSlim';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import PageTitle from '@/components/widgets/PageTitle';
import ServiceDeskReferenceLink from '@/components/errors/ServiceDeskReferenceLink';
import Spinner from '@/components/widgets/Spinner';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import { LOGIN_PATH } from '@/router/paths';
import {
  GP_SESSION_ON_DEMAND_RETURN_NAME,
  REDIRECT_PARAMETER,
  INTEGRATION_REFERRER_PARAMETER,
  REFERRER_PARAMETER,
} from '@/router/names';
import { CONSENT_NOT_GIVEN_DESCRIPTION } from '@/lib/utils';

export default {
  components: {
    ApiError,
    ConnectionError,
    ErrorContainer,
    ErrorHeader,
    ErrorLink,
    ErrorParagraph,
    ErrorParagraphWithLinks,
    ErrorUnorderedList,
    FlashMessage,
    HeaderSlim,
    PageTitle,
    ServiceDeskReferenceLink,
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
      nhs111Url: this.$store.$env.SYMPTOM_CHECKER_URL,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      letterCovidPassUrl: this.$store.$env.COVID_PASS_LETTER_URL,
      digitalCovidPassUrl: this.$store.$env.COVID_STATUS_URL,
    };
  },
  computed: {
    title() {
      let title;
      if (this.termsNotAccepted) {
        title = this.$t('login.authReturn.termsNotAccepted');
      } else if (this.statusCode === 465) {
        title = this.$t('login.authReturn.under13.title');
      } else if (this.statusCode === 400 || this.statusCode === 500 || this.statusCode === 468
                 || this.statusCode === 403 || this.statusCode === 469 || this.statusCode === 502
                 || this.statusCode === 504) {
        title = this.$t('login.authReturn.cannotLogin');
      } else {
        title = this.$t('login.authReturn.loginFailed');
      }

      return this.hasConnectionProblem
        ? `${title} - ${this.$t('appTitle')}`
        : undefined;
    },
    contactUsParam() {
      return {
        ErrCodeParam: 'errorcode',
        ErrCodeValue: this.serviceDeskReference,
        OdsCodeParam: 'odscode',
        OdsCodeValue: '',
      };
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    loginUrl() {
      if (this.$store.$env.SKIP_LOGGED_OUT_ENABLED) {
        const stateInfo = this.$route.query.state;
        if (typeof stateInfo === 'string') {
          const paramsIndex = stateInfo.indexOf('?');
          if (paramsIndex >= 0) {
            const redirectTo = stateInfo.substring(0, paramsIndex);
            const urlSearchParams = new URLSearchParams(stateInfo.substring(paramsIndex));
            const referrer = urlSearchParams.get(INTEGRATION_REFERRER_PARAMETER);

            return `${LOGIN_PATH}?${REDIRECT_PARAMETER}=${redirectTo}&${REFERRER_PARAMETER}=${referrer}`;
          }
        }
      }
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
    termsNotAccepted() {
      return this.$route.query.error_description === this.consentNotGivenDescription;
    },
    useDefaultPageStyling() {
      return this.$route.name === GP_SESSION_ON_DEMAND_RETURN_NAME;
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
@import "../style/custom/auth-return-layout";
</style>
