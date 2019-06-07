<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isAllergiesAndAdverseReactionsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ALLERGIESANDADVERSEREACTIONS"
                           :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
                           :aria-expanded="!isAllergiesAndAdverseReactionsCollapsed ?
                             'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.allergiesAndAdverseReactions.sectionHeader') }}
    </analytics-tracked-tag>
    <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                     :allergies="record.allergies" />
  </div>
</template>

<script>
import AllergiesAndAdverseReactions from '@/components/my-record/SharedComponents/AllergiesAndAdverseReactions';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';

export default {
  name: 'ScrMICROTEST',
  components: {
    AllergiesAndAdverseReactions,
    AnalyticsTrackedTag,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
      isAllergiesAndAdverseReactionsCollapsed: process.client,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case ALLERGIESANDADVERSEREACTIONS:
          this.isAllergiesAndAdverseReactionsCollapsed =
            !this.isAllergiesAndAdverseReactionsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';

  div {
   &.desktopWeb {
    .record-title {
    cursor: pointer;
    &:focus {
      @include outlineStyle
     }
    }
   }
  }
</style>
