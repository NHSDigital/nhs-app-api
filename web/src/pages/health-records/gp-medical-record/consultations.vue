<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="consultations.hasErrored"
      :has-access="consultations.hasAccess"
      :has-undetermined-access="consultations.hasUndeterminedAccess"/>
    <div v-else data-purpose="consultations">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
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
                {{ $t('my_record.noStartDate') }}</p>
              <p class="nhsuk-u-margin-bottom-0" data-purpose="record-item-detail">
                {{ consultation.consultantLocation }}</p>

              <div v-for="(consultationHeader, consultationHeaderIndex)
                     in consultation.consultationHeaders"
                   :key="`line-${consultationHeaderIndex}`">
                <p class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                  {{ consultationHeader.header }} </p>

                <ul v-if="consultationHeader.observationsWithTerm.length"
                    class="nhsuk-u-margin-bottom-0">
                  <li v-for="(obsWithTerm, obsWithTermIndex)
                        in consultationHeader.observationsWithTerm"
                      :key="`line-${obsWithTermIndex}`">
                    {{ obsWithTerm.term }}
                    <ul v-if="obsWithTerm.associatedTexts.length"
                        class="nhsuk-u-margin-bottom-0">
                      <li v-for="(obsWithTermText, obsWithTermTextIndex)
                            in obsWithTerm.associatedTexts"
                          :key="`line-${obsWithTermTextIndex}`" v-html="obsWithTermText"/>
                    </ul>
                  </li>
                </ul>
                <ul v-if="consultationHeader.associatedTexts.length"
                    class="nhsuk-u-margin-bottom-0">
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
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    DcrErrorNoAccessGpRecord,
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
    // (this.consultations.data);
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

    if (!this.$store.state.myRecord.record.consultations) {
      await this.$store.dispatch('myRecord/load');
    }
    this.consultations = this.$store.state.myRecord.record.consultations;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};
</script>

<style module scoped lang="scss">
a {
  display: inline-block;
  cursor: pointer;
}
</style>
