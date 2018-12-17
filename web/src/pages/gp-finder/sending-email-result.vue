<template>
  <div>
    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ headerText }} </h1>
      <form :action="backLink" method="get">
        <input :value="'true'" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <button type="submit">
          <back-icon/>
        </button>
      </form>
    </header>
    <div v-if="showTemplate" id="mainDiv" :class="[$style.webHeader,
                                                   $style.throttlingContent, 'pull-content']">
      <h3>{{ this.$t('th06.letYouKnowText') }}</h3>
      <p>{{ this.$t('th06.gpSurgeryFeatureText') }}</p>
      <generic-button :class="[$style.button, $style.goToHomeScreenButton]"
                      @click="onReturnHomeClicked">
        {{ this.$t('th06.homeButton') }}
      </generic-button>
    </div>
  </div>
</template>

<script>
import BackIcon from '@/components/icons/BackIcon';
import HeaderSlim from '@/components/HeaderSlim';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import { LOGIN, GP_FINDER } from '@/lib/routes';
import moment from 'moment';

export default {
  layout: 'throttling',
  components: {
    HeaderSlim,
    BackIcon,
    GenericTextInput,
    GenericButton,
  },
  data() {
    return {
      headerText: this.$t('th06.header'),
      backLink: GP_FINDER.path,
    };
  },
  methods: {
    onReturnHomeClicked() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      betaCookie.Complete = true;
      this.$store.app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: moment.duration(1, 'y').asSeconds(),
        secure: this.$env.SECURE_COOKIES,
      });
      this.$router.push(LOGIN.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/elements';
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersendemail';
@import '../../style/headerslim';
</style>
