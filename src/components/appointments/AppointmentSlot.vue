<template>
  <div :class="getClass" :aria-selected="isSelected" @click="select">
    <h5 :class="$style.date" aria-label="date">
      {{ formatDate(theSlot.startTime) }}
    </h5>
    <h4 :class="$style.startTime" aria-label="start time">
      {{ formatTime(theSlot.startTime) }}
    </h4>
    <hr aria-hidden="true">
    <p :class="$style.session" aria-label="session name">
      {{ appointmentSession | truncate(24) }}
    </p>
    <hr aria-hidden="true" >
    <p aria-label="location">
    <location-icon/>&nbsp;{{ location | truncate(24) }}</p>
    <ul
      v-for="clinician in theSlot.clinicians"
      :key="clinician.id"
      :class="$style.clinicians"
      aria-label="clinicians"
    >
      <li>
        <p>
        <clinician-icon/>&nbsp;{{ clinician.displayName | truncate(24) }}</p>
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { get } from 'lodash/fp';
import moment from 'moment';
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';

export default {
  components: {
    ClinicianIcon,
    LocationIcon,
  },
  props: {
    alwaysDeselect: {
      default: false,
      type: Boolean,
    },
    theSlot: {
      type: Object,
      required: true,
    },
  },
  computed: {
    appointmentSession() {
      return get('appointmentSession.displayName')(this.theSlot);
    },
    getClass() {
      const isSelected = this.isSlotSelected() && !this.alwaysDeselect;
      return isSelected ? this.$style.selectedContainer : this.$style.container;
    },
    location() {
      return get('location.displayName')(this.theSlot);
    },
    isSelected() {
      return this.theSlot ? false : this.isSlotSelected();
    },
  },
  methods: {
    formatTime: dateTime => moment(dateTime).format('h:mm a'),
    formatDate: dateTime => moment(dateTime).format('dddd D MMMM YYYY'),
    select() {
      if (!this.alwaysDeselect) {
        this.$store.dispatch('appointmentSlots/select', this.theSlot);
      }
    },
    isSlotSelected() {
      return this.$store.state.appointmentSlots.selectedSlot !== null
        && this.$store.state.appointmentSlots.selectedSlot.id === this.theSlot.id;
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/colours";
  @import "../../style/spacings";

  .date {
    display: block;
    font-size: 16px;
    font-weight: 700;
    line-height: 22px;
  }

  .startTime {
    @include space(margin, top, $one);
    display: block;
    font-weight: 700;
    line-height: 22px;
    font-size: 20px;
  }

  .session {
    display: block;
    font-weight: normal;
    font-size: 16px;
    line-height: 22px;
  }

  p {
    display: block;
    font-weight: normal;
    font-size: 16px;
    line-height: 22px;
  }

  li {
    list-style: none;
  }

  .selectedContainer {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    @include space(padding, all, $three);
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: $dark_grey;
      opacity: 0.2;
      @include space(margin, top, $two);
      @include space(margin, bottom, $two);
    }
    background: $nhs_blue;
    color: $white;
  }

  .container {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    background: $white;
    @include space(padding, all, $three);
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: $dark_grey;
      opacity: 0.2;
      @include space(margin, top, $two);
      @include space(margin, bottom, $two);
    }
    .startTime {
      color: #999;
    }
  }

</style>
