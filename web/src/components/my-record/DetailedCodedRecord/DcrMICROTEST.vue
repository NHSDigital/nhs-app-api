<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           :aria-expanded="!isImmunisationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.immunisations.sectionHeader') }}
    </analytics-tracked-tag>
    <immunisations :is-collapsed="isImmunisationsCollapsed" :immunisations="record.immunisations" />
  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const IMMUNISATIONS = 'immunisations';


export default {
  name: 'DcrMICROTEST',
  components: {
    AnalyticsTrackedTag,
    Immunisations,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      IMMUNISATIONS,
      isImmunisationsCollapsed: process.client,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case IMMUNISATIONS:
          this.isImmunisationsCollapsed =
            !this.isImmunisationsCollapsed;
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
