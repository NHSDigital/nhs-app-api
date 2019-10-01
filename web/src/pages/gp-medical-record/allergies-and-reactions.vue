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
        >
          <Card data-label="allergies-and-reactions">
            <div data-purpose="allergies-and-reactions-card">
              <span
                v-if="allergy.date && allergy.date.value"
                class="nhsuk-u-font-weight-bold"
              >{{ allergy.date.value | datePart(allergy.date.datePart) }}</span>
              <span v-else>{{ $t('my_record.noStartDate') }}</span>

              <p>{{ allergy.name }}</p>
              <p v-if="allergy.drug">{{ allergy.drug }}</p>
              <p v-if="allergy.reaction">{{ allergy.reaction }}</p>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="getBackPath"
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backButtonClicked"/>
    <glossary v-if="!showError"/>
  </div>
</template>

<script>
import _ from 'lodash';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';
import { MYRECORD } from '@/lib/routes';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';
import ScrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/SCRErrorNoAccessGpRecord';

export default {
  layout: 'nhsuk-layout',
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    ScrErrorNoAccessGpRecord,
  },
  data() {
    return {
      resultsCollapsed: true,
    };
  },
  computed: {
    orderedAllergies() {
      return _.orderBy((this.allergies || {}).data, [obj => obj.date.value], ['desc']);
    },
    getBackPath() {
      return MYRECORD.path;
    },
    showError() {
      return ((this.allergies || {}).hasErrored) || (this.allergies || {}).data.length === 0;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.allergies) {
      await store.dispatch('myRecord/load');
    }
    return {
      allergies: store.state.myRecord.record.allergies,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath, null);
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
