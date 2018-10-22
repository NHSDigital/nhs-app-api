<template>
  <li
    v-tabbing="defaultClasses"
    :class="getStyleClasses"
    :aria-label="isSelected?'selected-slot':undefined" tabindex="0"
    @keypress="onKeyDown">
    <a :href="createLink(timeSlot)" @click="select($event)">
      {{ formatTime(timeSlot.startTime) }}
    </a>
  </li>
</template>

<script>
/* eslint-disable import/extensions */
import assign from 'lodash/fp/assign';
import get from 'lodash/fp/get';
import DateProvider from '@/services/DateProvider';
import TabFocusMixin from '@/components/widgets/TabFocusMixin';
import { APPOINTMENT_CONFIRMATIONS } from '@/lib/routes';

export default {
  components: {
    TabFocusMixin,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    timeSlot: {
      type: Object,
      default: () => {},
      required: true,
    },
  },
  data() {
    return {
      isSelected: false,
    };
  },
  computed: {
    defaultClasses() {
      return [this.isSelected ? this.$style.selected : undefined, this.$style.timeSlot];
    },
  },
  methods: {
    createLink(timeSlot) {
      const bookingReasonNecessity = get('$store.state.availableAppointments.bookingReasonNecessity')(this);
      const slot = assign({}, timeSlot);
      slot.startTime = DateProvider.create(timeSlot.startTime).toDate().getTime();
      slot.endTime = DateProvider.create(timeSlot.endTime).toDate().getTime();
      return `${APPOINTMENT_CONFIRMATIONS.path}?slot=${JSON.stringify(slot)}&bookingReasonNecessity=${bookingReasonNecessity}`;
    },
    formatTime: dateTime => DateProvider.create(dateTime).format('h:mma'),
    select(e) {
      this.isSelected = true;
      this.$store.dispatch('availableAppointments/select', this.timeSlot);
      if (e) e.preventDefault();
    },
    deselect() {
      this.isSelected = false;
      this.$store.dispatch('availableAppointments/deselect');
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.$emit('select', this.timeSlot.ref);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
@import '../../../style/textstyles';

.timeSlot {
  @extend .selector;
  padding: 0;

  a {
    @include default_text;
    text-decoration: none;
    font-weight: normal;
    padding: 1em;

  }
}


</style>
