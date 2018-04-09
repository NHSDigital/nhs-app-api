<template>
  <header class="home-header">
      <div class="spacer">
      </div>
      <div class="nhso_logo">
        <nhs-logo/>
      </div>

      <h1>{{ $t('homeHeader.welcome') }}</h1>

      <symptom-banner/>
    </header>
</template>

<script>
import SymptomBanner from '@/components/SymptomBanner';
import NhsLogo from '@/components/icons/NhsLogo';

export default {
  components: {
    SymptomBanner,
    NhsLogo,
  },
};
</script>

<style lang="scss"  scoped>
@import '../style/textstyles';
@import '../style/buttons';
@import '../style/icons';
header.home-header {
  background: $home_gradient;
  height: 65vh;
  padding: 24px;
  box-sizing: border-box;
  text-align: center;
  box-shadow: 0 0 5px rgba(0, 0, 0, .5);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;

  h1 {
    @include h1;
  }

  h2 {
    @include h2;
    margin-bottom: 16px;
  }

  .nhso_logo {
    margin-top: 0;
  }
  .spacer {
    width: 100%;
    .account {
      float: right;
    }
  }
}

header.logged {
  bottom: 286px;
}
</style>
