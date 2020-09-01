<template>
  <div>
    <scr-error-no-access-gp-record
      v-if="showError"
      :has-errored="allergies.hasErrored"
      :has-undetermined-access="allergies.hasUndeterminedAccess"
    />
    <div v-else data-purpose="allergies-and-reactions">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(allergy, index) in orderedAllergies"
          :key="`allergy.name-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
          data-purpose="record-item"
        >
          <Card data-label="allergies-and-reactions">
            <div data-purpose="allergies-and-reactions-card">
              <p v-if="allergy.date && allergy.date.value" data-purpose="record-item-header"
                 class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                {{ allergy.date.value | datePart(allergy.date.datePart) }}</p>
              <p v-else class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">
                {{ $t('myRecord.unknownDate') }}</p>
              <p data-purpose="record-item-detail">{{ allergy.name }}</p>
              <p v-if="allergy.drug" data-purpose="record-item-detail">{{ allergy.drug }}</p>
              <p v-if="allergy.reaction" data-purpose="record-item-detail">
                {{ allergy.reaction }}</p>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.backButton.text'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import Card from '@/components/widgets/card/Card';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import ScrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/SCRErrorNoAccessGpRecord';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    ScrErrorNoAccessGpRecord,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      resultsCollapsed: true,
      allergies: null,
    };
  },
  computed: {
    orderedAllergies() {
      return orderBy([obj => this.getEffectiveDate(obj.date, '')], ['desc'])((this.allergies || {}).data);
    },
    showError() {
      return this.allergies &&
        (this.allergies.hasErrored || this.allergies.data.length === 0);
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.allergies) {
      await this.$store.dispatch('myRecord/load');
    }
    this.allergies = this.$store.state.myRecord.record.allergies;
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
@import "../../../style/colours";
@import "../../../style/desktopWeb/accessibility";
a {
  display: inline-block;
  &:focus {
    @include outlineStyle;
    background-color: $focus_highlight;
  }
  &:hover {
    @include linkHoverStyle;
    cursor: pointer;
  }
}
</style>
