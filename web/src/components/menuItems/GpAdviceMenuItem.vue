<template>
  <menu-item
    id="btn_gpAdvice"
    data-purpose="text_link"
    header-tag="h2"
    :href="gpAdvicePath"
    :text="$t('adviceCheck.gpAdvice.askYourGpForAdvice')"
    :description="$t('adviceCheck.gpAdvice.consultThroughOnlineForm')"
    :click-func="navigate"
    :aria-label="ariaLabelCaption(
      'adviceCheck.gpAdvice.askYourGpForAdvice',
      'adviceCheck.gpAdvice.consultThroughOnlineForm')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { GP_ADVICE_PATH } from '@/router/paths';
import { redirectTo, isBlankString } from '@/lib/utils';

export default {
  name: 'GpAdviceMenuItem',
  components: {
    MenuItem,
  },
  props: {
    routeCrumb: {
      type: String,
      required: false,
      default: undefined,
    },
  },
  data() {
    return {
      gpAdvicePath: GP_ADVICE_PATH,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    navigate() {
      if (!isBlankString(this.routeCrumb)) {
        this.$store.dispatch('navigation/setRouteCrumb', this.routeCrumb);
      }
      redirectTo(this, this.gpAdvicePath);
    },
  },
};
</script>
