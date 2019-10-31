<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="results.hasAccess"
                       :has-errored="results.hasErrored"
                       :class="[$style['record-content'], getCollapsedState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'],
                                         getCollapsedState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-if="supplier === 'VISION'">
      <a :class="$style.viewExaminations"
         :href="examinationsPath + nojsQuery"
         tabindex="0"
         @click.prevent="viewExaminations"
         @keypress="onKeyDown($event)">
        {{ $t('my_record.examinations.visionDetailsLink') }}
      </a>
    </div>
  </div>

</template>

<script>
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { key, redirectTo } from '@/lib/utils';
import { MY_RECORD_VISION_EXAMINATIONS_DETAIL } from '@/lib/routes';

export default {
  name: 'Examinations',
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
      examinationsPath: MY_RECORD_VISION_EXAMINATIONS_DETAIL.path,
      nojsQuery: `?nojs=${encodeURIComponent(this.$store.state.myRecord.nojsData)}`,
    };
  },
  computed: {
    getCollapsedState() {
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
    viewExaminations() {
      redirectTo(this, this.examinationsPath);
    },
    onKeyDown(e) {
      if (e.key === key.Enter) {
        this.viewExaminations();
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';

  .viewExaminations {
    padding: 1em;
    display: inline-block;
  }
</style>
