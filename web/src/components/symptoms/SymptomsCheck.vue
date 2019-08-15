<template>
  <div v-if="showTemplate" :class="$style['no-padding']" data-purpose="">
    <ul :class="[$style['list-menu'], !$store.state.device.isNativeApp && $style.desktopWeb]"
        role="list">
      <li role="link">
        <analytics-tracked-tag
          id="btn_choices" :href="conditionsCheckerUrl"
          :class="$style['no-decoration']"
          :text="$t('sy01.a_z.subheader')"
          :aria-label="`${$t('sy01.a_z.subheaderAriaLabel')}. ${$t('sy01.a_z.body')}`"
          tag="a" target="_blank">
          <h2 :aria-label="$t('sy01.a_z.subheaderAriaLabel')">{{ $t('sy01.a_z.subheader') }}</h2>
          <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
            {{ $t('sy01.a_z.body') }}</p>
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag
          id="btn_111" :href="symptomsCheckerUrl"
          :class="$style['no-decoration']"
          :text="$t('sy01.111.subheader')"
          :aria-label="`${$t('sy01.111.subheader')}. ${$t('sy01.111.body')}`"
          tag="a" target="_blank">
          <h2>{{ $t('sy01.111.subheader') }}</h2>
          <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
            {{ $t('sy01.111.body') }}</p>
        </analytics-tracked-tag>
      </li>
      <sjr-if v-if="loggedIn" journey="cdssAdvice" tag="li">
        <analytics-tracked-tag
          id="btn_gpAdvice"
          :tabindex="-1"
          :text="$t('appointments.guidance.menuItem3.header')"
          :aria-label="`${$t('appointments.guidance.menuItem3.header')}.
            ${$t('appointments.guidance.menuItem3.text')}`"
          data-purpose="text_link">
          <a id="btn_gp_advice"
             :href="gpAdviceConditionsPath"
             :class="$style['no-decoration']"
             @click="navigate($event)">
            <h2>{{ $t('appointments.guidance.menuItem3.header') }}</h2>
            <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
              {{ $t('appointments.guidance.menuItem3.text') }}</p>
          </a>
        </analytics-tracked-tag>
      </sjr-if>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { SYMPTOMS, APPOINTMENT_GP_ADVICE_CONDITIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import SjrIf from '@/components/SjrIf';
import { createUri } from '@/lib/noJs';

export default {
  name: 'SymptomsCheck',
  components: {
    AnalyticsTrackedTag,
    SjrIf,
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
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/listmenu";
@import "../../style/desktopWeb/accessibility";

.list-menu a {
  outline: 0;

  &:focus {
    @include outlineStyle;
  }

  &:hover {
    @include outlineStyleLight;
  }
}

.no-decoration {
  text-decoration: none;
}

.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}
</style>
