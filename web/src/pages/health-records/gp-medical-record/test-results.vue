<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <dcr-error-no-access-gp-record
        v-if="showError"
        :has-errored="results.hasErrored"
        :has-access="results.hasAccess"
        :has-undetermined-access="results.hasUndeterminedAccess"
      />
      <div v-else-if="supplier === 'TPP' || supplier === 'EMIS'"
           role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(testResult, testIndex) in orderedTestResults"
          :key="`testResult-${testIndex}`"
          data-purpose="record-item"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
          <Card data-label="allergies-and-reactions">
            <div data-purpose="test-results-card">
              <p v-if="testResult.date && testResult.date.value"
                 class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">
                {{ testResult.date.value | datePart(testResult.date.datePart) }}
              </p>
              <p v-else class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">
                {{ $t('my_record.noStartDate') }}
              </p>
              <p v-if="supplier === 'TPP'"
                 class="nhsuk-u-margin-bottom-0">
                <a :href="getTestResultPath(testResult.id)"
                   :class="$style.viewTestResult"
                   data-purpose="record-item-detail"
                   tabindex="0"
                   @click.prevent="activateTestResult(testResult.id)"
                   @keypress.enter.prevent="activateTestResult(testResult.id)">
                  {{ testResult.description }}
                </a>
              </p>
              <p v-if="supplier === 'EMIS'"
                 class="nhsuk-body nhsuk-u-margin-bottom-2"
                 data-purpose="record-item-detail">
                {{ testResult.description }}</p>
              <div v-if="testResult.associatedTexts.length > 0"
                   class="nhsuk-body nhsuk-u-margin-bottom-2">
                <p class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                  {{ $t('my_record.testResults.commentHeader') }}
                </p>
                <p v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
                   :key="`associatedText-${associatedTextItemIndex}`"
                   class="nhsuk-u-margin-bottom-0">
                  {{ associatedText }}</p>
              </div>
              <ul v-if="testResult.testResultChildLineItems.length > 0"
                  class="nhsuk-list nhsuk-list--bullet nhsuk-u-margin-bottom-0">
                <li v-for="(lineItem, lineItemIndex)
                      in testResult.testResultChildLineItems"
                    :key="`line-${lineItemIndex}`"
                    data-purpose="record-item-child-detail">
                  {{ lineItem.description }}
                  <ul v-if="lineItem.associatedTexts.length > 0"
                      class="nhsuk-list nhsuk-list--bullet nhsuk-u-margin-bottom-0">
                    <li v-for="(lineItemAssociatedText, lineItemAssociatedTextIndex)
                          in lineItem.associatedTexts"
                        :key="`lineAssociatedText-${lineItemAssociatedTextIndex}`">
                      {{ lineItemAssociatedText }}
                    </li>
                  </ul>
                </li>
              </ul>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
      <glossary v-if="!showError"/>
      <desktopGenericBackLink
        v-if="!$store.state.device.isNativeApp"
        :path="backPath"
        :button-text="'generic.backButton.text'"
        @clickAndPrevent="backButtonClicked"/>
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { redirectTo } from '@/lib/utils';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';

export default {
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      results: null,
      supplier: null,
    };
  },
  computed: {
    orderedTestResults() {
      return orderBy(
        (this.results || {}).data,
        [result => this.getEffectiveDate(result.date, '')],
        ['desc'],
      );
    },
    showError() {
      return this.results && (
        this.results.hasErrored ||
        this.results.data.length === 0 ||
        !this.results.hasAccess);
    },
    getTotalResults() {
      return (this.results || {}).data.length > 10;
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.testResults) {
      await this.$store.dispatch('myRecord/load');
    }
    this.results = this.$store.state.myRecord.record.testResults;
    this.supplier = this.$store.state.myRecord.record.supplier;
  },
  methods: {
    activateTestResult(testResultId) {
      redirectTo(this, this.getTestResultPath(testResultId));
    },
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value
        ? effectiveDate.value
        : defaultValue;
    },
    getTestResultPath(testResultId) {
      return `/health-records/gp-medical-record/testresultdetail/${testResultId}`;
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
