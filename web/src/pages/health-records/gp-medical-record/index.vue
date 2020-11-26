<template>
  <div v-if="$store.state.myRecord.hasAcceptedTerms || hasAgreedToMedicalWarning">
    <div v-if="showPatientDetails"
         id="mainDiv"
         data-sid="user-info-details">
      <h2 data-sid="patient-name"
          :class="['nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-3 ' +
            'nhsuk-u-margin-bottom-0']"
          data-hj-suppress>
        {{ $store.state.myRecord.patientDetails.patientName }}
      </h2>
      <p class="nhsuk-label nhsuk-u-margin-top-0
                  nhsuk-u-padding-bottom-0 nhsuk-u-font-weight-bold">
        {{ $t('myRecord.dateOfBirth') }}
      </p>
      <p data-sid="user-date-of-birth"
         :class="[$style['user-info'],
                  'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3' +
                    'nhsuk-u-margin-bottom-0']">
        {{ $store.state.myRecord.patientDetails.dateOfBirth | longDate }}
      </p>
      <p class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0
                  nhsuk-u-font-weight-bold">
        {{ $t('myRecord.nhsNumber') }}
      </p>
      <p data-sid="user-nhs-number"
         :class="[$style['user-info'],
                  'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-3 nhsuk-u-margin-bottom-0']">
        {{ $store.state.myRecord.patientDetails.nhsNumber }}
      </p>
      <p class="nhsuk-label nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0
                  nhsuk-u-font-weight-bold">
        {{ $t('myRecord.address') }}
      </p>
      <p data-sid="user-address"
         :class="[$style['user-info'],
                  'nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-5 nhsuk-u-margin-bottom-0']"
         data-hj-suppress>
        {{ $store.state.myRecord.patientDetails.address }}
      </p>
    </div>

    <proxy-patient-details v-else-if="showTemplate && isProxying"
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
      </menu-item-list>
      <template v-if="!hasDetailedRecordAccess">
        <p>
          {{ $t('myRecord.thisIsASummaryToViewMoreContactSurgery') }}
        </p>
      </template>
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
              'myRecord.thisInfoIsNotAvailabeInTheApp' :
              'myRecord.youDoNotHaveAccessToYourRecord') }}
          </strong></p>
          <p>{{ $t('myRecord.contactSurgeryForMoreInformation') }} </p>
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
import ProxyPatientDetails from '@/components/gp-medical-record/SharedComponents/ProxyPatientDetails';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';

const PATIENTDETAILS = 'patientdetails';

export default {
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
      return (this.showTemplate &&
           !this.isProxying &&
           this.$store.getters['myRecord/patientDetailsExist']);
    },
    hasAgreedToMedicalWarning() {
      return agreedToMedicalWarning('agreedToMedicalWarning');
    },
  },
  updated() {
    window.scrollTo(0, 0);
  },
  async mounted() {
    if (this.shouldLoadRecord()) {
      await this.$store.dispatch('myRecord/clear');
      await this.$store.dispatch('myRecord/acceptTerms');
      await this.$store.dispatch('myRecord/load');
    }

    this.$store.dispatch('myRecord/reload', true);
    EventBus.$emit(FOCUS_NHSAPP_TITLE);
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    hasRecordAccess() {
      return this.hasSummaryRecordAccess || this.hasDetailedRecordAccess;
    },
    shouldLoadRecord() {
      if (!this.hasAgreedToMedicalWarning) return false;
      if (!this.hasLoaded) return true;
      return this.$store.state.myRecord.reload;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';

  .user-info {
    display: inline-block;
  }
  .user-info-name {
    display: inline-block;
  }
</style>
