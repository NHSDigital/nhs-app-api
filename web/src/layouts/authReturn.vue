<template>
  <div>
    <div
      id="app"
      :class="!$store.state.device.isNativeApp && $style.desktopWeb"
    >
      <div
        v-if="!$store.state.device.isNativeApp"
        :class="$style['header-container-desktop']"
      >
        <web-header
          :show-menu="false"
          :show-links="false"
        />
      </div>
      <div v-else>
        <header-slim
          :show-in-native="true"
          :click-url="loginUrl"
        >{{ headerTitle }}
        </header-slim>
      </div>
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div
            ref="mainContent"
            class="nhsuk-grid-column-two-thirds"
          >
            <div
              id="mainContent"
              tabindex="-1"
              :class="[mainClass, $style['main-container-desktop']]"
            >
              <div
                v-if="showError"
                :class="!$store.state.device.isNativeApp && $style.desktopWeb"
              >
                <session-error-api-error
                  :id="$style.serverError"
                  :class="$store.state.device.isNativeApp?'pull-content nhsuk-u-padding-top-7':''"
                >
                  <h1 v-if="!$store.state.device.isNativeApp"
                      class="nhsuk-u-margin-bottom-4 nhsuk-u-margin-top-4">
                    {{ $t('auth_return.errors.pageTitle') }} </h1>
                  <message-dialog
                    :override-style="overrideStyle"
                    message-type="error"
                    aria-live="polite"
                  >
                    <api-error-container v-if="errorStatusCode==464">
                      <api-error-title
                        title="auth_return.errors.464.title"
                        header="auth_return.errors.464.header"
                      />
                      <api-error-paragraph from="auth_return.errors.464.line2" />
                      <api-error-unordered-list from="auth_return.errors.464.uList" />
                      <api-error-paragraph
                        from="auth_return.errors.464.contactUs"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.464.message" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.464.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                      </div>
                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==465">
                      <api-error-title
                        title="auth_return.errors.465.title"
                        header="auth_return.errors.465.header"
                      />
                      <api-error-paragraph from="auth_return.errors.465.message" />

                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==400">
                      <api-error-title
                        title="auth_return.errors.400.title"
                        header="auth_return.errors.400.header"
                      />
                      <api-error-paragraph from="auth_return.errors.400.line1" />
                      <api-error-paragraph
                        from="auth_return.errors.400.line2"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.400.contactUs" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.400.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                        <api-error-button
                          from="auth_return.errors.400.backButtonText"
                          :action="loginUrl"
                        />
                      </div>
                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==403">
                      <api-error-title
                        title="auth_return.errors.403.title"
                        header="auth_return.errors.403.header"
                      />
                      <api-error-paragraph from="auth_return.errors.403.line1" />
                      <api-error-paragraph from="auth_return.errors.403.line2" />
                      <api-error-paragraph
                        from="auth_return.errors.403.line3"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.403.line4" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.403.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                        <api-error-button
                          from="auth_return.errors.403.backButtonText"
                          :action="loginUrl"
                        />
                      </div>
                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==500">
                      <api-error-title
                        title="auth_return.errors.500.title"
                        header="auth_return.errors.500.header"
                      />
                      <api-error-paragraph from="auth_return.errors.500.line1" />
                      <api-error-paragraph from="auth_return.errors.500.line3" />
                      <api-error-paragraph
                        from="auth_return.errors.500.line4"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.500.line5" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.500.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                        <api-error-button
                          from="auth_return.errors.500.backButtonText"
                          :action="loginUrl"
                        />
                      </div>
                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==502">
                      <api-error-title
                        title="auth_return.errors.502.title"
                        header="auth_return.errors.502.header"
                      />

                      <api-error-paragraph from="auth_return.errors.502.listTitle" />
                      <api-error-unordered-list from="auth_return.errors.502.uList" />
                      <api-error-paragraph from="auth_return.errors.502.line3" />
                      <api-error-paragraph
                        from="auth_return.errors.502.line4"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.502.message" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.502.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                        <api-error-button
                          from="auth_return.errors.502.backButtonText"
                          :action="loginUrl"

                        />
                      </div>
                    </api-error-container>
                    <api-error-container v-else-if="errorStatusCode==504">
                      <api-error-title
                        title="auth_return.errors.504.title"
                        header="auth_return.errors.504.header"
                      />
                      <api-error-paragraph from="auth_return.errors.504.listTitle" />
                      <api-error-unordered-list from="auth_return.errors.504.uList" />
                      <api-error-paragraph from="auth_return.errors.504.line3" />
                      <api-error-paragraph
                        from="auth_return.errors.504.line4"
                        :variable="serviceDeskReference"
                      />

                      <api-error-paragraph from="auth_return.errors.504.message" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.504.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                        />
                        <api-error-button
                          from="auth_return.errors.504.backButtonText"
                          :action="loginUrl"

                        />
                      </div>
                    </api-error-container>

                    <api-error-container v-else>
                      <api-error-title
                        title="auth_return.errors.default.title"
                        header="auth_return.errors.default.header"
                      />
                      <api-error-paragraph from="auth_return.errors.default.line1" />
                      <api-error-paragraph from="auth_return.errors.default.line3" />
                      <api-error-paragraph
                        from="auth_return.errors.default.line4"
                        :variable="serviceDeskReference"
                      />
                      <api-error-paragraph from="auth_return.errors.default.line5" />
                      <div :class="$style['api-error-button-container']">
                        <api-error-button
                          from="auth_return.errors.default.contactUsButtonText"
                          :action="contactUsUrl"
                          :target="target"
                          :class="$style['api-error-button']"
                        />
                        <api-error-button
                          from="auth_return.errors.default.backButtonText"
                          :action="loginUrl"
                        />
                      </div>
                    </api-error-container>

                  </message-dialog>
                </session-error-api-error>
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
    <div
      v-if="!$store.state.device.isNativeApp"
      :class="$style['footer-container-desktop']"
    >
      <web-footer />
    </div>
  </div>
