<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <!-- eslint-disable-next-line vue/no-v-html -->
      <div id="document" class="documentContainer nhsuk-u-margin-top-5" v-html="documentData"/>
      <glossary/>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="documentPath"
                                 @clickAndPrevent="backToDocumentsClicked"/>
    </div>
  </div>
</template>

<script>
import NativeAppCallbacks from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { GP_MEDICAL_RECORD_PATH, DOCUMENT_PATH, LOGOUT_PATH, LOGIN_PATH } from '@/router/paths';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';
import { redirectTo } from '@/lib/utils';
import {
  CLINICAL_ABBREVIATIONS_URL,
} from '@/router/externalLinks';

export default {
  components: {
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
      glossaryLinkURL: CLINICAL_ABBREVIATIONS_URL,
      documentData: null,
    };
  },
  computed: {
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
    documentPath() {
      return DOCUMENT_PATH.replace(':id', this.$route.params.id);
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    // TPP document would already have been loaded into the store at this point.
    if (!this.$store.state.documents.currentDocument.data) {
      await this.$store.dispatch('documents/loadDocument', { documentIdentifier: this.$route.params.id });
    }

    this.documentData = this.$store.state.documents.currentDocument.data;

    if (this.documentData) {
      this.navHidden = true;
      NativeAppCallbacks.hideHeader();
      NativeAppCallbacks.hideMenuBar();
    }

    // Need to set user-scalable=yes
    // and maximum-scale=10.0 options
    // from the content attribute so user can zoom
    // on their document.
    this.setZoom(true);
    EventBus.$emit(FOCUS_NHSAPP_TITLE);
  },
  beforeDestroy() {
    // Set the user-scalable=0 and maxium-scale=1.0
    // when the component is destroyed.
    this.setZoom(false);
  },
  methods: {
    backToDocumentsClicked() {
      this.$router.go(-1);
    },
    setZoom(zoomable) {
      const viewport = document.getElementsByName('viewport')[0];
      let content = 'width=device-width, initial-scale=1, minimum-scale=1.0';
      if (!zoomable) {
        content += ', maximum-scale=1.0, user-scalable=0';
      } else {
        content += ', maximum-scale=10.0, user-scalable=yes';
      }
      viewport.setAttribute('content', content);

      // Android needs a native callback to set zoom on the webview natively.
      if (this.isAndroid) {
        NativeAppCallbacks.setZoomable(zoomable);
      }
    },
  },
  beforeRouteLeave(to, from, next) {
    if (!(to.path === LOGIN_PATH || to.path === LOGOUT_PATH) && this.navHidden) {
      NativeAppCallbacks.showHeader();
      NativeAppCallbacks.showMenuBar();
    }
    next();
  },
};
</script>

<style lang="scss">
  .documentContainer {
    margin-bottom: 40px;
    word-break: break-word;

    img {
      max-width: 100%;
    }
  }
</style>
