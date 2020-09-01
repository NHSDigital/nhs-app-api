<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p v-if="!isViewable">{{ $t('messages.toAccessContactSurgery') }}</p>
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
import get from 'lodash/fp/get';
import NativeAppCallbacks from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';
import { UPDATE_HEADER, UPDATE_TITLE, FOCUS_NHSAPP_ROOT, EventBus } from '@/services/event-bus';
import {
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_PATH,
  LOGOUT_PATH,
  LOGIN_PATH,
} from '@/router/paths';
import {
  CLINICAL_ABBREVIATIONS_URL,
} from '@/router/externalLinks';

export default {
  name: 'GpMessagesViewAttachmentPage',
  components: {
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
      isViewable: get('$store.state.documents.currentDocument.isViewable', this),
      glossaryLinkURL: CLINICAL_ABBREVIATIONS_URL,
    };
  },
  computed: {
    attachment() {
      return this.$store.state.documents.currentDocument.data;
    },
    messagePath() {
      return GP_MESSAGES_VIEW_MESSAGE_PATH;
    },
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
  },
  created() {
    if (!this.$store.state.gpMessages.selectedMessageDetails ||
        !this.$store.state.documents.currentDocument) {
      redirectTo(this, GP_MESSAGES_PATH);
      return;
    }

    if (!this.isViewable) {
      EventBus.$emit(UPDATE_HEADER, 'pageHeaders.gpMessagesAttachmentUnavailable');
      EventBus.$emit(UPDATE_TITLE, 'pageTitles.gpMessagesAttachmentUnavailable');
    }

    if (this.document) {
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
    if (!(to.path === LOGIN_PATH || to.path === LOGOUT_PATH) && this.navHidden) {
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
