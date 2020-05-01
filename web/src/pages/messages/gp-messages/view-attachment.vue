<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p v-if="!isViewable">{{ $t('my_record.documents.documentUnavailableSubtext') }}</p>
      <div v-else>
        <div id="document" class="attachmentContainer nhsuk-u-margin-top-5" v-html="attachment"/>
        <glossary/>
      </div>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="messagePath"
                                 @clickAndPrevent="backToMessageClicked"/>
    </div>
  </div>
</template>

<script>
import NativeAppCallbacks from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { GP_MESSAGES, LOGIN, LOGOUT, GP_MESSAGES_VIEW_MESSAGE } from '@/lib/routes';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
  computed: {
    attachment() {
      return this.$store.state.documents.currentDocument.data;
    },
    messagePath() {
      return GP_MESSAGES_VIEW_MESSAGE.path;
    },
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
  },
  async asyncData({ store, redirect }) {
    if (!store.state.gpMessages.selectedMessageDetails ||
      !store.state.documents.currentDocument) {
      return redirect(GP_MESSAGES.path);
    }
    const { isViewable } = store.state.documents.currentDocument;
    if (!isViewable) {
      store.dispatch('header/updateHeaderText',
        store.app.i18n.t('pageHeaders.gpMessagesAttachmentUnavailable'));
      store.dispatch('pageTitle/updatePageTitle',
        store.app.i18n.t('pageTitles.gpMessagesAttachmentUnavailable'));
    }

    return { isViewable };
  },
  created() {
    if (this.document && process.client) {
      this.navHidden = true;
      NativeAppCallbacks.hideHeader();
      NativeAppCallbacks.hideMenuBar();
    }
  },
  mounted() {
    // Need to set user-scalable=yes
    // and maximum-scale=10.0 options
    // from the content attribute so user can zoom
    // on their document.
    this.setZoom(true);
    EventBus.$emit(FOCUS_NHSAPP_ROOT);
  },
  beforeDestroy() {
    // Set the user-scalable=0 and maxium-scale=1.0
    // when the component is destroyed.
    this.setZoom(false);
  },
  methods: {
    backToMessageClicked() {
      this.$router.go(-1);
    },
    setZoom(zoomable) {
      const viewport = document.getElementsByName('viewport')[0];
      let content = 'width=device-width, initial-scale=1, minimum-scale=1.0';
      content += (!zoomable) ? ', maximum-scale=1.0, user-scalable=0' :
        ', maximum-scale=10.0, user-scalable=yes';
      viewport.setAttribute('content', content);

      // Android needs a native callback to set zoom on the webview natively.
      if (this.isAndroid) {
        NativeAppCallbacks.setZoomable(zoomable);
      }
    },
  },
  beforeRouteLeave(to, from, next) {
    if (!(to.path === LOGIN.path || to.path === LOGOUT.path) && this.navHidden) {
      NativeAppCallbacks.showHeader();
      NativeAppCallbacks.showMenuBar();
    }
    next();
  },
};
</script>

<style lang="scss">
  .attachmentContainer {
    margin-bottom: 40px;
    word-break: break-word;

    img {
      max-width: 100%;
    }
  }
</style>
