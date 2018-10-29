<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <glossary-header />
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isPatientDetailsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PATIENTDETAILS"
                           :text="$t('my_record.patientInfo.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.patientInfo.sectionHeader') }}
    </analytics-tracked-tag>
    <patient-details :is-collapsed="isPatientDetailsCollapsed"
                     :patient-details="patientDetails"/>

    <div v-if="myRecord.hasSummaryRecordAccess">
      <scr-emis v-if="myRecord.supplier === 'EMIS'"/>

      <scr-tpp v-if="myRecord.supplier === 'TPP'"/>

      <scr-vision v-if="myRecord.supplier === 'VISION'"/>

      <div v-if="myRecord.hasDetailedRecordAccess">

        <dcr-emis v-if="myRecord.supplier === 'EMIS'"/>

        <dcr-tpp v-if="myRecord.supplier === 'TPP'" ref="TPPChild"/>

        <dcr-vision v-if="myRecord.supplier === 'VISION'"/>

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
</template>

<script>
import PatientDetails from '@/components/my-record/SharedComponents/PatientDetails';
import DcrEmis from '@/components/my-record/DetailedCodedRecord/DcrEMIS';
import DcrTpp from '@/components/my-record/DetailedCodedRecord/DcrTPP';
import DcrVision from '@/components/my-record/DetailedCodedRecord/DcrVISION';
import ScrEmis from '@/components/my-record/SummaryCareRecord/ScrEMIS';
import ScrTpp from '@/components/my-record/SummaryCareRecord/ScrTPP';
import ScrVision from '@/components/my-record/SummaryCareRecord/ScrVISION';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GlossaryHeader from '@/components/GlossaryHeader';
import { MYRECORDWARNING, MYRECORDTESTRESULT } from '@/lib/routes';

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
  },
  beforeRouteEnter(to, from, next) {
    if (from.path === MYRECORDWARNING.path || from.path === MYRECORDTESTRESULT.path.split('/:')[0]) {
      next();
    } else {
      next(MYRECORDWARNING.path);
    }
  },
  data() {
    return {
      PATIENTDETAILS,
      isPatientDetailsCollapsed: true,
      hasLoaded: false,
      myRecord: {},
      patientDetails: {},
      clinicalAbbreviationsUrl: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
  mounted() {
    this.$store.app.$http
      .getV1PatientDemographics({})
      .then((data) => {
        this.patientDetails = data.response;
        this.isPatientDetailsCollapsed = false;
      }).then(() => {
        this.$store.app.$http
          .getV1PatientMyRecord({})
          .then((data) => {
            this.myRecord = data.response;
            this.hasLoaded = true;
          });
      });
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case PATIENTDETAILS:
          this.isPatientDetailsCollapsed = !this.isPatientDetailsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/medrecordtitle';

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
</style>
