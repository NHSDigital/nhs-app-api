<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="immunisations.hasAccess"
                       :has-errored="immunisations.hasErrored"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'],
                                         getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(item, index) in orderedImmunisations" :key="`item-${index}`"
         :class="$style['record-item']" data-purpose="record-item">
      <span v-if="item.effectiveDate.value" :class="$style.fieldName">
        {{ item.effectiveDate.value | datePart(item.effectiveDate.datePart) }}
      </span>
      <p>{{ item.term }}</p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    immunisations: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedImmunisations() {
      return orderBy([obj => obj.effectiveDate.value], ['desc'])(this.immunisations.data);
    },
    showError() {
      return this.immunisations.hasErrored ||
             this.immunisations.data.length === 0 ||
             !this.immunisations.hasAccess;
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
