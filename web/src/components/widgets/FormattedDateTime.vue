<template>
  <time :class="$style['formatted-time']"
        :datetime="dateTime | formatDate('YYYY-MM-DD h:mma')"
  >{{ formattedTime }}</time>
</template>

<script>
import { formatIndividualMessageTime, formatInboxMessageTime } from '@/lib/utils';

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
  },
  computed: {
    formattedTime() {
      return this.summaryTimeFormat ? formatInboxMessageTime(this.dateTime, this.$t.bind(this)) :
        formatIndividualMessageTime(this.dateTime, this.$t.bind(this));
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/functions';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';

.formatted-time{
  @include nhsuk-responsive-padding(0);
  @include nhsuk-responsive-margin(0);
  display: block;
}
</style>
