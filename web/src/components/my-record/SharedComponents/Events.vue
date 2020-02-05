<template>
  <dcr-error-no-access v-if="showError"
                       :data="events"
                       :class="[$style['record-content'], getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-access="events.hasAccess"
                       :has-errored="events.hasErrored" />
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <div v-for="(event, eventIndex) in orderedEvents" :key="`event-${eventIndex}`"
         :class="$style['record-item']" data-purpose="record-item">
      <p v-if="event.date" data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s"> {{ event.date | datePart }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ event.locationAndDoneBy }}</p>
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

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Events',
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
      return orderBy([obj => obj.date], ['desc'])(this.events.data);
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
</style>
