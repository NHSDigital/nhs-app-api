<template>
  <li
    v-tabbing="defaultClasses"
    :class="getStyleClasses"
    :aria-label="isSelected?'selected-slot':undefined" tabindex="0"
    @keypress="onKeyDown">
    {{ formatTime(timeSlot.startTime) }}
  </li>
</template>

<script>
/* eslint-disable import/extensions */
import DateProvider from '@/services/DateProvider';
import TabFocusMixin from '@/components/widgets/TabFocusMixin';

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
      return [this.isSelected ? this.$style.selected : undefined, this.$style.selector];
    },
  },
  methods: {
    formatTime: dateTime => DateProvider.create(dateTime).format('h:mma'),
    select() {
      this.isSelected = true;
      this.$store.dispatch('availableAppointments/select', this.timeSlot);
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

</style>
