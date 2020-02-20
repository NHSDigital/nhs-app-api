<template>
  <div>
    <div id="app" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
      <div v-if="!$store.state.device.isNativeApp" :class="$style['header-container-desktop']">
        <web-header :show-menu="false" :show-links="false"/>
      </div>
      <div v-else>
        <header-slim :show-in-native="true" :click-url="loginUrl"/>
      </div>
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div ref="mainContent" class="nhsuk-grid-column-two-thirds">
            <div id="mainContent"
                 tabindex="-1"
                 :class="[mainClass, $style['main-container-desktop']]">
              <div v-if="showError"
                   :class="!$store.state.device.isNativeApp && $style.desktopWeb">
                <div :id="$style.serverError"
                     :class="$store.state.device.isNativeApp
                       ? 'pull-content nhsuk-u-padding-top-7'
                       : ''">
                  <h1 class="nhsuk-u-padding-bottom-3 nhsuk-u-margin-top-4 nhsuk-u-margin-bottom-0">
                    {{ $t('auth_return.error.title.loginFailed') }} </h1>
                  <error-container v-if="errorStatusCode===464">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.464.line1" />
                    <error-unordered-list from="auth_return.error.464.uList" />
                    <error-paragraph from="auth_return.error.464.contactUs"
                                     :variable="serviceDeskReference"/>
                    <error-paragraph from="auth_return.error.464.message" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
                                :query-param="contactUsParam"/>
                  </error-container>
                  <error-container v-else-if="errorStatusCode===465">
                    <error-title title="auth_return.error.465.title"/>
                    <error-paragraph from="auth_return.error.465.message" />
                  </error-container>
                  <error-container v-else-if="errorStatusCode===400">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.400.line1" />
                    <error-paragraph from="auth_return.error.400.line2"
                                     :variable="serviceDeskReference"/>
                    <error-paragraph from="auth_return.error.400.contactUs" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
                                :query-param="contactUsParam"/>
                    <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                  </error-container>
                  <error-container v-else-if="errorStatusCode===403">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.403.line1" />
                    <error-paragraph from="auth_return.error.403.line2" />
                    <error-paragraph from="auth_return.error.403.line3"
                                     :variable="serviceDeskReference"/>
                    <error-paragraph from="auth_return.error.403.line4" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
                                :query-param="contactUsParam"/>
                    <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                  </error-container>
                  <error-container v-else-if="errorStatusCode===500">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.500.line1" />
                    <error-paragraph from="auth_return.error.500.line3" />
                    <error-paragraph from="auth_return.error.500.line4"
                                     :variable="serviceDeskReference"/>
                    <error-paragraph from="auth_return.error.500.line5" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
                                :query-param="contactUsParam"/>
                    <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                  </error-container>
                  <error-container v-else-if="errorStatusCode===502">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.502.listTitle" />
                    <error-unordered-list from="auth_return.error.502.uList" />
                    <error-paragraph from="auth_return.error.502.line3" />
                    <error-paragraph from="auth_return.error.502.line4"
                                     :variable="serviceDeskReference"/>
                    <error-paragraph from="auth_return.error.502.message" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
                                :query-param="contactUsParam"/>
                    <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                  </error-container>
                  <error-container v-else-if="errorStatusCode===504">
                    <error-title title="auth_return.error.title.loginFailed"/>
                    <error-paragraph from="auth_return.error.504.listTitle" />
                    <error-unordered-list from="auth_return.error.504.uList" />
                    <error-paragraph from="auth_return.error.504.line3" />
                    <error-paragraph from="auth_return.error.504.line4"
                                     :variable="serviceDeskReference" />
                    <error-paragraph from="auth_return.error.504.message" />
                    <error-link from="generic.contactUsButton.text"
                                :action="contactUsUrl"
                                :target="target"
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
                                :target="target"
                                :query-param="contactUsParam"/>
                    <error-link from="auth_return.error.backButtonText" :action="loginUrl"/>
                  </error-container>
                </div>
              </div>
              <div v-else>
                <main :class="mainClass">
                  <spinner />
                  <connection-error />
                  <api-error />
                  <flash-message />
                  <nuxt />
                </main>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-if="!$store.state.device.isNativeApp"
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
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import ErrorUnorderedList from '@/components/errors/ErrorUnorderedList';
import FlashMessage from '@/components/widgets/FlashMessage';
import HeaderSlim from '@/components/HeaderSlim';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import Spinner from '@/components/widgets/Spinner';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import { LOGIN } from '@/lib/routes';

export default {
  components: {
    ApiError,
    ConnectionError,
    ErrorContainer,
    ErrorLink,
    ErrorParagraph,
    ErrorTitle,
    ErrorUnorderedList,
    FlashMessage,
    HeaderSlim,
    Spinner,
    WebHeader,
    WebFooter,
  },
  mixins: [ErrorMessageMixin],
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: this.$t('auth_return.error.title.loginFailed'),
    };
  },
  data() {
    return {
      contactUsUrl: this.$env.CONTACT_US_URL,
    };
  },
  computed: {
    contactUsParam() {
      return {
        param: 'errorcode',
        value: this.serviceDeskReference,
      };
    },
    errorStatusCode() {
      return this.statusCode;
    },
    headerTitle() {
      return this.showError()
        ? this.getMessage('header')
        : this.$store.state.header.headerText;
    },
    loginUrl() {
      return LOGIN.path;
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
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.errorStatusCode];
    },
    serviceDeskReference() {
      return get('state.errors.apiErrors[0].serviceDeskReference')(this.$store) || '';
    },
    target() {
      return '_blank';
    },
  },
  created() {
    this.$store.dispatch('pageTitle/updatePageTitle', this.$t('auth_return.error.title.loginFailed'));
  },
  mounted() {
    NativeVersionSetup(this.$store);
  },
  methods: {
    pageTitle() {
      const nhsApp = 'NHS App';
      const { pageTitle } = this.$store.state.pageTitle;

      if (pageTitle) {
        return `${pageTitle}-${nhsApp}`;
      }

      return nhsApp;
    },
    showError() {
      return this.hasApiError || this.hasConnectionError; // API or connection errors
    },
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
