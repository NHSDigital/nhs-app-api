<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p> {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul >
            <li v-for="(event, eventIndex) in orderedEvents"
                :key="`event-${eventIndex}`" :class="$style.event">
              <label v-if="event.date"> {{ event.date | datePart }} </label>
              <p :class="$style.eventDetail"> {{ event.locationAndDoneBy }}</p>
              <ul :class="$style.eventLine">
                <li v-for="(eventItem, eventItemIndex) in event.eventItems"
                    :key="`event-${eventItemIndex}`" :class="$style.eventLine">
                  {{ eventItem }}
                </li>
              </ul>
            </li>
          </ul>
        </div>
        <div v-else>
          <p> {{ $t('myRecord.genericNoDataMessage') }} </p>
        </div>
      </div>
      <div v-else>
        <p> {{ $t('myRecord.genericNoAccessMessage') }} </p>
      </div>
    </div>
    <hr>
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
  },
};

</script>

<style lang="scss" module>
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .event {
    border-bottom: 1px solid #e8edee;
    padding-bottom: 16px;
  }
  .eventDetail {
    padding-bottom: 0px !important;
  }
  .eventLine {
    @include small_text;
    padding-left: 16px;
  }
</style>
