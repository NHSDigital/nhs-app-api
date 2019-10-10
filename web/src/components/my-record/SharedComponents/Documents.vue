<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="documents.hasAccess"
                       :has-errored="documents.hasErrored"
                       :class="[$style['record-content'],
                                getCollapsedState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed"
       :class="[$style['record-content'],
                getCollapsedState,
                !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <p v-if="supplier === 'EMIS'">
      <a :class="$style.viewDocuments"
         tabindex="0"
         @click="viewDocuments"
         @keypress="onKeyDown($event)">
        {{ $t('my_record.documents.documentsLink') }}
      </a>
    </p>
  </div>
</template>

<script>
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { MY_RECORD_DOCUMENTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Documents',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    documents: {
      type: Object,
      default: () => {},
    },
    supplier: {
      type: String,
      default: '',
    },
  },
  computed: {
    getCollapsedState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    showError() {
      return this.documents.hasErrored ||
             this.documents.data.length === 0 ||
             !this.documents.hasAccess;
    },
  },
  methods: {
    viewDocuments() {
      redirectTo(this, MY_RECORD_DOCUMENTS.path, null);
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.viewDocuments();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';

  .viewDocuments {
    padding: 1em;
    display: inline-block;
  }

</style>
