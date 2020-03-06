<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div id="document" class="documentContainer nhsuk-u-margin-top-5" v-html="document"/>
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
import { GP_MEDICAL_RECORD, LOGIN, LOGOUT, DOCUMENT } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
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
    document() {
      return this.$store.state.myRecord.document.data;
    },
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
    documentPath() {
      return DOCUMENT.path.replace(':id', this.$route.params.id);
    },
  },
  async asyncData({ store, route, redirect }) {
    if (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirect(GP_MEDICAL_RECORD.path);
      return;
    }

    await store.dispatch('myRecord/loadDocument', route.params.id);
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
    if (!(to.path === LOGIN.path || to.path === LOGOUT.path) && this.navHidden) {
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
