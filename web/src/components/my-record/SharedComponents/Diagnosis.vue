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
         :href="diagnosisPath + nojsQuery"
         tabindex="0"
         @click.prevent="viewDiagnosis"
         @keypress.enter="viewDiagnosis">
        {{ $t('my_record.diagnosis.visionDetailsLink') }}
      </a>
    </div>
  </div>

</template>
<script>
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { redirectTo } from '@/lib/utils';
import { MY_RECORD_VISION_DIAGNOSIS_DETAIL } from '@/lib/routes';

export default {
  name: 'Diagnosis',
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
  data() {
    return {
      diagnosisPath: MY_RECORD_VISION_DIAGNOSIS_DETAIL.path,
      nojsQuery: `?nojs=${encodeURIComponent(this.$store.state.myRecord.nojsData)}`,
    };
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
    viewDiagnosis() {
      redirectTo(this, this.diagnosisPath);
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';

  .viewDiagnosis {
    padding: 1em;
    display: inline-block;
  }

</style>
