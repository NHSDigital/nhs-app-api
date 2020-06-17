<template>
  <li
    v-tabbing="defaultClasses"
    :class="getStyleClasses"
    @keypress.enter="select">
    <a :href="appointmentConfirmationPath" @click.prevent="select">
      <span >
        <span :class="$style.strong"
              data-label="start time"
              :aria-label="formatTime(timeSlot.startTime)">
          {{ formatTime(timeSlot.startTime) }}
        </span>
        <p v-if="timeSlot.sessionName"
           class="nhsuk-u-margin-bottom-0"
           data-label="session name">
          {{ timeSlot.sessionName }}
        </p>
      </span>
    </a>
  </li>
</template>

<script>
import DateProvider from '@/services/DateProvider';
import TabFocusMixin from '@/components/widgets/TabFocusMixin';
import { APPOINTMENT_CONFIRMATIONS_PATH } from '@/router/paths';

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
      appointmentConfirmationPath: APPOINTMENT_CONFIRMATIONS_PATH,
    };
  },
  computed: {
    defaultClasses() {
      return [this.isSelected ? this.$style.selected : undefined, this.$style.timeSlot];
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
      background-color: white;
      border-radius: 0.313em;
    }

    &:hover {
      background-color: white;
      border-radius: 0.313em;
        @include outlineStyleLight;
    }
  }
 .strong {
   font-weight: 700;
 }
}
</style>
