<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="problems.hasErrored"
      :has-access="problems.hasAccess"
      :has-undetermined-access="problems.hasUndeterminedAccess"
    />
    <div v-else data-purpose="health-conditions">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(problem, index) in orderedProblems"
          :key="`problem-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
        >
          <Card data-label="health-conditions">
            <div data-purpose="health-conditions-card">
              <p
                v-if="problem.effectiveDate && problem.effectiveDate.value"
                class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
              >{{ problem.effectiveDate.value | datePart(problem.effectiveDate.datePart) }}</p>
              <p v-else class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
                {{ $t('my_record.noStartDate') }}</p>

              <div v-for="(lineItem, lineItemIndex)
                     in problem.lineItems"
                   :key="`line-${lineItemIndex}`"
                   :class="$style['nhsuk-body-m']">
                <p class="nhsuk-u-margin-bottom-0">{{ lineItem.text }}</p>
                <div v-if="lineItem.lineItems">
                  <p v-for="(childLineItem, childLineItemIndex)
                       in lineItem.lineItems"
                     :key="`line-${childLineItemIndex}`"
                     class="nhsuk-u-margin-bottom-0">
                    {{ childLineItem }}
                  </p>
                </div>
              </div>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="getBackPath"
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';
import { MYRECORD } from '@/lib/routes';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';

export default {
  layout: 'nhsuk-layout',
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    DcrErrorNoAccessGpRecord,
  },
  data() {
    return {
      resultsCollapsed: true,
    };
  },
  computed: {
    orderedProblems() {
      return orderBy([problem => this.getEffectiveDate(problem.effectiveDate, '')], ['desc'])(this.problems.data);
    },
    getBackPath() {
      return MYRECORD.path;
    },
    showError() {
      return this.problems.hasErrored
             || this.problems.data.length === 0
             || !this.problems.hasAccess;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.problems) {
      await store.dispatch('myRecord/load');
    }
    return {
      problems: store.state.myRecord.record.problems,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
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
li {
  list-style-type: none;
}
</style>
