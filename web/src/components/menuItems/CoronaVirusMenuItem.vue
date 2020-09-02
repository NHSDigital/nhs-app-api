<template>
  <menu-item
    id="btn_corona"
    header-tag="h2"
    :href="coronaCheckerUrl"
    :description="$t('menuItems.corona.findOutWhatToDoIfYouHaveCoronavirus')"
    target="_blank"
    :text="$t('menuItems.corona.getAdviceAboutCoronavirus')"
    :aria-label="ariaLabelCaption(
      'menuItems.corona.getAdviceAboutCoronavirus',
      'menuItems.corona.findOutWhatToDoIfYouHaveCoronavirus')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';

export default {
  name: 'CoronaVirusMenuItem',
  components: {
    MenuItem,
  },
  data() {
    return {
      coronaCheckerUrl: this.$store.$env.CORONA_SERVICE_URL,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