</template>

<script>
import Sources from '@/lib/sources';
import HeaderSlim from '@/components/HeaderSlim';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import { LOGIN } from '@/lib/routes';
import NativeVersionSetup from '../services/nativeVersionSetup';
import SessionErrorApiError from '@/components/errors/sesssion-errors/SessionErrorApiError';
import ApiErrorContainer from '@/components/errors/sesssion-errors/ApiErrorContainer';
import ApiErrorTitle from '@/components/errors/sesssion-errors/ApiErrorTitle';
import ApiErrorParagraph from '@/components/errors/sesssion-errors/ApiErrorParagraph';
import ApiErrorButton from '@/components/errors/sesssion-errors/ApiErrorButton';
import ApiErrorUnorderedList from '@/components/errors/sesssion-errors/ApiErrorUnorderedList';
import MessageDialog from '@/components/widgets/MessageDialog';

export default {
  components: {
    HeaderSlim,
    WebHeader,
    WebFooter,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    SessionErrorApiError,
    ApiErrorContainer,
    ApiErrorTitle,
    ApiErrorParagraph,
    ApiErrorButton,
    ApiErrorUnorderedList,
    MessageDialog,
  },
  mixins: [ErrorMessageMixin],
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: this.$t('auth_return.errors.pageTitle'),
    };
  },
  computed: {
    serviceDeskReference() {
      return {
        swap: true,
        text: (this.$store.state.errors.apiErrors[0] || {}).serviceDeskReference || '',
        label: (this.$store.state.errors.apiErrors[0] || {}).serviceDeskReference || '',
      };
    },
    target() {
      return '_blank';
    },
    contactUsUrl() {
      return 'https://www.nhs.uk/contact-us/nhs-app-contact-us';
    },
    errorStatusCode() {
      return this.statusCode;
    },
    loginUrl() {
      return LOGIN.path;
    },
    headerTitle() {
      return this.showError()
        ? this.getMessage('header')
        : this.$store.state.header.headerText;
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
  },
  created() {
    const { source } = this.$route.query;
    if (source) {
      this.$store.dispatch('device/updateIsNativeApp', Sources.isNative(source));
      this.$store.dispatch('device/setSourceDevice', source);
    }
    this.$store.dispatch('pageTitle/updatePageTitle', this.$t('auth_return.errors.pageTitle'));
  },
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
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

    .api-error-button-container {
      display: inline-block;
      padding: 1em;
    }
}
</style>
