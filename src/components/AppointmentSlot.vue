<template>
  <div :aria-selected="isSelected(slotId)">
    <div :class="appointmentSelected" @click="select">
      <h5 :class="$style.date">{{ formatDate(appointmentSlot.startTime) }}</h5>
      <h4 :class="$style.startTime">{{ formatTime(appointmentSlot.startTime) }}</h4>
      <hr aria-hidden="true">
      <p :class="$style.session">{{ appointmentSession | truncate(24) }}</p>
      <hr aria-hidden="true">
      <div>
        <p><location-icon/>&nbsp;{{ location | truncate(24) }}</p>
      </div>
      <ul v-for="clinician in appointmentSlot.clinicians"
          :key="clinician.id" :class="$style.clinicians">
        <li>
          <p><clinician-icon/>&nbsp;{{ clinician.displayName | truncate(24) }}</p>
        </li>
      </ul>
    </div>
  </div>
</template>


<script>
/* eslint-disable import/extensions */
import { find, get } from 'lodash/fp';
import { mapGetters } from 'vuex';
import moment from 'moment';
import LocationIcon from '../components/icons/LocationIcon';
import ClinicianIcon from '../components/icons/ClinicianIcon';

export default {
  components: {
    ClinicianIcon,
    LocationIcon,
  },
  props: {
    slotId: {
      type: String,
      default: '',
    },
    alwaysDeselect: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      currentSlotId: '',
    };
  },
  computed: {
    appointmentSlot() {
      return (
        find(slot => slot.id === this.slotId)(this.$store.state.appointmentSlots.slots) || {}
      );
    },
    appointmentSelected() {
      const slotID = this.currentSlotId;
      const deselect = this.alwaysDeselect;
      const { selectedSlotId } = this.$store.state.appointmentSlots;

      return selectedSlotId === slotID && !deselect
        ? this.$style.selectedContainer
        : this.$style.container;
    },
    appointmentSession() {
      return get('appointmentSession.displayName')(this.appointmentSlot);
    },
    location() {
      return get('location.displayName')(this.appointmentSlot);
    },
    ...mapGetters({
      isSelected: 'appointmentSlots/isSelected',
    }),
  },
  methods: {
    formatTime: dateTime => moment(dateTime).format('h:mm a'),
    formatDate: dateTime => moment(dateTime).format('dddd D MMMM YYYY'),
    select() {
      this.$store.dispatch('appointmentSlots/select', undefined);
      this.currentSlotId = undefined;
      if (!this.alwaysDeselect) {
        this.$store.dispatch('appointmentSlots/select', this.slotId);
        this.currentSlotId = this.slotId;
        this.$forceUpdate();
      }
    },
    getClass() {
      const isSelected = !this.alwaysDeselect && this.isSelected(this.slotId);
      return isSelected ? this.$style.selectedContainer : this.$style.container;
    },
  },
};
</script>

<style module lang="scss">
@import "../style/html";
@import "../style/colours";
@import "../style/spacings";

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
