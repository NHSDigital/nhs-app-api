<template>
  <div>
    <collapsible-dialog :class="$style['opening-times']">
      <template slot="header">
        {{ $t('nominatedPharmacy.openingTimes') }}
      </template>
      <div>
        <div v-for="(openingTimeDetail, i)
               in pharmacyOpeningTime"
             :key="i">
          <div data-day-row :class="$style['row']">
            <div :class="$style['column']">{{ openingTimeDetail.day }}</div>
            <div :class="$style['column']">
              <div
                v-if="openingTimeDetail.times.length === 0">{{ $t('nominatedPharmacy.closed') }}
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
import CollapsibleDialog from '@/components/widgets/collapsible/CollapsibleDialog';

export default {
  name: 'PharmacyOpeningTimes',
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
  @import "@/style/custom/pharmacy-opening-times";
</style>

