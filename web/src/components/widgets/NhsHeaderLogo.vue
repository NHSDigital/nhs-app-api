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
  methods: {
    onClick() {
      redirectTo(this, this.indexPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/nhs-header-logo";
</style>
