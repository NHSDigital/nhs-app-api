<template>
  <scr-error-no-access v-if="showError"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-errored="allergies.hasErrored"
                       :has-undetermined-access="allergies.hasUndeterminedAccess"/>

  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(allergy, index) in orderedAllergies" :key="`allergy.name-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <span v-if="allergy.date && allergy.date.value" :class="$style.fieldName">
        {{ allergy.date.value | datePart(allergy.date.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>

      <p>{{ allergy.name }}</p>
      <p v-if="allergy.drug">{{ allergy.drug }}</p>
      <p v-if="allergy.reaction">{{ allergy.reaction }}</p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import _ from 'lodash';
import ScrErrorNoAccess from '@/components/my-record/SharedComponents/SCRErrorNoAccess';

export default {
  name: 'AllergiesAndAdverseReactions',
  components: {
    ScrErrorNoAccess,
  },
  props: {
    allergies: {
      type: Object,
      default: () => {},
    },
    isCollapsed: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedAllergies() {
      return _.orderBy(this.allergies.data, [obj => obj.date.value], ['desc']);
    },
    showError() {
      return this.allergies.hasErrored || this.allergies.data.length === 0;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
  @import '../../../style/medrecordtitle';

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: #425563;
    font-size: 0.813em;
    font-weight: 700;
  }

  div {
   &.desktopWeb {
    max-width: 540px;

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
