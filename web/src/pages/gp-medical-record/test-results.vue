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
              <span v-if="testResult.date.value">
                <strong>
                  {{ testResult.date.value | datePart(testResult.date.datePart) }}
                </strong>
              </span>
              <span v-else>
                <strong>
                  {{ $t('my_record.noStartDate') }}
                </strong>
              </span>
              <p v-if="supplier === 'TPP'">
                <a :href="getTestResultPath(testResult.id)"
                   :class="$style.viewTestResult"
                   tabindex="0"
                   @click="activateTestResult(testResult.id, $event)"
                   @keypress="onKeyDown(testResult.id, $event)">
                  {{ testResult.description }}
                </a>
              </p>
              <p v-if="supplier === 'EMIS'" class="nhsuk-body nhsuk-u-margin-bottom-2">
                {{ testResult.description }}</p>
              <ul v-if="testResult.associatedTexts.length !== 0"
                  class="nhsuk-list nhsuk-list--bullet">
                <li v-for="(associatedText, associatedTextItemIndex) in testResult.associatedTexts"
                    :key="`associatedText-${associatedTextItemIndex}`">
                  {{ associatedText }}</li>
              </ul>
              <ul v-if="testResult.testResultChildLineItems.length !== 0"
                  class="nhsuk-list nhsuk-list--bullet">
                <li v-for="(lineItem, lineItemIndex)
                      in testResult.testResultChildLineItems"
                    :key="`line-${lineItemIndex}`">
                  {{ lineItem.description }}
                  <ul v-if="lineItem.associatedTexts.length !== 0"
                      class="nhsuk-list nhsuk-list--bullet">
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

      <desktopGenericBackLink
        v-if="!$store.state.device.isNativeApp"
        :path="backPath"
        :button-text="'rp03.backButton'"
        @clickAndPrevent="backButtonClicked"
      />
      <glossary v-if="!showError"/>
    </div>
  </div>
</template>

<script>
import _ from 'lodash';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';
import { MYRECORD } from '@/lib/routes';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
  },
  data() {
    return {
      backPath: MYRECORD.path,
    };
  },
  computed: {
    orderedTestResults() {
      return _.orderBy(
        (this.results || {}).data,
        [result => this.getEffectiveDate(result.date, '')],
        ['desc'],
      );
    },
    showError() {
      return (
        (this.results || {}).hasErrored ||
        (this.results || {}).data.length === 0 ||
        !this.results.hasAccess
      );
    },
    getTotalResults() {
      return (this.results || {}).data.length > 10;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.testResults) {
      await store.dispatch('myRecord/load');
    }
    return {
      results: store.state.myRecord.record.testResults,
      supplier: store.state.myRecord.record.supplier,
    };
  },
  methods: {
    activateTestResult(testResultId, event) {
      event.preventDefault();
      redirectTo(this, this.getTestResultPath(testResultId));
    },
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value
        ? effectiveDate.value
        : defaultValue;
    },
    onKeyDown(testResultId, e) {
      if (e.keyCode === 13) {
        this.activateTestResult(testResultId, e);
      }
    },
    getTestResultPath(testResultId) {
      return `/gp-medical-record/testresultdetail/${testResultId}`;
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../style/colours";
@import "../../style/desktopWeb/accessibility";
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
