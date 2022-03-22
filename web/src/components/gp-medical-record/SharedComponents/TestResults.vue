<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="results.hasErrored"
      :has-access="results.hasAccess"
      :has-undetermined-access="results.hasUndeterminedAccess"
    />
    <div v-else-if="supplier === 'TPP' || supplier === 'EMIS'"
         class="nhsuk-u-margin-bottom-4">
      <div role="list" class="nhsuk-grid-row">
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
                {{ $t('myRecord.unknownDate') }}
              </p>
              <p v-if="supplier === 'TPP'"
                 class="nhsuk-u-margin-bottom-0">
                <a :id="`view-test-results-${testIndex}`"
                   :href="getTestResultPath(testResult.id)"
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
                  {{ $t('myRecord.gpMedicalRecord.comment') }}
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
      <no-further-information-available />
    </div>
    <glossary v-if="showGlossary && !showError"/>
  </div>
</template>

<script>
import orderBy from 'lodash/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import NoFurtherInformationAvailable from '@/components/gp-medical-record/SharedComponents/NoFurtherInformationAvailable';
import { redirectTo } from '@/lib/utils';
import { TESTRESULTID_PATH } from '@/router/paths';

export default {
  name: 'TestResults',
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    MedicalRecordCardGroupItem,
    NoFurtherInformationAvailable,
    Glossary,
  },
  props: {
    results: {
      type: Object,
      default: null,
    },
    showGlossary: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      supplier: null,
      testResultsIdPath: TESTRESULTID_PATH,
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
    this.supplier = this.$store.state.myRecord.record.supplier;
  },
  methods: {
    activateTestResult(testResultId) {
      redirectTo(this, this.getTestResultPath(testResultId));
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value
        ? effectiveDate.value
        : defaultValue;
    },
    getTestResultPath(testResultId) {
      return this.testResultsIdPath.replace(':testResultId', testResultId);
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
