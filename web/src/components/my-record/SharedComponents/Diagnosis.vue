<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="results.hasAccess"
                       :has-errored="results.hasErrored"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'],
                                         getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-if="supplier === 'VISION'">
      <a :class="$style.viewDiagnosis"
         tabindex="0"
         @click="viewDiagnosis($event)"
         @keypress="onKeyDown($event)">
        {{ $t('my_record.diagnosis.visionDetailsLink') }}
      </a>
    </div>
  </div>

</template>
<script>
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { MY_RECORD_VISION_DIAGNOSIS_DETAIL } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    results: {
      type: Object,
      default: () => {},
    },
    supplier: {
      type: String,
      default: '',
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    resultsData() {
      return this.results;
    },
    showError() {
      if (this.supplier === 'VISION') {
        return (this.results.rawHtml === null);
      }
      return this.results.hasErrored ||
                this.results.data.length === 0 ||
                !this.results.hasAccess;
    },
  },
  methods: {
    viewDiagnosis(event) {
      event.preventDefault();
      redirectTo(this, MY_RECORD_VISION_DIAGNOSIS_DETAIL.path, null);
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.viewDiagnosis(e);
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';

  .viewDiagnosis {
    padding: 1em;
    font-size: 0.875em;
  }

  div {
   &.desktopWeb {
    a {
     cursor: pointer;
     &:focus {
      @include outlineStyle
     }
    }
   }
  }

</style>
