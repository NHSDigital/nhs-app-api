<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="consultations.hasErrored"
      :has-access="consultations.hasAccess"
      :has-undetermined-access="consultations.hasUndeterminedAccess"
    />
    <div v-else data-purpose="consultations">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(consultation, index) in orderedConsultations"
          :key="`consultation-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
        >
          <Card data-label="consultations">
            <div data-purpose="consultations-card">
              <span
                v-if="consultation.effectiveDate && consultation.effectiveDate.value"
                class="nhsuk-u-font-weight-bold"
              >
                {{ consultation.effectiveDate.value |
                  datePart(consultation.effectiveDate.datePart) }}
              </span>
              <span v-else>{{ $t('my_record.noStartDate') }}</span>

              <p>{{ consultation.consultantLocation }}</p>

              <div v-for="(consultationHeader, consultationHeaderIndex)
                     in consultation.consultationHeaders"
                   :key="`line-${consultationHeaderIndex}`" :class="$style.consultationHeader">
                <strong> {{ consultationHeader.header }} </strong>

                <ul :class="[$style.consultationTerm]">
                  <li v-for="(obsWithTerm, obsWithTermIndex)
                        in consultationHeader.observationsWithTerm"
                      :key="`line-${obsWithTermIndex}`">
                    {{ obsWithTerm.term }}
                    <ul :class="[$style.observationText]">
                      <li v-for="(obsWithTermText, obsWithTermTextIndex)
                            in obsWithTerm.associatedTexts"
                          :key="`line-${obsWithTermTextIndex}`" v-html="obsWithTermText"/>
                    </ul>
                  </li>
                </ul>

                <ul :class="[$style.consultationTerm]">
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
    orderedConsultations() {
      return _.orderBy(this.consultations.data, [consultation => this.getEffectiveDate(consultation.effectiveDate, '')], ['desc']);
    },
    getBackPath() {
      return MYRECORD.path;
    },
    showError() {
      return this.consultations.hasErrored
             || this.consultations.data.length === 0
             || !this.consultations.hasAccess;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.consultations) {
      await store.dispatch('myRecord/load');
    }
    return {
      consultations: store.state.myRecord.record.consultations,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath, null);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
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

.consultationTerm {
  margin-top: 5px;
  margin-bottom: 5px;
  font-size: 18px;
}

.observationText {
  font-size: 16px;
}
</style>
