<template>
  <div v-if="showTemplate" :class="$style['no-padding']" data-purpose="">
    <ul :class="[$style['list-menu'], !$store.state.device.isNativeApp && $style.desktopWeb]"
        role="list">
      <li role="link">
        <analytics-tracked-tag
          id="btn_symptoms"
          :destination="symptomsPath"
          :class="$style['no-decoration']"
          :text="$t('appointments.guidance.menuItem1.header')"
          :aria-label="`${$t('appointments.guidance.menuItem1.subheaderAriaLabel')}.
            ${$t('appointments.guidance.menuItem1.text')}`">
          <a id="checkSymptomsLink" @click="onCheckSymptomClicked">
            <h2>{{ $t('appointments.guidance.menuItem1.header') }}</h2>
            <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
              {{ $t('appointments.guidance.menuItem1.text') }}</p>
          </a>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { SYMPTOMS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'AppointmentGuidanceMenu',
  components: {
    AnalyticsTrackedTag,
  },
  computed: {
    symptomsPath() {
      return SYMPTOMS.path;
    },
  },
  methods: {
    onCheckSymptomClicked() {
      redirectTo(this, SYMPTOMS.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
  @import '../../style/listmenu';
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
