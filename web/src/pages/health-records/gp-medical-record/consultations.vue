<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="consultations.hasErrored"
      :has-access="consultations.hasAccess"
      :has-undetermined-access="consultations.hasUndeterminedAccess"/>
    <div v-else data-purpose="consultations" class="nhsuk-u-margin-bottom-4">
      <div role="list" class="nhsuk-grid-row">
        <MedicalRecordCardGroupItem
          v-for="(consultation, index) in orderedConsultations"
          :key="`consultation-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
          <Card data-label="consultations">
            <div data-purpose="consultations-card">
              <p
                v-if="consultation.effectiveDate && consultation.effectiveDate.value"
                data-purpose="record-item-header"
                class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                {{ consultation.effectiveDate.value |
                  datePart(consultation.effectiveDate.datePart) }}
              </p>
              <p v-else
                 data-purpose="record-item-header"
                 class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                {{ $t('myRecord.unknownDate') }}</p>
              <p class="nhsuk-u-margin-bottom-0" data-purpose="record-item-detail">
                {{ consultation.consultantLocation }}</p>

              <div v-for="(consultationHeader, consultationHeaderIndex)
                     in consultation.consultationHeaders"
                   :key="`line-${consultationHeaderIndex}`">
                <p class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                  {{ consultationHeader.header }} </p>

                <ul v-if="consultationHeader.observationsWithTerm.length"
                    class="nhsuk-u-margin-bottom-0 break">
                  <li v-for="(obsWithTerm, obsWithTermIndex)
                        in consultationHeader.observationsWithTerm"
                      :key="`line-${obsWithTermIndex}`">
                    {{ obsWithTerm.term }}
                    <ul v-if="obsWithTerm.associatedTexts.length"
                        class="nhsuk-u-margin-bottom-0 break">
                      <li v-for="(obsWithTermText, obsWithTermTextIndex)
                            in obsWithTerm.associatedTexts"
                          :key="`line-${obsWithTermTextIndex}`" v-html="obsWithTermText"/>
                    </ul>
                  </li>
                </ul>
                <ul v-if="consultationHeader.associatedTexts.length"
                    class="nhsuk-u-margin-bottom-0 break">
                  <li v-for="(associatedText, associatedTextIndex)
                        in consultationHeader.associatedTexts"
                      :key="`line-${associatedTextIndex}`">
                    {{ associatedText }}
                  </li>
                </ul>
              </div>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
      <no-further-information-available />
    </div>
    <glossary v-if="!showError"/>
    <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                               :path="backPath"
                               :button-text="'generic.back'"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import NoFurtherInformationAvailable from '@/components/gp-medical-record/SharedComponents/NoFurtherInformationAvailable';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    Glossary,
    MedicalRecordCardGroupItem,
    NoFurtherInformationAvailable,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      resultsCollapsed: true,
      consultations: null,
    };
  },
  computed: {
    orderedConsultations() {
      return orderBy([consultation => this.getEffectiveDate(consultation.effectiveDate, '')],
        ['desc'])((this.consultations || {}).data);
    },
    showError() {
      return this.consultations &&
        this.consultations.data &&
        (this.consultations.hasErrored
             || this.consultations.data.length === 0
             || !this.consultations.hasAccess);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'EMIS') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    this.logOperationAudit();

    if (!this.$store.state.myRecord.record.consultations) {
      await this.$store.dispatch('myRecord/load');
    }
    this.consultations = this.$store.state.myRecord.record.consultations;
  },
  methods: {
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
    logOperationAudit() {
      this.$store.dispatch('log/postOperationAudit', {
        operation: 'PatientRecord_Section_View_Response',
        details: 'Patient record CONSULTATIONS AND EVENTS successfully retrieved.',
      });
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/break";
  @import "@/style/custom/inline-block-pointer";
</style>
