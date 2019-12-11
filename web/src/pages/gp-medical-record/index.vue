<template>
  <div v-if="$store.state.myRecord.hasAcceptedTerms || hasAgreedToMedicalWarning()">
    <div v-if="showTemplate && !isProxying" id="mainDiv" data-sid="user-info-details">
      <div v-if="showPatientDetails">
        <h2 data-sid="patient-name"
            :class="['nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-3 ' +
              'nhsuk-u-margin-bottom-0']"
            data-hj-suppress>
          {{ $store.state.myRecord.patientDetails.patientName }}
        </h2>
        <p class="nhsuk-label nhsuk-u-margin-top-0
                  nhsuk-u-padding-bottom-0 nhsuk-u-font-weight-bold">
          {{ $t('my_record.patientInfo.fieldLabelDOB') }}
        </p>
        <p data-sid="user-date-of-birth"
           :class="[$style['user-info'],
                    'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3' +
                      'nhsuk-u-margin-bottom-0']">
          {{ $store.state.myRecord.patientDetails.dateOfBirth | longDate }}
        </p>
        <p class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0
                  nhsuk-u-font-weight-bold">
          {{ $t('my_record.patientInfo.fieldLabelNHS') }}:
        </p>
        <p data-sid="user-nhs-number"
           :class="[$style['user-info'],
                    'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3 nhsuk-u-margin-bottom-0']">
          {{ $store.state.myRecord.patientDetails.nhsNumber }}
        </p>
        <p class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0
                  nhsuk-u-font-weight-bold">
          {{ $t('my_record.patientInfo.fieldLabelAddress') }}:
        </p>
        <p data-sid="user-address"
           :class="[$style['user-info'],
                    'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-5 nhsuk-u-margin-bottom-0']"
           data-hj-suppress>
          {{ $store.state.myRecord.patientDetails.address }}
        </p>
      </div>
    </div>

    <proxy-patient-details v-else-if="isProxying"
                           :proxy-patient-details="$store.state.linkedAccounts.actingAsUser"/>

    <div v-if="showTemplate && hasRecordAccess()" :class="$style.summaryRecordContainer"
         data-purpose="medical-record-menu">
      <menu-item-list>
        <template v-if="hasSummaryRecordAccess">
          <scr-emis-gp-record v-if="supplier === 'EMIS'"/>

          <scr-tpp-gp-record v-if="supplier === 'TPP'"/>

          <scr-vision-gp-record v-if="supplier === 'VISION'"/>

          <scr-microtest-gp-record v-if="supplier === 'MICROTEST'"/>
        </template>

        <template v-if="hasDetailedRecordAccess">
          <dcr-emis-gp-record v-if="supplier === 'EMIS'"/>

          <dcr-tpp-gp-record v-if="supplier === 'TPP'"/>

          <dcr-vision-gp-record v-if="supplier === 'VISION'"/>

          <dcr-microtest-gp-record v-if="supplier === 'MICROTEST'"/>
        </template>

        <template v-else>
          <p :class="$style.summaryRecordWarning">
            {{ $t('my_record.viewRestOfHealthRecordWarning') }}
          </p>
        </template>
      </menu-item-list>
      <glossary/>
    </div>
    <div v-else class="pull-content">
      <div v-if="hasLoaded">
        <div v-if="isProxying" :class="[$style['info'], 'nhsuk-u-margin-top-3']">
          <shutter :feature="'medicalRecord'" />
        </div>
        <div v-else id="errorMsg" :class="[$style['record-content'], 'nhsuk-u-margin-bottom-6']">
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
  <div v-else>
    <Warning />
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DcrEmisGpRecord from '@/components/gp-medical-record/DetailedCodedRecord/DcrEMISGpRecord';
import DcrTppGpRecord from '@/components/gp-medical-record/DetailedCodedRecord/DcrTPPGpRecord';
import DcrVisionGpRecord from '@/components/gp-medical-record/DetailedCodedRecord/DcrVISIONGpRecord';
import DcrMicrotestGpRecord from '@/components/gp-medical-record/DetailedCodedRecord/DcrMICROTESTGpRecord';
import ScrEmisGpRecord from '@/components/gp-medical-record/SummaryCareRecord/ScrEMISGpRecord';
import ScrTppGpRecord from '@/components/gp-medical-record/SummaryCareRecord/ScrTPPGpRecord';
import ScrVisionGpRecord from '@/components/gp-medical-record/SummaryCareRecord/ScrVISIONGpRecord';
import ScrMicrotestGpRecord from '@/components/gp-medical-record/SummaryCareRecord/ScrMICROTESTGpRecord';
import MenuItemList from '@/components/MenuItemList';
import Glossary from '@/components/Glossary';
import Warning from '@/components/my-record/Warning';
import agreedToMedicalWarning from '@/lib/sessionStorage';
import Shutter from '@/components/linked-profiles/Shutter';
import ProxyPatientDetails from '@/components/my-record/SharedComponents/ProxyPatientDetails';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

const PATIENTDETAILS = 'patientdetails';

export default {
  layout: 'nhsuk-layout',
  components: {
    Glossary,
    DcrEmisGpRecord,
    DcrTppGpRecord,
    DcrVisionGpRecord,
    DcrMicrotestGpRecord,
    ScrEmisGpRecord,
    ScrTppGpRecord,
    ScrVisionGpRecord,
    ScrMicrotestGpRecord,
    MenuItemList,
    Warning,
    Shutter,
    ProxyPatientDetails,
  },
  data() {
    return {
      PATIENTDETAILS,
      hasAgreed: false,
      isProxying: this.$store.getters['session/isProxying'],
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
    showPatientDetails() {
      return (this.$store.state.myRecord.patientDetails.patientName ||
        this.$store.state.myRecord.patientDetails.dateOfBirth ||
        this.$store.state.myRecord.patientDetails.nhsNumber ||
        this.$store.state.myRecord.patientDetails.address);
    },
  },
  async asyncData({ store }) {
    if (store.state.myRecord.hasAcceptedTerms) {
      await store.dispatch('myRecord/clear');
      await store.dispatch('myRecord/acceptTerms');
      await store.dispatch('myRecord/load');
    }

    return {
      medicalRecord: store.state.myRecord.record,
    };
  },
  updated() {
    window.scrollTo(0, 0);
  },
  async mounted() {
    if (this.shouldLoadRecord()) {
      await this.$store.dispatch('myRecord/clear');
      await this.$store.dispatch('myRecord/acceptTerms');
      await this.$store.dispatch('myRecord/load');
      EventBus.$emit(FOCUS_NHSAPP_ROOT);
    }
  },
  methods: {
    hasRecordAccess() {
      return this.hasSummaryRecordAccess || this.hasDetailedRecordAccess;
    },
    hasAgreedToMedicalWarning() {
      this.hasAgreed = agreedToMedicalWarning();
      return this.hasAgreed;
    },
    shouldLoadRecord() {
      const previousPath = this.$router.previousPaths[this.$router.previousPaths.length - 1];

      if (!this.hasAgreed) return false;
      if (!this.hasLoaded || !previousPath.includes('gp-medical-record')) return true;

      return false;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/medrecordcontent';

  .user-info {
    display: inline-block;
  }
  .user-info-name {
    display: inline-block;
  }
</style>
