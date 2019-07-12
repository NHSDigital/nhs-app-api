<template>
  <li
    v-tabbing="defaultClasses"
    :class="getStyleClasses"
    @keypress="onKeyDown">
    <a :href="createLink()" @click.prevent="select">
      <span >
        <span :class="$style.strong"
              data-label="start time"
              :aria-label="formatTime(timeSlot.startTime)">
          {{ formatTime(timeSlot.startTime) }}
        </span>
        <p v-if="timeSlot.sessionName" data-label="session name">
          {{ timeSlot.sessionName }}
        </p>
      </span>
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
  name: 'TimeSlot',
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
        this.select();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
@import '../../../style/textstyles';
@import '../../../style/desktopWeb/accessibility';

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
    &:focus {
      border-radius: 0.313em;
    }

    &:hover {
      border-radius: 0.313em;
        @include outlineStyleLight;
    }
  }
 .strong {
   font-weight: 700;
 }
}
</style>
