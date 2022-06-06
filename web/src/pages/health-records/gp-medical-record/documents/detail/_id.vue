<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <!-- eslint-disable-next-line vue/no-v-html -->
      <div id="document" class="documentContainer nhsuk-u-margin-top-5" v-html="documentData"/>
      <glossary/>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="documentPath"/>
    </div>
  </div>
</template>

<script>
import NativeApp from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { GP_MEDICAL_RECORD_PATH, DOCUMENT_PATH, LOGOUT_PATH, LOGIN_PATH } from '@/router/paths';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
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
      NativeApp.hideHeader();
      NativeApp.hideMenuBar();
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
        NativeApp.setZoomable(zoomable);
      }
    },
  },
  beforeRouteLeave(to, from, next) {
    if (!(to.path === LOGIN_PATH || to.path === LOGOUT_PATH) && this.navHidden) {
      NativeApp.showHeader();
      NativeApp.showMenuBar();
    }
    next();
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/health-records-gp-medical-record-document-detail-id";
</style>
