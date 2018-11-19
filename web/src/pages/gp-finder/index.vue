<template>
  <div v-if="showTemplate">
    <h3>{{ $t('th02.heading1') }}</h3>
    <h4>{{ $t('th02.heading2') }}</h4>
    <p id="search-label">{{ $t('th02.hintText') }}</p>
    <form :action="gpFinderResultsPath" method="GET">
      <generic-text-input id="searchTextInput"
                          ref="search"
                          :class="$style.searchTextInput"
                          :required="true"
                          :type="'text'"
                          :a-labelled-by="'search-label'"
                          input-name="searchQuery"
                          maxlength="150"/>
      <generic-button :class="[$style.button, $style.green]" :type="'submit'">
        {{ $t('th02.callToAction') }}
      </generic-button>
    </form>
    <a>{{ $t('th02.hasAnAccountLink') }}</a>
  </div>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import { GP_FINDER_RESULTS } from '@/lib/routes';

export default {
  components: {
    GenericTextInput,
    GenericButton,
  },
  computed: {
    gpFinderResultsPath() {
      return GP_FINDER_RESULTS.path;
    },
  },
  layout: 'throttling',
};
</script>

<style module lang="scss" scoped>
@import '../../style/elements';
@import '../../style/buttons';
@import '../../style/throttling/gpfinder';
</style>
