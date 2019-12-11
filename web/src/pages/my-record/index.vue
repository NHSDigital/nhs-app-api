<template>
  <div v-if="$store.state.myRecord.hasAcceptedTerms || hasAgreedToMedicalWarning()"
       :class="!$store.state.device.isNativeApp">
    <div v-if="showTemplate" id="mainDiv">
      <glossary/>
      <analytics-tracked-tag :class="['nhsuk-heading-s',
                                      'nhsuk-u-padding-3',
                                      'record-title',
                                      getCollapsedState(isPatientDetailsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="PATIENTDETAILS"
                             :text="$t('my_record.patientInfo.sectionHeader')"
                             :aria-expanded="!isPatientDetailsCollapsed ? 'true' : 'false'"
                             data-purpose="accordion"
                             role="button"
                             tag="a">
        <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
          {{ $t('my_record.patientInfo.sectionHeader') }}</h2>
      </analytics-tracked-tag>

      <div :class="[$style.patientDetailsContainer, $style['nhsuk-u-padding-bottom-3']]">
        <patient-details v-if="!isProxying" :is-collapsed="isPatientDetailsCollapsed"
                         :patient-details="$store.state.myRecord.patientDetails"/>
        <proxy-patient-details v-else-if="isProxying" :is-collapsed="isPatientDetailsCollapsed"
                               :proxy-patient-details="$store.state.linkedAccounts.actingAsUser"/>
      </div>

      <div v-if="hasRecordAccess()" :class="$style.summaryRecordContainer">

        <template v-if="hasSummaryRecordAccess">
          <scr-emis v-if="supplier === 'EMIS'" :record="$store.state.myRecord.record"/>

          <scr-tpp v-if="supplier === 'TPP'" :record="$store.state.myRecord.record"/>

          <scr-vision v-if="supplier === 'VISION'" :record="$store.state.myRecord.record"/>

          <scr-microtest v-if="supplier === 'MICROTEST'" :record="$store.state.myRecord.record"/>

        </template>

        <div v-if="hasDetailedRecordAccess">

          <dcr-emis v-if="supplier === 'EMIS'" :record="$store.state.myRecord.record"/>

          <dcr-tpp v-if="supplier === 'TPP'"
                   ref="TPPChild"
                   :record="$store.state.myRecord.record"/>

          <dcr-vision v-if="supplier === 'VISION'" :record="$store.state.myRecord.record"/>

          <dcr-microtest v-if="supplier === 'MICROTEST'" :record="$store.state.myRecord.record"/>
        </div>

        <div v-else>
          <p class="nhsuk-u-padding-top-3">
            {{ $t('my_record.viewRestOfHealthRecordWarning') }}
          </p>
        </div>
      </div>
      <div v-else class="pull-content">
        <div v-if="hasLoaded">
          <div v-if="isProxying" :class="[$style['info'], 'nhsuk-u-margin-top-3']">
            <shutter :feature="'medicalRecord'" />
          </div>
          <div v-else id="errorMsg" :class="$style.info">
            <p><strong style="margin-top: 0.5em;">
              {{ $t( supplier === 'MICROTEST' ?
                'my_record.noRecordsOrNoAccess.warningHeader' :
                'my_record.noRecordAccess.warningHeader') }}
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
import DcrMicrotest from '@/components/my-record/DetailedCodedRecord/DcrMICROTEST';
import ScrEmis from '@/components/my-record/SummaryCareRecord/ScrEMIS';
import ScrTpp from '@/components/my-record/SummaryCareRecord/ScrTPP';
import ScrVision from '@/components/my-record/SummaryCareRecord/ScrVISION';
import ScrMicrotest from '@/components/my-record/SummaryCareRecord/ScrMICROTEST';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Glossary from '@/components/Glossary';
import Warning from '@/components/my-record/Warning';
import agreedToMedicalWarning from '@/lib/sessionStorage';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import Shutter from '@/components/linked-profiles/Shutter';
import ProxyPatientDetails from '@/components/my-record/SharedComponents/ProxyPatientDetails';

const PATIENTDETAILS = 'patientdetails';

export default {
  layout: 'nhsuk-layout',
  components: {
    PatientDetails,
    AnalyticsTrackedTag,
    DcrEmis,
    DcrTpp,
    DcrVision,
    DcrMicrotest,
    ScrEmis,
    ScrTpp,
    ScrVision,
    ScrMicrotest,
    Glossary,
    Warning,
    Shutter,
    ProxyPatientDetails,
  },
  data() {
    return {
      PATIENTDETAILS,
      hasAgreed: false,
      isProxying: this.$store.getters['session/isProxying'],
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
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
  async mounted() {
    if (this.hasAgreed) {
      await this.$store.dispatch('myRecord/acceptTerms');
      await this.$store.dispatch('myRecord/load');
      EventBus.$emit(FOCUS_NHSAPP_ROOT);
    }
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? 'closed' : 'opened';
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
    hasAgreedToMedicalWarning() {
      this.hasAgreed = agreedToMedicalWarning();
      return this.hasAgreed;
    },
  },
};

</script>
<style lang="scss">
  @import '../../style/medrecordtitle';
</style>
<style module lang="scss" scoped>

  @import '../../style/desktopWeb/accessibility';

  .summaryRecordWarning {
    padding-left: 1em;
    padding-right: 1em;
    padding-top: 0.5em;
  }

  .glossary {
    padding: 0.5em 1em 0em 1em;
  }

  div {
    &.desktopWeb {
      & > * {
        max-width: 540px;
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

      .patientDetailsContainer {
        margin-left: 1em;
        margin-right: 1em;
      }
    }
  }
</style>
