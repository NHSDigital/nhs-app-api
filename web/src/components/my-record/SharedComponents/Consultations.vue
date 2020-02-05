<!-- eslint-disable vue/no-v-html -->
<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="consultations.hasAccess"
                       :has-errored="consultations.hasErrored"
                       :is-collapsed="isCollapsed"
                       :class="[$style['record-content'], getCollapsedState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapsedState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(consultation, consultationIndex) in orderedConsultations"
         :key="`consultation-${consultationIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="consultation.effectiveDate && consultation.effectiveDate.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ consultation.effectiveDate.value | datePart(consultation.effectiveDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ $t('my_record.noStartDate') }}
      </p>

      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ consultation.consultantLocation }} </p>

      <div v-for="(consultationHeader, consultationHeaderIndex)
             in consultation.consultationHeaders"
           :key="`line-${consultationHeaderIndex}`"
           :class="[$style.consultationHeader,
                    'nhsuk-u-padding-left-3',
                    'nhsuk-body-m']">
        <strong> {{ consultationHeader.header }} </strong>

        <ul :class="$style.consultationTerm">
          <li v-for="(obsWithTerm, obsWithTermIndex)
                in consultationHeader.observationsWithTerm"
              :key="`line-${obsWithTermIndex}`">
            {{ obsWithTerm.term }}
            <ul :class="$style.observationText">
              <li v-for="(obsWithTermText, obsWithTermTextIndex)
                    in obsWithTerm.associatedTexts"
                  :key="`line-${obsWithTermTextIndex}`" v-html="obsWithTermText"/>
            </ul>
          </li>
        </ul>

        <ul :class="$style.consultationTerm">
          <li v-for="(associatedText, associatedTextIndex)
                in consultationHeader.associatedTexts"
              :key="`line-${associatedTextIndex}`">
            {{ associatedText }}
          </li>
        </ul>
      </div>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Consultations',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    consultations: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapsedState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedConsultations() {
      return orderBy([consultation => this.getEffectiveDate(consultation.effectiveDate, '')], ['desc'])(this.consultations.data);
    },
    showError() {
      return this.consultations.hasErrored
             || this.consultations.data.length === 0
             || !this.consultations.hasAccess;
    },
  },
  methods: {
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
 @import '../../../style/medrecordcontent';
</style>
