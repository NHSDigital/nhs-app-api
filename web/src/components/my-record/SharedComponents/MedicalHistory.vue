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
    <div v-for="(medicalHistory, medicalHistoryIndex) in orderedMedicalHistory"
         :key="`medicalHistory-${ medicalHistoryIndex }`" :class="$style['record-item']"
         data-purpose="record-item">
      <span v-if="medicalHistory.startDate && medicalHistory.startDate.value"
            :class="$style.fieldName">
        {{ medicalHistory.startDate.value | datePart(medicalHistory.startDate.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>
      <p> {{ medicalHistory.rubric }} </p>
      <p> {{ medicalHistory.description }} </p>
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
@import '../../../style/medrecordtitle';
@import "../../../style/colours";

.fieldName {
  padding-left: 1.3em;
  padding-right: 1.3em;
  padding-bottom: 0.250rem;
  color: $dark_grey;
  font-size: 0.813em;
  font-weight: 700;
}

div {
 &.desktopWeb {
  max-width: 540px;
  cursor: default;

  span {
   font-family: $default_web;
   font-weight: normal;
  }
  p {
   font-family: $default_web;
   font-weight: normal;
  }
 }
}

</style>
