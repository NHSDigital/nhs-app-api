<template>
  <div v-if="showTemplate" :class="$style.flexContainer">
    <a id="help_icon" :class="$style['help-icon']"
       :href="helpAndSupportURL" target="_blank" tabindex="-1">
      <help-icon/>
    </a>
    <div :class="[$style.throttlingHeader, $style['header-container']]">
      <header>
        <div :class="$style.spacer" />
        <nhs-logo :class="$style.nhsoLogo"/>
        <div :class="$style.spacer" />
      </header>
      <throttling-banner />
    </div>
    <div :class="$style.throttlingContent">
      <h3>{{ $t('th02.heading1') }}</h3>
      <h4>{{ $t('th02.heading2') }}</h4>
      <p id="search-label">{{ $t('th02.hintText') }}</p>
      <form :action="gpFinderResultsPath" method="GET">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <error-message v-if="showError" :id="$style['error-label']" role="alert">
          {{ $t('th02.emptySearchError') }}
        </error-message>
        <generic-text-input id="searchTextInput"
                            ref="search"
                            :type="'text'"
                            :a-labelled-by="'search-label'"
                            input-name="searchQuery"
                            maxlength="150"/>
        <generic-button :class="[$style.button, $style.green]" :type="'submit'">
          {{ $t('th02.callToAction') }}
        </generic-button>
      </form>
      <a :href="skipThrottlingLink">{{ $t('th02.hasAnAccountLink') }}</a>
    </div>
  </div>
</template>

<script>
import NhsLogo from '@/components/icons/NhsLogo';
import HelpIcon from '@/components/icons/HelpIcon';
import ThrottlingBanner from '@/components/ThrottlingBanner';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import { GP_FINDER, GP_FINDER_RESULTS } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    NhsLogo,
    HelpIcon,
    ThrottlingBanner,
    ErrorMessage,
    GenericTextInput,
    GenericButton,
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
      showError: this.$route.query.error,
    };
  },
  computed: {
    gpFinderResultsPath() {
      return GP_FINDER_RESULTS.path;
    },
    skipThrottlingLink() {
      return `${GP_FINDER.path}?skip=true`;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersearch';
</style>
