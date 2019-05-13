<template>
  <div>
    <collapsible-dialog :class="$style['opening-times']">
      <template slot="header">
        {{ $t('nominated_pharmacy.openingTimes') }}
      </template>
      <div>
        <div v-for="(openingTimeDetail, i)
               in pharmacyOpeningTime"
             :key="i">
          <div data-day-row :class="$style['row']">
            <div :class="$style['column']">{{ openingTimeDetail.day }}</div>
            <div :class="$style['column']">
              <div
                v-if="openingTimeDetail.times.length === 0">{{ $t('nominated_pharmacy.closed') }}
              </div>
              <div v-else>
                <div v-for="(openingTime, j) in openingTimeDetail.times"
                     :key="j">{{ openingTime }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </collapsible-dialog>
  </div>
</template>

<script>
import CollapsibleDialog from '@/components/widgets/CollapsibleDialog';

export default {
  components: {
    CollapsibleDialog,
  },
  props: {
    pharmacyOpeningTime: {
      type: Array,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import "../../style/panels";
@import "../../style/colours";
@import "../../style/textstyles";
@import "../../style/home";

.opening-times {
  margin-bottom: 1em;
  margin-top: 1em;
}

.column {
  float: left;
  width: 50%;
}

.row:after {
  content: "";
  display: table;
  clear: both;
  padding-bottom: 0.5em;
}

.additional-padding {
  margin-top: 1em;
}
</style>

