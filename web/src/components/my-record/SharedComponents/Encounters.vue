<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="encounters.hasErrored"
                       :has-access="encounters.hasAccess"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="encounters.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(encounter, encounterIndex) in orderedEncounters"
         :key="`encounter-${ encounterIndex }`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="encounter.recordedOn && encounter.recordedOn.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ encounter.recordedOn.value | datePart(encounter.recordedOn.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ encounter.description }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.encounters.value') }}{{ encounter.value }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.encounters.unit') }}{{ encounter.unit }} </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Encounters',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    encounters: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedEncounters() {
      return orderBy([encounter => this.getRecordedOnDate(encounter.recordedOn, '')],
        ['desc'])(this.encounters.data);
    },
    showError() {
      return this.encounters.hasErrored ||
          this.encounters.data.length === 0 ||
          !this.encounters.hasAccess;
    },
  },
  methods: {
    getRecordedOnDate(recordedOn, defaultValue) {
      return recordedOn && recordedOn.value ? recordedOn.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
