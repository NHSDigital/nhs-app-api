<template>
  <li
    v-tabbing="defaultClasses"
    :class="getStyleClasses"
    :aria-label="isSelected?'selected-slot':undefined" tabindex="-1"
    @keypress="onKeyDown">
    <a :class="[isDesktopWeb ? $style.desktopWeb : undefined]"
       :href="createLink()" @click.prevent="select">
      {{ formatTime(timeSlot.startTime) }}
    </a>
  </li>
</template>

<script>
/* eslint-disable import/extensions */
import DateProvider from '@/services/DateProvider';
import TabFocusMixin from '@/components/widgets/TabFocusMixin';
import { APPOINTMENT_CONFIRMATIONS } from '@/lib/routes';
import { createUri } from '@/lib/noJs';

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
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
  },
  computed: {
    defaultClasses() {
      return [this.isSelected ? this.$style.selected : undefined, this.$style.timeSlot];
    },
  },
  methods: {
    createLink() {
      const noJs = {
        availableAppointments: {
          bookingReasonNecessity: this.$store.state.availableAppointments.bookingReasonNecessity,
          selectedSlot: this.timeSlot,
        },
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };

      return createUri({ path: APPOINTMENT_CONFIRMATIONS.path, noJs });
    },
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
@import '../../../style/textstyles';

.timeSlot {
  @extend .selector;
  padding: 0;

  a {
    @include default_text;
    text-decoration: none;
    font-weight: normal;
    padding: 1em;
    &.desktopWeb {
     font-family: $default-web;
     font-weight: lighter;
    }
  }

 a:focus {
  outline-color: $focus_highlight;
 }
}


</style>
