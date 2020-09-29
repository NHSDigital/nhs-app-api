<template>
  <div>
    <router-link
      v-if="isLogoLink"
      id="nhs_logo"
      ref="homeLogoEl"
      :href="indexPath"
      :to="indexPath"
      :class="'nhsuk-header__link nhsuk-header__link--service'"
      :aria-label="$t('navigation.header.nhsAppOnlineHomepage')"
      @click.stop.prevent="onClick">
      <nhs-header-svg/>
      <span v-if="!$store.state.device.isNativeApp"
            id="logo-text"
            class="nhsuk-header__service-name">
        {{ $t('navigation.header.nhsAppOnline') }}</span>
    </router-link>
    <div v-else>
      <nhs-header-svg/>
      <span v-if="!$store.state.device.isNativeApp"
            id="logo-text"
            class="nhsuk-header__service-name">
        {{ $t('navigation.header.nhsAppOnline') }}</span>
    </div>
  </div>
</template>

<script>
import { FOCUS_NHSAPP_ROOT, EventBus } from '@/services/event-bus';
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import NhsHeaderSvg from '@/components/icons/NhsHeaderSVG';

export default {
  name: 'NhsHeaderLogo',
  components: {
    NhsHeaderSvg,
  },
  props: {
    indexPath: {
      default: INDEX_PATH,
      type: String,
    },
    isLogoLink: {
      type: Boolean,
      default: true,
    },
  },
  beforeMount() {
    EventBus.$on(FOCUS_NHSAPP_ROOT, this.focus);
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_ROOT, this.focus);
  },
  methods: {
    focus() {
      if (this.$store.state.device.isNativeApp) {
        this.$refs.homeLogoEl.focus();
      }
    },
    onClick() {
      redirectTo(this, this.indexPath);
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "../../style/colours";
  @import '../../style/screensizes';
  a {
    &:visited, &:active {
      color: $white;
    }

    &:hover {
      background: $hover_blue;
      box-shadow: 0 0 0 3px $hover_blue;
      color: $white;
      text-decoration: underline;
    }
  }
  @include tabletAndBelow() {
   span {
     padding-top: 0;
   }
  }
</style>


