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
      <span v-if="encounter.recordedOn && encounter.recordedOn.value"
            :class="$style.fieldName">
        {{ encounter.recordedOn.value | datePart(encounter.recordedOn.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>
      <p> {{ encounter.description }} </p>
      <p> {{ $t('my_record.encounters.value') }}{{ encounter.value }} </p>
      <p> {{ $t('my_record.encounters.unit') }}{{ encounter.unit }} </p>
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
