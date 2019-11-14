<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <glossary :extra-classes="[$style.glossary]"/>
      <div class="documentContainer" v-html="document"/>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 @clickAndPrevent="backToDocumentsClicked"/>
    </div>
  </div>
</template>

<script>
import NativeAppCallbacks from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { MYRECORD, MY_RECORD_DOCUMENTS, LOGIN, LOGOUT } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';
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
      documentsPath: `${MY_RECORD_DOCUMENTS.path}#document-${this.$route.params.id}`,
    };
  },
  computed: {
    document() {
      return this.$store.state.myRecord.document.data;
    },
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
  },
  async asyncData({ store, route, redirect }) {
    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED)
      || (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning())
    ) {
      redirect(MYRECORD.path);
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
      redirectTo(this, this.documentsPath, null);
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

    img {
      max-width: 100%;
    }
  }
</style>
<style module lang="scss" scoped>
  .glossary {
    padding: 0.5em 1em 0em 1em;
  }
</style>
