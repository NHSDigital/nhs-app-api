<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="recalls.hasErrored"
                       :has-access="recalls.hasAccess"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="recalls.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(recall, recallIndex) in orderedRecalls"
         :key="`recall-${ recallIndex }`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="recall.recordDate && recall.recordDate.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ recall.recordDate.value | datePart(recall.recordDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ recall.name }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ recall.description }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.recalls.result') }}{{ recall.result }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.recalls.nextDate') }}{{ recall.nextDate }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.recalls.status') }}{{ recall.status }} </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Recalls',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    recalls: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedRecalls() {
      return orderBy([recall => this.getRecordDate(recall.recordDate, '')],
        ['desc'])(this.recalls.data);
    },
    showError() {
      return this.recalls.hasErrored ||
             this.recalls.data.length === 0 ||
             !this.recalls.hasAccess;
    },
  },
  methods: {
    getRecordDate(recordDate, defaultValue) {
      return recordDate && recordDate.value ? recordDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
</style>
