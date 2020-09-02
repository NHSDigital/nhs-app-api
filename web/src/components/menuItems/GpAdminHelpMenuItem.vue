<template>
  <menu-item
    id="btn_gpHelpNoAppointment"
    header-tag="h2"
    data-purpose="text_link"
    :href="adminHelpPath"
    :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
    :description="$t('appointments.guidance.additionalGpServices.getSickNotesAndLetters')"
    :click-func="navigate"
    :click-param="adminHelpPath"
    :aria-label="ariaLabelCaption(
      'appointments.guidance.additionalGpServices.additionalGpServices',
      'appointments.guidance.additionalGpServices.getSickNotesAndLetters')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  APPOINTMENT_ADMIN_HELP_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'GpAdminHelpMenuItem',
  components: {
    MenuItem,
  },
  props: {
    previousRoute: {
      type: String,
      required: true,
      default: undefined,
    },
  },
  data() {
    return {
      adminHelpPath: APPOINTMENT_ADMIN_HELP_PATH,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    navigate(path) {
      this.$store.dispatch('onlineConsultations/setPreviousRoute', this.previousRoute);
      redirectTo(this, path);
    },
  },
};
</script>
