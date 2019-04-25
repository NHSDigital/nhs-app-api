<template>
  <scr-error-no-access v-if="showError"
                       :data="medications"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(medication, medIndex) in orderedMedications"
         :key="`medication-${medIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <span v-if="medication.date" :class="$style.fieldName">{{ medication.date | datePart }}</span>
      <p v-for="(lineItem, lineItemIndex) in medication.lineItems"
         :key="`line-${lineItemIndex}`">
        {{ lineItem.text }}
        <ul>
          <li v-for="(innerLineItem, innerLineItemIndex) in lineItem.lineItems"
              :key="`innerline-${innerLineItemIndex}`">
            {{ innerLineItem }}
          </li>
        </ul>
      </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import ScrErrorNoAccess from '@/components/my-record/SharedComponents/SCRErrorNoAccess';

export default {
  name: 'Medications',
  components: {
    ScrErrorNoAccess,
  },
  props: {
    medications: {
      type: Array,
      default: () => [],
    },
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    hasError: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedMedications() {
      return orderBy([obj => obj.date], ['desc'])(this.medications);
    },
    showError() {
      return this.medications.hasErrored
             || (this.medications && this.medications.length === 0);
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
