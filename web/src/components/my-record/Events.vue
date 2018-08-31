<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else-if="!data.hasAccess">
      {{ $t('my_record.genericNoAccessMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
  <div v-else :class="[$style['record-content'], getCollapseState]">
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
      <hr>
    </div>
  </div>
</template>

<script>

import _ from 'lodash';

export default {
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    data: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedEvents() {
      return _.orderBy(this.data.data, [obj => obj.date], ['desc']);
    },
    showError() {
      return this.data.hasErrored ||
             this.data.data.length === 0 ||
             !this.data.hasAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/medrecordcontent';

.fieldName {
  padding-left: 1.3em;
  padding-right: 1.3em;
  padding-bottom: 0.250rem;
  color: #425563;
  font-size: 0.813em;
  font-weight: 700;
}

</style>
