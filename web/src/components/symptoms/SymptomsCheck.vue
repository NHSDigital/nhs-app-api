<template>
  <div v-if="showTemplate" data-purpose="">
    <menu-item-list>
      <menu-item id="btn_choices"
                 header-tag="h2"
                 role="link"
                 data-purpose="text_link"
                 target="_blank"
                 :href="conditionsCheckerUrl"
                 :description="$t('sy01.a_z.body')"
                 :text="$t('sy01.a_z.subheader')"
                 :aria-label="ariaLabelCaption(
                   'sy01.a_z.subheaderAriaLabel',
                   'sy01.a_z.body')"/>

      <menu-item id="btn_111"
                 header-tag="h2"
                 role="link"
                 :href="symptomsCheckerUrl"
                 :description="$t('sy01.111.body')"
                 target="_blank"
                 :text="$t('sy01.111.subheader')"
                 :aria-label="ariaLabelCaption(
                   'sy01.111.subheader',
                   'sy01.111.body')"/>

      <menu-item v-if="loggedIn && isCdssAdvice"
                 id="btn_gpAdvice"
                 header-tag="h2"
                 data-purpose="text_link"
                 role="link"
                 :description="$t('appointments.guidance.menuItem3.text')"
                 :click-func="navigate"
                 :href="gpAdviceConditionsPath"
                 :text="$t('appointments.guidance.menuItem3.header')"
                 :aria-label="ariaLabelCaption(
                   'appointments.guidance.menuItem3.header',
                   'appointments.guidance.menuItem3.text')"/>
    </menu-item-list>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { APPOINTMENT_GP_ADVICE_CONDITIONS, SYMPTOMS } from '@/lib/routes';
import srjIf from '@/lib/sjrIf';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { redirectTo } from '@/lib/utils';
import { createUri } from '@/lib/noJs';

export default {
  name: 'SymptomsCheck',
  components: {
    MenuItemList,
    MenuItem,
  },
  data() {
    let symptomsCheckerUrl = this.$store.app.$env.SYMPTOM_CHECKER_URL;
    if (this.$store.state.device.isNativeApp) {
      symptomsCheckerUrl += this.$store.app.$env.SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS;
    }
    return {
      symptomsCheckerUrl,
      conditionsCheckerUrl: this.$store.app.$env.CONDITIONS_CHECKER_URL,
      symptomsPath: SYMPTOMS.path,
    };
  },
  computed: {
    loggedIn() {
      return this.$store.getters['session/isLoggedIn'];
    },
    isCdssAdvice() {
      return srjIf({ $store: this.$store, journey: 'cdssAdvice' });
    },
    gpAdviceConditionsPath() {
      const noJsData = {
        onlineConsultations: {
          previousRoute: this.symptomsPath,
        },
      };
      return createUri({ path: APPOINTMENT_GP_ADVICE_CONDITIONS.path, noJs: noJsData });
    },
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname, null);
      event.preventDefault();

      if (event.currentTarget.pathname === this.gpAdviceConditionsPath) {
        this.$store.dispatch('onlineConsultations/setPreviousRoute', this.symptomsPath);
        this.$store.dispatch('navigation/setNewMenuItem', 0);
      }
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
