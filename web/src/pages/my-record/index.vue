<template>
  <div v-if="$store.state.myRecord.hasAcceptedTerms"
       :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
      <glossary-header :extra-classes="[$style.glossary]"/>
      <analytics-tracked-tag :class="[$style['record-title'],
                                      getCollapsedState(isPatientDetailsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="PATIENTDETAILS"
                             :text="$t('my_record.patientInfo.sectionHeader')"
                             data-purpose="accordion"
                             role="button"
                             tag="a"
                             tabindex="0">
        {{ $t('my_record.patientInfo.sectionHeader') }}
      </analytics-tracked-tag>
      <div :class="$style.patientDetailsContainer">
        <patient-details :is-collapsed="isPatientDetailsCollapsed"
                         :patient-details="$store.state.myRecord.patientDetails"/>
      </div>

      <div v-if="hasRecordAccess()" :class="$style.summaryRecordContainer">

        <template v-if="hasSummaryRecordAccess">
          <scr-emis v-if="supplier === 'EMIS'" :record="$store.state.myRecord.record"/>

          <scr-tpp v-if="supplier === 'TPP'" :record="$store.state.myRecord.record"/>

          <scr-vision v-if="supplier === 'VISION'" :record="$store.state.myRecord.record"/>
        </template>

        <div v-if="hasDetailedRecordAccess">

          <dcr-emis v-if="supplier === 'EMIS'" :record="$store.state.myRecord.record"/>

          <dcr-tpp v-if="supplier === 'TPP'"
                   ref="TPPChild"
                   :record="$store.state.myRecord.record"/>

          <dcr-vision v-if="supplier === 'VISION'" :record="$store.state.myRecord.record"/>

        </div>

        <div v-else>
          <p :class="$style.summaryRecordWarning">
            {{ $t('my_record.viewRestOfHealthRecordWarning') }}
          </p>
        </div>
      </div>
      <div v-else class="pull-content">
        <div v-if="hasLoaded">
          <div id="errorMsg" :class="$style.info">
            <p><strong style="margin-top: 0.5em;">
              {{ $t('my_record.noRecordAccess.warningHeader') }}
            </strong></p>
            <p>{{ $t('my_record.noRecordAccess.warningBody') }} </p>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else>
    <Warning />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import PatientDetails from '@/components/my-record/SharedComponents/PatientDetails';
import DcrEmis from '@/components/my-record/DetailedCodedRecord/DcrEMIS';
import DcrTpp from '@/components/my-record/DetailedCodedRecord/DcrTPP';
import DcrVision from '@/components/my-record/DetailedCodedRecord/DcrVISION';
import ScrEmis from '@/components/my-record/SummaryCareRecord/ScrEMIS';
import ScrTpp from '@/components/my-record/SummaryCareRecord/ScrTPP';
import ScrVision from '@/components/my-record/SummaryCareRecord/ScrVISION';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GlossaryHeader from '@/components/GlossaryHeader';
import Warning from '@/components/my-record/Warning';

const PATIENTDETAILS = 'patientdetails';

export default {
  components: {
    PatientDetails,
    AnalyticsTrackedTag,
    GlossaryHeader,
    DcrEmis,
    DcrTpp,
    DcrVision,
    ScrEmis,
    ScrTpp,
    ScrVision,
    Warning,
  },
  data() {
    return {
      PATIENTDETAILS,
      clinicalAbbreviationsUrl: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
  computed: {
    supplier() {
      return get('$store.state.myRecord.record.supplier')(this);
    },
    hasDetailedRecordAccess() {
      return get('$store.state.myRecord.record.hasDetailedRecordAccess')(this);
    },
    hasAcceptedTerms() {
      return get('$store.state.myRecord.hasAcceptedTerms')(this);
    },
    hasLoaded() {
      return get('$store.state.myRecord.hasLoaded')(this);
    },
    hasSummaryRecordAccess() {
      return get('$store.state.myRecord.record.hasSummaryRecordAccess')(this);
    },
    isPatientDetailsCollapsed() {
      return get('$store.state.myRecord.isPatientDetailsCollapsed')(this);
    },
  },
  async asyncData({ store }) {
    if (store.state.myRecord.hasAcceptedTerms) {
      await store.dispatch('myRecord/acceptTerms');
      await store.dispatch('myRecord/load');
    }
  },
  updated() {
    window.scrollTo(0, 0);
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case PATIENTDETAILS:
          this.$store.dispatch('myRecord/togglePatientDetail');
          break;
        default:
          break;
      }
    },
    hasRecordAccess() {
      return this.hasSummaryRecordAccess || this.hasDetailedRecordAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/medrecordtitle';
  @import '../../style/desktopWeb/accessibility';

  .no-padding {
    margin-left: -1em;
    margin-right: -1em;
  }

  .info {
    padding: 0.5em 1em 0em 1em;
    margin-bottom: 0.5em;
    font-size: 1em;
    p {
      padding-bottom: 0.5em;
      padding-top: 0.5em;
    }
  }

  p {
    display: block;
    font-weight: normal;
    font-size: 1em;
    line-height: 1.5em;
    color: #212B32;
  }

  .summaryRecordWarning {
    padding-left:1em;
    padding-right:1em;
    padding-top:0.5em;
  }
  .glossary {
    padding: 0.5em 1em 0em 1em;
  }
  div {
   &.desktopWeb {
    &>* {
        max-width: 540px;
      }

    p {
     font-family: $default_web;
     font-weight: lighter;
    }

    .record-content {
     margin-left: 1em;
    }

    .summaryRecordContainer {
     padding-left: 1em;
     padding-bottom: 2em;
     margin-right: 1em;
    }

    .record-title {
     padding-left: 1em;
     margin-left: 1em;
     margin-right: 1em;
     cursor: pointer;
    }

    .record-title:focus {
     @include outlineStyle
    }


    .patientDetailsContainer {
     margin-left: 1em;
     margin-right: 1em;
    }
   }
  }
</style>
