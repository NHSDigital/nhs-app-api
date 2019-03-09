<template>
  <dcr-error-no-access v-if="showError"
                       :data="events"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-access="events.hasAccess"
                       :has-errored="events.hasErrored" />
  <div v-else :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-for="(event, eventIndex) in orderedEvents" :key="`event-${eventIndex}`"
         :class="$style['record-item']" data-purpose="record-item">
      <span v-if="event.date" :class="$style.fieldName"> {{ event.date | datePart }} </span>
      <p> {{ event.locationAndDoneBy }}</p>
      <ul :class="$style.eventLine">
        <li v-for="(eventItem, eventItemIndex) in event.eventItems"
            :key="`event-${eventItemIndex}`">
          {{ eventItem }}
        </li>
      </ul>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import _ from 'lodash';
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
    events: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedEvents() {
      return _.orderBy(this.events.data, [obj => obj.date], ['desc']);
    },
    showError() {
      return this.events.hasErrored ||
             this.events.data.length === 0 ||
             !this.events.hasAccess;
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

</style>
