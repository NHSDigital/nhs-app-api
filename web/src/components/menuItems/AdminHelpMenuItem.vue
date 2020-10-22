<template>
  <menu-item
    id="btn_adminHelp"
    header-tag="h2"
    data-purpose="text_link"
    :href="adminHelpPath"
    :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
    :description="$t('appointments.guidance.additionalGpServices.getSickNotesAndLetters')"
    :click-func="navigate"
    :aria-label="ariaLabelCaption(
      'appointments.guidance.additionalGpServices.additionalGpServices',
      'appointments.guidance.additionalGpServices.getSickNotesAndLetters')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { APPOINTMENT_ADMIN_HELP_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'AdminHelpMenuItem',
  components: {
    MenuItem,
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
    navigate() {
      redirectTo(this, this.adminHelpPath);
    },
  },
};
</script>
