<template>
  <menu-item
    id="btn_gpAdvice"
    data-purpose="text_link"
    header-tag="h2"
    :href="gpAdviceConditionsPath"
    :text="$t('appointments.guidance.askGp.forAdvice')"
    :description="$t('appointments.guidance.askGp.consultThroughOnlineForm')"
    :click-func="navigate"
    :click-param="gpAdviceConditionsPath"
    :aria-label="ariaLabelCaption(
      'appointments.guidance.askGp.forAdvice',
      'appointments.guidance.askGp.consultThroughOnlineForm')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  APPOINTMENT_GP_ADVICE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'GpAdviceMenuItem',
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
      gpAdviceConditionsPath: APPOINTMENT_GP_ADVICE_PATH,
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
