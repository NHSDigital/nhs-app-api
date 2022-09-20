<template>
  <time :class="$style['formatted-time']"
        :datetime="dateTime | formatDate('YYYY-MM-DD h:mma')"
  >{{ formattedTime }}</time>
</template>

<script>
import { formatIndividualMessageTime, formatInboxMessageTime, formatKeywordReplyMessageTime } from '@/lib/utils';

export default {
  name: 'FormattedDateTime',
  props: {
    dateTime: {
      type: String,
      required: true,
    },
    summaryTimeFormat: {
      type: Boolean,
      default: false,
    },
    timeFormat: {
      type: Number,
      default: 0,
    },
  },
  computed: {
    formattedTime() {
      const timeFormatValue = this.getTimeFormatValue;
      return this.mapTimeFormat(timeFormatValue);
    },
    getTimeFormatValue() {
      if (this.timeFormat === 0) {
        return this.summaryTimeFormat ? 1 : 0;
      }

      return this.timeFormat;
    },
  },
  methods: {
    mapTimeFormat(timeFormat) {
      return {
        0: formatIndividualMessageTime(this.dateTime, this.$t.bind(this)),
        1: formatInboxMessageTime(this.dateTime, this.$t.bind(this)),
        2: formatKeywordReplyMessageTime(this.dateTime, this.$t.bind(this)),
      }[timeFormat];
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/formatted-date-time";
</style>
