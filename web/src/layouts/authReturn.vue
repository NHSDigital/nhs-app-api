<template>
  <div>
    <div id="app">
      <div v-if="showError" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
        <div v-if="!$store.state.device.isNativeApp" :class="$style['header-container-desktop']">
          <web-header :show-menu="false" :show-links="false"/>
        </div>
        <div v-else>
          <header-slim :show-in-native="true" :click-url="loginUrl"/>
        </div>
      </div>
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div ref="mainContent" class="nhsuk-grid-column-two-thirds">
            <div id="mainContent">
              <div v-if="showError"
                   :class="!$store.state.device.isNativeApp && $style.desktopWeb">
                <div tabindex="-1"
                     :class="[mainClass, $style['main-container-desktop']]">
                  <div :id="$style.serverError"
                       :class="$store.state.device.isNativeApp
                         ? 'pull-content nhsuk-u-padding-top-7'
                         : ''">
                    <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4
                        nhsuk-u-margin-bottom-0">
                      {{ $t('auth_return.error.title.loginFailed') }} </h1>
                    <error-container v-if="statusCode===464">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-header from="auth_return.error.464.wales.header" />
                      <error-paragraph from="auth_return.error.464.wales.line1" />
                      <error-paragraph-with-links
                        from="auth_return.error.464.wales.line2.content" />
                      <error-header from="auth_return.error.464.england.header" />
                      <error-paragraph from="auth_return.error.464.england.line1" />
                      <error-paragraph-with-links
                        from="auth_return.error.464.england.line2.content"/>
                      <error-paragraph-with-links from="auth_return.error.464.england.line3.content"
                                                  :query-param="contactUsParam"/>
                      <error-header from="auth_return.error.464.ni_scotland.header" />
                      <error-paragraph from="auth_return.error.464.ni_scotland.line1" />
                      <error-paragraph from="auth_return.error.464.ni_scotland.line2" />
                      <p id="errorCode" class="nhsuk-u-font-size-16">
                        {{ $t('auth_return.error.464.reference') }}
                        {{ serviceDeskReference }}
                      </p>
                    </error-container>
                    <error-container v-else-if="statusCode===465">
                      <error-title title="auth_return.error.465.title"/>
                      <error-paragraph from="auth_return.error.465.message" />
                    </error-container>
                    <error-container v-else-if="statusCode===400">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.400.line1" />
                      <error-paragraph from="auth_return.error.400.line2"
                                       :variable="serviceDeskReference"/>
                      <error-paragraph from="auth_return.error.400.contactUs" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===403">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.403.line1" />
                      <error-paragraph from="auth_return.error.403.line2" />
                      <error-paragraph from="auth_return.error.403.line3"
                                       :variable="serviceDeskReference"/>
                      <error-paragraph from="auth_return.error.403.line4" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===500">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.500.line1" />
                      <error-paragraph from="auth_return.error.500.line3" />
                      <error-paragraph from="auth_return.error.500.line4"
                                       :variable="serviceDeskReference"/>
                      <error-paragraph from="auth_return.error.500.line5" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===502">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.502.listTitle" />
                      <error-unordered-list from="auth_return.error.502.uList" />
                      <error-paragraph from="auth_return.error.502.line3" />
                      <error-paragraph from="auth_return.error.502.line4"
                                       :variable="serviceDeskReference"/>
                      <error-paragraph from="auth_return.error.502.message" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else-if="statusCode===504">
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.504.listTitle" />
                      <error-unordered-list from="auth_return.error.504.uList" />
                      <error-paragraph from="auth_return.error.504.line3" />
                      <error-paragraph from="auth_return.error.504.line4"
                                       :variable="serviceDeskReference" />
                      <error-paragraph from="auth_return.error.504.message" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                    <error-container v-else>
                      <error-title title="auth_return.error.title.loginFailed"/>
                      <error-paragraph from="auth_return.error.default.line1" />
                      <error-paragraph from="auth_return.error.default.line2" />
                      <error-paragraph from="auth_return.error.default.line3"
                                       :variable="serviceDeskReference"/>
                      <error-paragraph from="auth_return.error.default.line4" />
                      <error-link from="generic.contactUsButton.text"
                                  :action="contactUsUrl"
                                  target="_blank"
                                  :query-param="contactUsParam"/>
                      <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                    </error-container>
                  </div>
                </div>
              </div>
              <div v-else>
                <main >
                  <spinner />
                  <connection-error />
                  <api-error />
                  <flash-message />
                  <slot />
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
import Spinner from '@/components/widgets/Spinner';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import { LOGIN_PATH } from '@/router/paths';

export default {
  components: {
    ApiError,
    ConnectionError,
    ErrorContainer,
    ErrorHeader,
    ErrorLink,
    ErrorParagraph,
    ErrorParagraphWithLinks,
    ErrorTitle,
    ErrorUnorderedList,
    FlashMessage,
    HeaderSlim,
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
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
    };
  },
  computed: {
    title() {
      return this.showError && this.$t('auth_return.error.title.loginFailed');
    },
    contactUsParam() {
      return {
        param: 'errorcode',
        value: this.serviceDeskReference,
      };
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
</style>
