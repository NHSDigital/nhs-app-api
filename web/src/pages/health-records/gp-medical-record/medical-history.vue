<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="medicalHistory.hasErrored"
      :has-access="medicalHistory.hasAccess"
      :has-undetermined-access="medicalHistory.hasUndeterminedAccess"
    />
    <div v-else data-purpose="medical-history">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(history, index) in orderedMedicalHistory"
          :key="`history-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
        >
          <Card data-label="medical-history">
            <div data-purpose="medical-history-card">
              <span
                v-if="history.startDate && history.startDate.value"
                class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-4"
              >{{ history.startDate.value | datePart(history.startDate.datePart) }}</span>
              <span v-else>{{ $t('my_record.noStartDate') }}</span>

              <p> {{ history.rubric }} </p>
              <p> {{ history.description }} </p>
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
import { GP_MEDICAL_RECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
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
      backPath: GP_MEDICAL_RECORD.path,
    };
  },
  computed: {
    orderedMedicalHistory() {
      return orderBy([medicalHistory => this.getStartDate(medicalHistory.startDate, '')],
        ['desc'])(this.medicalHistory.data);
    },
    showError() {
      return this.medicalHistory.hasErrored
             || this.medicalHistory.data.length === 0
             || !this.medicalHistory.hasAccess;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.medicalHistories) {
      await store.dispatch('myRecord/load');
    }
    return {
      medicalHistory: store.state.myRecord.record.medicalHistories,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getStartDate(startDate, defaultValue) {
      return startDate && startDate.value ? startDate.value : defaultValue;
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
