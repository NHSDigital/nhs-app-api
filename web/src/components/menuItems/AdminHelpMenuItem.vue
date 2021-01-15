<template>
  <menu-item
    id="btn_adminHelp"
    header-tag="h2"
    data-purpose="text_link"
    :href="adminHelpPath"
    :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
    :description="$t(description)"
    :click-func="navigate"
    :aria-label="ariaLabelCaption(
      'appointments.guidance.additionalGpServices.additionalGpServices',
      description)"/>
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
  props: {
    description: {
      type: String,
      required: false,
      default: 'appointments.guidance.additionalGpServices.getSickNotesAndLetters',
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
    navigate() {
      redirectTo(this, this.adminHelpPath);
    },
  },
};
</script>
