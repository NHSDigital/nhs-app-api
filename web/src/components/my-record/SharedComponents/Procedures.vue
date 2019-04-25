<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="results.hasAccess"
                       :has-errored="results.hasErrored"
                       :class="[$style['record-content'], getCollapsedState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content']
                                         , getCollapsedState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-if="supplier === 'VISION'">
      <a :class="$style.viewProcedures"
         :href="proceduresPath + nojsQuery"
         tabindex="0"
         @click.prevent="viewProcedures"
         @keypress="onKeyDown($event)">
        {{ $t('my_record.procedures.visionDetailsLink') }}
      </a>
    </div>
  </div>

</template>

<script>
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { MY_RECORD_VISION_PROCEDURES_DETAIL } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Procedures',
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
      proceduresPath: MY_RECORD_VISION_PROCEDURES_DETAIL.path,
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
    viewProcedures() {
      redirectTo(this, this.proceduresPath, null);
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.viewProcedures();
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';

  .viewProcedures {
    padding: 1em;
    font-size: 0.875em;
  }

  div {
   &.desktopWeb {
    .viewProcedures {
     cursor: pointer;
     &:focus {
      @include outlineStyle
     }
    }
   }
  }

</style>
