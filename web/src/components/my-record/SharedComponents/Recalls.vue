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
      <span v-if="recall.recordDate && recall.recordDate.value"
            :class="$style.fieldName">
        {{ recall.recordDate.value | datePart(recall.recordDate.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>
      <p> {{ recall.name }} </p>
      <p> {{ recall.description }} </p>
      <p> {{ $t('my_record.recalls.result') }}{{ recall.result }} </p>
      <p> {{ $t('my_record.recalls.nextDate') }}{{ recall.nextDate }} </p>
      <p> {{ $t('my_record.recalls.status') }}{{ recall.status }} </p>
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
