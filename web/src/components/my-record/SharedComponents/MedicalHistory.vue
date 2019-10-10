<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="medicalHistory.hasErrored"
                       :has-access="medicalHistory.hasAccess"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="medicalHistory.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(history, historyIndex) in orderedMedicalHistory"
         :key="`history-${ historyIndex }`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="history.startDate && history.startDate.value" data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ history.startDate.value | datePart(history.startDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ history.rubric }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ history.description }} </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'MedicalHistory',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    medicalHistory: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedMedicalHistory() {
      return orderBy([medicalHistory => this.getStartDate(medicalHistory.startDate, '')],
        ['desc'])(this.medicalHistory.data);
    },
    showError() {
      return this.medicalHistory.hasErrored ||
             this.medicalHistory.data.length === 0 ||
             !this.medicalHistory.hasAccess;
    },
  },
  methods: {
    getStartDate(startDate, defaultValue) {
      return startDate && startDate.value ? startDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
</style>
