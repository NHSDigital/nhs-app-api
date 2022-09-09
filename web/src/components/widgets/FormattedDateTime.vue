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
      let formattedTimeValue = '';
      const timeFormatValue = this.getTimeFormatValue;

      if (timeFormatValue === 0) {
        formattedTimeValue = formatIndividualMessageTime(this.dateTime, this.$t.bind(this));
      }

      if (timeFormatValue === 1) {
        formattedTimeValue = formatInboxMessageTime(this.dateTime, this.$t.bind(this));
      }

      if (timeFormatValue === 2) {
        formattedTimeValue = formatKeywordReplyMessageTime(this.dateTime, this.$t.bind(this));
      }

      return formattedTimeValue;
    },
    getTimeFormatValue() {
      if (this.timeFormat === 0) {
        return this.summaryTimeFormat ? 1 : 0;
      }

      return this.timeFormat;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/formatted-date-time";
</style>
